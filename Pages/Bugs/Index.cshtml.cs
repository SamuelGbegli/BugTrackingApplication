using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugTrackingApplication.Data;
using BugTrackingApplication.Models;

namespace BugTrackingApplication.Pages.Bugs
{
    public class IndexModel : PageModel
    {
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;

        public IndexModel(BugTrackingApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Project Project { get; set; }
        public IList<Bug> Bugs { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (_context.Bugs != null)
            {
                var bugsIQ = from b in _context.Bugs
                             select b;

                if (id is not null)
                {
                    if (_context.Projects.Find(id) != null)
                    {
                        bugsIQ = bugsIQ.Where(b => b.ProjectID == id);

                        Project = _context.Projects.Find(id);
                        Bugs = await bugsIQ.ToListAsync();
                        return Page();
                    }
                    else return NotFound();
                }
                    
                Bugs = await bugsIQ
                .Include(b => b.Project).ToListAsync();
                return Page();
            }
            return NotFound();
        }
    }
}
