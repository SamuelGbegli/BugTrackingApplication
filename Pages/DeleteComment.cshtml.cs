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
using Microsoft.AspNetCore.Authorization;

namespace BugTrackingApplication.Pages
{
    [Authorize]
    public class DeleteCommentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteCommentModel(ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
      public Comment Comment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FirstOrDefaultAsync(m => m.ID == id);

            if (comment == null)
            {
                return NotFound();
            }

            if (comment.User != _userManager.GetUserId(User)) return Forbid();

            else
            {
                Comment = comment;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }
            var comment = await _context.Comments.FindAsync(id);

            var bug = _context.Bugs.First(b => b.ID == comment.BugID);
            var project = _context.Projects.First(p => p.ID == bug.ProjectID);

            if (comment != null)
            {
                Comment = comment;


                bug.Updated = DateTime.Now;
                project.Updated = DateTime.Now;

                _context.Comments.Remove(Comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Comments", new { id =  bug.ID});
        }
    }
}
