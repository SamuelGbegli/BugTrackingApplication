using BugTrackingApplication.Data;
using BugTrackingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingApplication.Pages
{
    [Authorize]
    public class ProjectsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty(SupportsGet = true)]
        public string Sort { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Order { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool OpenBugsOnly { get; set; }

        public ProjectsModel (ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Project> Projects { get; set; }

        public async Task OnGetAsync(string sort, string order, bool openbugsonly)
        {
            Sort = sort;
            Order = order;
            OpenBugsOnly = openbugsonly;

            if (_context.Projects != null)
            {
                var user = _userManager.GetUserId(HttpContext.User);

                Projects = await _context.Projects
                    .Where(p => p.User == user)
                    .Include(p => p.Bugs)
                    .ToListAsync();

                if (openbugsonly) Projects = Projects.Where(p => p.Bugs.Any(b => b.IsOpen)).ToList();

                switch (sort)
                {
                    case "Date created":
                        if (order == "Ascending") Projects = Projects.OrderBy(p => p.Created).ToList();
                        else Projects = Projects.OrderByDescending(p => p.Created).ToList();
                        break;
                    case "Name":
                        if (order == "Ascending") Projects = Projects.OrderBy(p => p.Name).ToList();
                        else Projects = Projects.OrderByDescending(p => p.Name).ToList();
                        break;
                    case "Open bugs":
                        if (order == "Ascending") Projects = Projects.OrderBy(p => p.Bugs.Count()).ToList();
                        else Projects = Projects.OrderByDescending(p => p.Bugs.Count()).ToList();
                        break;
                    default:
                        if (order == "Ascending") Projects = Projects.OrderBy(p => p.Updated).ToList();
                        else Projects = Projects.OrderByDescending(p => p.Updated).ToList();
                        break;
                }

            }
        }
    }
}
