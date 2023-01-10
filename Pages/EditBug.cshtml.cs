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
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace BugTrackingApplication.Pages
{
    [Authorize]
    public class EditBugModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditBugModel(ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Bug Bug { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Bugs == null)
            {
                return NotFound();
            }

            var bug = await _context.Bugs
                .Include(b => b.Project).FirstAsync(b => b.ID == id);

            if (bug == null)
            {
                return NotFound();
            }
            if (bug.User != _userManager.GetUserId(HttpContext.User)) return Forbid();

            Bug = bug;            
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {
            Comment comment = null;

            var bugToUpdate = await _context.Bugs
                .Include(b => b.Project).FirstOrDefaultAsync(b => b.ID == id);
            if (bugToUpdate == null) return NotFound();
            if(bugToUpdate.User != _userManager.GetUserId(HttpContext.User)) return Forbid();

            if (bugToUpdate.Severity != Bug.Severity)
                comment = new Comment
                {
                    Text = "Changed severity to " + Bug.Severity,
                    BugID = Bug.ID,
                    Bug = bugToUpdate,
                    CanEdit = false,
                    User = _userManager.GetUserId(HttpContext.User)
                };

            if(await TryUpdateModelAsync<Bug>(
                bugToUpdate, "bug",
                b => b.Title, b=> b.Severity, b => b.Description))
            {
                bugToUpdate.Updated = DateTime.Now;
                bugToUpdate.Project.Updated = DateTime.Now;
                
                if (comment is not null)
                {
                    comment.Created = DateTime.Now;
                    comment.Updated = DateTime.Now;
                    _context.Comments.Add(comment);
                }

                await _context.SaveChangesAsync();
                return RedirectToPage("./Bugs", new { id = bugToUpdate.ProjectID });
            }          

            return Page();
        }

        private bool BugExists(int id)
        {
          return _context.Bugs.Any(e => e.ID == id);
        }
    }
}
