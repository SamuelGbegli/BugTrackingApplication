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

namespace BugTrackingApplication.Pages.Comments
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Comment> Comments { get;set; } = default!;
        public Bug Bug { get; set; }

        [BindProperty]
        public Comment Comment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (_context.Comments != null && id is not null)
            {

            var bug = await _context.Bugs
                    .Include(b => b.Comments).FirstAsync(b => b.ID == id);

                if (bug.User != _userManager.GetUserId(HttpContext.User)) return Forbid();

                Bug = bug;

                Comment = new Comment
                {
                    BugID = bug.ID,
                    CanEdit = true,
                    User = _userManager.GetUserId(HttpContext.User),
                    Bug = bug
                };

                    return Page();
                
            }
            return NotFound();
        }
    
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var emptyComment = new Comment();

            if(await TryUpdateModelAsync<Comment>(
                emptyComment, "comment",
                c => c.Text, c => c.CanEdit, c=> c.User)
                )
            {
                var bug = await _context.Bugs
                    .Include(b => b.Project)
                    .FirstAsync(b => b.ID == id);

                Comment.Created = DateTime.Now;
                Comment.Updated = DateTime.Now;

                bug.Updated = DateTime.Now;
                bug.Project.Updated = DateTime.Now;


                _context.Comments.Add(Comment);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new { id = Comment.BugID });
            }
            return NotFound();

        }
    }
}
