using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugTrackingApplication.Data;
using BugTrackingApplication.Models;
using Microsoft.AspNetCore.Identity;
using BugTrackingApplication.Models.ViewModels;

namespace BugTrackingApplication.Pages.Projects
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty(SupportsGet = true)]
        public string Sort { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Order { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool OpenBugsOnly { get; set; }

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<ProjectViewModel> Projects { get; set; } = new List<ProjectViewModel>();

        public async Task OnGetAsync(string sort, string order, bool openbugsonly)
        {
            Sort= sort;
            Order = order;
            OpenBugsOnly = openbugsonly;

            if (_context.Projects != null)
            {
                var user = _userManager.GetUserId(HttpContext.User);

                var projectsIQ = from p in _context.Projects
                                 select p;

                projectsIQ = projectsIQ.Where(p => p.User == user);

                foreach (var project in projectsIQ)
                {
                    if (OpenBugsOnly)
                    {
                        if (_context.Bugs.Where(b => b.ProjectID == project.ID && b.IsOpen).Count() == 0) continue;
                    }
                    Projects.Add(new ProjectViewModel
                    {
                        Project = project,
                        BugCount = _context.Bugs.Where(b => b.ProjectID == project.ID && b.IsOpen).Count()
                    });
                }

                switch (sort)
                {
                    case "Date created":
                        if (order == "Ascending") Projects = Projects.OrderBy(p => p.Project.Created).ToList();
                        else Projects = Projects.OrderByDescending(p => p.Project.Created).ToList();
                        break;
                    case "Last updated":
                        if (order == "Ascending") Projects = Projects.OrderBy(p => p.Project.Updated).ToList();
                        else Projects = Projects.OrderByDescending(p => p.Project.Updated).ToList();
                        break;
                    case "Open bugs":
                        if (order == "Ascending") Projects = Projects.OrderBy(p => p.BugCount).ToList();
                        else Projects = Projects.OrderByDescending(p => p.BugCount).ToList();
                        break;
                    default:
                        if (order == "Ascending") Projects = Projects.OrderBy(p => p.Project.Name).ToList();
                        else Projects = Projects.OrderByDescending(p => p.Project.Name).ToList();
                        break;
                }

            }
        }
    
        public async Task OnPostDelete(int id, string sort, string order, bool openbugsonly)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);
                var bugs = _context.Bugs.Where(b => b.ProjectID == id).ToList();
                var comments = _context.Comments.Where(c => bugs.Any(b => b.ID == c.BugID)).ToList();

                _context.Comments.RemoveRange(comments);
                _context.Bugs.RemoveRange(bugs);
                _context.Projects.Remove(project);

                await _context.SaveChangesAsync();

                await OnGetAsync(sort, order, openbugsonly);
            }
            catch
            {
                NotFound();
            }
        }
    }
}
