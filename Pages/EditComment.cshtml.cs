using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTrackingApplication.Data;
using BugTrackingApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace BugTrackingApplication.Pages
{
    public class EditCommentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditCommentModel(ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Comment Comment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment =  await _context.Comments.FirstOrDefaultAsync(m => m.ID == id);
            if (comment == null)
            {
                return NotFound();
            }
            Comment = comment;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {

            var commentToUpdate = await _context.Comments
                .Include(c => c.Bug)
                .FirstAsync(c => c.ID == id);
            if (commentToUpdate == null) return NotFound();
            if (commentToUpdate.User != _userManager.GetUserId(HttpContext.User)) return Forbid();

            if(await TryUpdateModelAsync<Comment>(
                commentToUpdate, "comment", c => c.Text, c => c.CanEdit))
            {
                var project = await _context.Projects.FindAsync(commentToUpdate.Bug.ProjectID);

                commentToUpdate.Updated = DateTime.Now;
                commentToUpdate.Bug.Updated = DateTime.Now;
                project.Updated = DateTime.Now;

                await _context.SaveChangesAsync();

                return RedirectToPage("./Comments", new { id = commentToUpdate.BugID });
            }
            
            return Page();
        }

        private bool CommentExists(int id)
        {
          return _context.Comments.Any(e => e.ID == id);
        }
    }
}
