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

            var bug = _context.Bugs.First(b => b.ID == id);

            if (bug.User != _userManager.GetUserId(HttpContext.User)) return Forbid();

            Bug = bug;

            if (_context.Comments != null && id is not null)
            {
                var commentIQ = from c in _context.Comments
                                select c;
                    commentIQ = commentIQ.Where(c => c.BugID == id);

                commentIQ = commentIQ.OrderByDescending(c => c.Updated);

                    Comments = await commentIQ
                    .Include(c => c.Bug).ToListAsync();

                Comment = new Comment
                {
                    BugID = (int)id,
                    CanEdit = true,
                    User = _userManager.GetUserId(HttpContext.User),
                    Bug = _context.Bugs.FirstOrDefault(b => b.ID == id)
                };

                    return Page();
                
            }
            return NotFound();
        }
    
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                Comment.Created = DateTime.Now;
                Comment.Updated = DateTime.Now;
                Comment.CanEdit = true;

                var bug = _context.Bugs.First(b => b.ID == Comment.BugID);
                var project = _context.Projects.First(p => p.ID == bug.ProjectID);

                bug.Updated = DateTime.Now;
                project.Updated = DateTime.Now;
                Comment.User = _userManager.GetUserId(HttpContext.User);

                _context.Comments.Add(Comment);

                await _context.SaveChangesAsync();

                return RedirectToPage("./Index", new { id = Comment.BugID });
            }
            return NotFound();
        }
    }
}
