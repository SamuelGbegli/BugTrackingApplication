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
    public class IndexModel : PageModel
    {
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;

        public IndexModel(BugTrackingApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Comment> Comment { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (_context.Comments != null)
            {
                var commentIQ = from c in _context.Comments
                                select c;
                
                if(id is not null)
                    commentIQ = commentIQ.Where(c => c.BugID == id);

                Comment = await commentIQ
                .Include(c => c.Bug).ToListAsync();

                return Page();
            }
            return NotFound();
        }
    }
}
