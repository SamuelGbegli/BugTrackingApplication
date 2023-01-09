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

namespace BugTrackingApplication.Pages
{
    public class DeleteBugModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteBugModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            if (bug.User != _userManager.GetUserId(HttpContext.User)) return Forbid();

            Bug = bug;

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
                _context.Bugs.Remove(Bug);
                _context.Projects.Find(ProjectID).Updated = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Bugs", new { id = ProjectID});
        }
    }
}
