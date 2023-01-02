using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugTrackingApplication.Data;
using BugTrackingApplication.Models;

namespace BugTrackingApplication.Pages.Comments
{
    public class DeleteModel : PageModel
    {
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;

        public DeleteModel(BugTrackingApplication.Data.ApplicationDbContext context)
        {
            _context = context;
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

            if (comment != null)
            {
                Comment = comment;

                //Comment.Bug.Updated = DateTime.Now;
                //Comment.Bug.Project.Updated = DateTime.Now;

                _context.Comments.Remove(Comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
