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

        public IndexModel(BugTrackingApplication.Data.ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Project Project { get; set; }
        public IList<Bug> Bugs { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (_context.Bugs != null && id is not null)
            {
                var project = _context.Projects.Find(id);
                var bugsIQ = from b in _context.Bugs
                             select b;

                    
                    if (project != null)
                    {

                    if (project.User != _userManager.GetUserId(HttpContext.User)) return Forbid();
                        bugsIQ = bugsIQ.Where(b => b.ProjectID == id);

                        Project = _context.Projects.Find(id);
                        Bugs = await bugsIQ.ToListAsync();
                        return Page();
                    }
            }
            return NotFound();
        }
    }
}
