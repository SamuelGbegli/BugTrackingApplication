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
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public string Sort { get; set; } = "Last updated";
        public string Order { get; set; } = "Descending";

        public bool LowSeverity { get; set; }
        public bool MidSeverity { get; set; }
        public bool HighSeverity { get; set; }

        public List<Severity> SelectedSeverities { get; set; } = new List<Severity>();
        public string OpenFilter { get; set; }

        public IndexModel(BugTrackingApplication.Data.ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Project Project { get; set; }
        public IList<Bug> Bugs { get;set; }

        public async Task<IActionResult> OnGetAsync(int? id, string sort, string order,
            string openfilter, bool lowseverity, bool medseverity, bool highseverity)
        {
            if (_context.Bugs != null && id is not null)
            {
                Sort = sort;
                Order = order;
                OpenFilter = openfilter;

                LowSeverity = lowseverity;
                MidSeverity = medseverity;
                HighSeverity = highseverity;
                
                if(LowSeverity) SelectedSeverities.Add(Severity.Low);
                if (MidSeverity) SelectedSeverities.Add(Severity.Medium);
                if (HighSeverity) SelectedSeverities.Add(Severity.High);

                if (!SelectedSeverities.Any()) SelectedSeverities = new List<Severity> {
                    Severity.Low,
                    Severity.Medium,
                    Severity.High};

                var project = _context.Projects.Find(id);
                var bugsIQ = from b in _context.Bugs
                             where SelectedSeverities.Contains(b.Severity)
                             select b;                

                if (project != null)
                {

                    if (project.User != _userManager.GetUserId(HttpContext.User)) return Forbid();
                    bugsIQ = bugsIQ.Where(b => b.ProjectID == id);                   

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
                    Bugs = await bugsIQ.ToListAsync();
                    return Page();
                }
            }
            return NotFound();
        }
    }
}
