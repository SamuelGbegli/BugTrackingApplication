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

namespace BugTrackingApplication.Pages.Bugs
{
    public class EditModel : PageModel
    {
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(BugTrackingApplication.Data.ApplicationDbContext context,
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

            var bug =  await _context.Bugs.FirstOrDefaultAsync(m => m.ID == id);
            if (bug == null)
            {
                return NotFound();
            }
            if (bug.User != _userManager.GetUserId(HttpContext.User)) return Forbid();

            Bug = bug;
           ViewData["ProjectID"] = new SelectList(_context.Projects, "ID", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {


            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Bug).State = EntityState.Modified;

            try
            {

                _context.Projects.First(p => p.ID == Bug.ProjectID).Updated = DateTime.Now;
                Bug.Updated = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BugExists(Bug.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BugExists(int id)
        {
          return _context.Bugs.Any(e => e.ID == id);
        }
    }
}
