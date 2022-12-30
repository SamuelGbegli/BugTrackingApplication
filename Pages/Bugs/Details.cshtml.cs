using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugTrackingApplication.Data;
using BugTrackingApplication.Models;

namespace BugTrackingApplication.Pages.Bugs
{
    public class DetailsModel : PageModel
    {
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;

        public DetailsModel(BugTrackingApplication.Data.ApplicationDbContext context)
        {
            _context = context;
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
            else 
            {
                Bug = bug;
            }
            return Page();
        }
    }
}
