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

namespace BugTrackingApplication.Pages.Projects
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
      public Project Project { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Projects == null) return NotFound();

            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ID == id);

            if (project == null) return NotFound();
            if (project.User != _userManager.GetUserId(User)) return Forbid();

            Project = project;
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }
            var project = await _context.Projects.FindAsync(id);

            if (project != null)
            {
                Project = project;
                var BugsToRemove = _context.Bugs.Where(b => b.ProjectID == Project.ID);
                var CommentsToRemove = new List<Comment>();
                foreach (var b in BugsToRemove)
                {
                    CommentsToRemove.AddRange(_context.Comments.Where(c => c.BugID == b.ID));
                }

                _context.Comments.RemoveRange(CommentsToRemove);
                _context.Bugs.RemoveRange(BugsToRemove);

                _context.Projects.Remove(Project);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
