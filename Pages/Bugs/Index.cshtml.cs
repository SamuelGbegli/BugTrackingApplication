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
using NuGet.Packaging;

namespace BugTrackingApplication.Pages.Bugs
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public string Sort { get; set; } = "Last updated";
        public string Order { get; set; } = "Descending";

        public List<Severity> SelectedSeverities { get; set; } = new List<Severity>();
        public string OpenFilter { get; set; }

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Project Project { get; set; }
        public IList<Bug> Bugs { get; set; }

        public int TotalBugCount { get; set; }
        public int TotalBugsOpen { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string sort, string order,
            string openfilter, string[] severity)
        {
            if (_context.Bugs != null && id is not null)
            {
                Sort = sort;
                Order = order;
                OpenFilter = openfilter;

                if (severity.Contains("l")) SelectedSeverities.Add(Severity.Low);
                if (severity.Contains("m")) SelectedSeverities.Add(Severity.Medium);
                if (severity.Contains("h")) SelectedSeverities.Add(Severity.High);

                Project = await _context.Projects
                    .Include(p => p.Bugs)
                    .FirstAsync(p => p.ID == id);

                var bugsIQ = from b in Project.Bugs
                             select b;

                if (SelectedSeverities.Any())
                {
                    bugsIQ = bugsIQ.Where(b => SelectedSeverities.Contains(b.Severity));
                }

                if (Project != null)
                {

                    if (Project.User != _userManager.GetUserId(HttpContext.User)) return Forbid();
                    bugsIQ = bugsIQ.Where(b => b.ProjectID == id);

                    TotalBugCount = _context.Bugs.Where(b => b.ProjectID == id).Count();
                    TotalBugsOpen = _context.Bugs.Where(b => b.ProjectID == id && b.IsOpen).Count();

                    switch (OpenFilter)
                    {
                        case "Open bugs only":
                            bugsIQ = bugsIQ.Where(b => b.IsOpen);
                            break;
                        case "Closed bugs only":
                            bugsIQ = bugsIQ.Where(b => !b.IsOpen);
                            break;
                    }

                    switch (sort)
                    {
                        case "Date created":
                            if (Order == "Ascending") bugsIQ = bugsIQ.OrderBy(b => b.Created);
                            else bugsIQ = bugsIQ.OrderByDescending(b => b.Created);
                            break;
                        case "Title":
                            if (Order == "Ascending") bugsIQ = bugsIQ.OrderBy(b => b.Title);
                            else bugsIQ = bugsIQ.OrderByDescending(b => b.Title);
                            break;
                        case "Severity":
                            if (Order == "Ascending") bugsIQ = bugsIQ.OrderBy(b => b.Severity);
                            else bugsIQ = bugsIQ.OrderByDescending(b => b.Severity);
                            break;
                        default:
                            if (Order == "Ascending") bugsIQ = bugsIQ.OrderBy(b => b.Updated);
                            else bugsIQ = bugsIQ.OrderByDescending(b => b.Updated);
                            break;
                    }

                    Project = _context.Projects.Find(id);
                    Bugs = bugsIQ.ToList();

                    return Page();
                }
            }
            return NotFound();
        }


    }
}
    