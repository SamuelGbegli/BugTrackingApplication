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
    public class DeleteModel : PageModel
    {
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;

        public DeleteModel(BugTrackingApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Bug Bug { get; set; }
        public int ProjectID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Bugs == null)
            {
                return NotFound();
            }

            var bug = await _context.Bugs.FirstOrDefaultAsync(m => m.ID == id);

            if (bug == null)
            {
                return NotFound();
            }
            else 
            {
                Bug = bug;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Bugs == null)
            {
                return NotFound();
            }
            var bug = await _context.Bugs.FindAsync(id);

            if (bug != null)
            {
                Bug = bug;
                ProjectID = bug.ProjectID;
                //Bug.Project.Updated = DateTime.Now;
                _context.Bugs.Remove(Bug);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index", new { id = ProjectID});
        }
    }
}
