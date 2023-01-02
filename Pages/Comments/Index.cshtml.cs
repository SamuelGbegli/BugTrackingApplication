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
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(BugTrackingApplication.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Comment> Comments { get;set; } = default!;
        public Bug Bug { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            var bug = _context.Bugs.First(b => b.ID == id);

            if (bug.User != _userManager.GetUserId(HttpContext.User)) return Forbid();

            Bug = bug;

            if (_context.Comments != null)
            {
                var commentIQ = from c in _context.Comments
                                select c;
                
                if(id is not null)
                {
                    commentIQ = commentIQ.Where(c => c.BugID == id);

                    Comments = await commentIQ
                    .Include(c => c.Bug).ToListAsync();

                    return Page();
                }
            }
            return NotFound();
        }
    }
}
