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

namespace BugTrackingApplication.Pages.Bugs
{
    public class DetailsModel : PageModel
    {
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(BugTrackingApplication.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

      public Bug Bug { get; set; }

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
            else
            {
                Bug = bug;
            }
            return Page();
        }

        public async Task OnPostAsync(int? id)
        {
            var bug = await _context.Bugs.FirstOrDefaultAsync(m => m.ID == id);
            if (bug != null)
            {
                bug.IsOpen = !bug.IsOpen;
                string status = bug.IsOpen ? "Opened" : "Closed";

                _context.Comments.Add(new Comment
                {
                    Text = "${status} on " + DateTime.Now.ToString(),
                    BugID = bug.ID,
                    Bug = bug,
                    CanEdit = false,
                    Created = DateTime.Now,
                    Updated = DateTime.Now

                });
                await _context.SaveChangesAsync();
            }

            await OnGetAsync(id);
        }
    }
}
