using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugTrackingApplication.Data;
using BugTrackingApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingApplication.Pages.Comments
{
    public class CreateModel : PageModel
    {
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(BugTrackingApplication.Data.ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
        ViewData["BugID"] = new SelectList(_context.Bugs, "ID", "Title");

            Comment.Bug = await _context.Bugs.FirstOrDefaultAsync(b => b.ID == id);
            Comment.BugID = (int)id;

            return Page();
        }

        [BindProperty]
        public Comment Comment { get; set; } = new Comment();
        
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid)
            {

                return Page();
            }
    

            Comment.Created = DateTime.Now;
            Comment.Updated = DateTime.Now;
            Comment.CanEdit = true;
            Comment.User = _userManager.GetUserId(HttpContext.User);

            Comment.Bug.Updated = DateTime.Now;
            Comment.Bug.Project.Updated = DateTime.Now;

            _context.Comments.Add(Comment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { id = Comment.BugID});
        }
    }
}
