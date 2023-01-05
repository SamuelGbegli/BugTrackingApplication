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

namespace BugTrackingApplication.Pages.Bugs
{
    public class IndexModel : PageModel
    {
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public string Sort { get; set; }
        public string Order { get; set; }

        public IndexModel(BugTrackingApplication.Data.ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Project Project { get; set; }
        public IList<Bug> Bugs { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, string sort, string order)
        {
            if (_context.Bugs != null && id is not null)
            {
                Sort = sort;
                Order = order;

                var project = _context.Projects.Find(id);
                var bugsIQ = from b in _context.Bugs
                             select b;

                bugsIQ = bugsIQ.OrderByDescending(b => b.Updated);

                if (project != null)
                {

                    if (project.User != _userManager.GetUserId(HttpContext.User)) return Forbid();

                    bugsIQ = bugsIQ.Where(b => b.ProjectID == id);

                    switch (sort)
                    {
                        case "Date created":
                            if (Order == "Ascending") bugsIQ = bugsIQ.OrderBy(b => b.Created);
                            else bugsIQ = bugsIQ.OrderByDescending(b => b.Created);
                            break;
                        case "Last updated":
                            if (Order == "Ascending") bugsIQ = bugsIQ.OrderBy(b => b.Updated);
                            else bugsIQ = bugsIQ.OrderByDescending(b => b.Updated);
                            break;
                        case "Severity":
                            if (Order == "Ascending") bugsIQ = bugsIQ.OrderBy(b => b.Severity);
                            else bugsIQ = bugsIQ.OrderByDescending(b => b.Severity);
                            break;
                        default:
                            if (Order == "Ascending") bugsIQ = bugsIQ.OrderBy(b => b.Title);
                            else bugsIQ = bugsIQ.OrderByDescending(b => b.Title);
                            break;
                    }

                    Project = _context.Projects.Find(id);
                    Bugs = await bugsIQ.ToListAsync();
                    return Page();
                }
            }
            return NotFound();
        }
    }
}
