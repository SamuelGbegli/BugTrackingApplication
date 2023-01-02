using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugTrackingApplication.Data;
using BugTrackingApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BugTrackingApplication.Pages.Bugs
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
        ViewData["ProjectID"] = new SelectList(_context.Projects, "ID", "Name");

            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ID == id);

            if (project is null) return NotFound();
            if (project.User != _userManager.GetUserId(HttpContext.User)) return Forbid();
            Bug.ProjectID = (int)id;
            return Page();
        }

        [BindProperty]
        public Bug Bug { get; set; } = new Bug();

        public int ProjectID { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Comments.Add(new Comment
            {
                Text = "Created bug " + Bug.Title + " on " + DateTime.Now.ToString(),
                BugID = Bug.ID,
                Bug = Bug,
                CanEdit = false,
                Created = DateTime.Now,
                Updated = DateTime.Now
            });

            Bug.Created = DateTime.Now;
            Bug.Updated = DateTime.Now;
            Bug.IsOpen = true;

            Bug.User = _userManager.GetUserId(HttpContext.User); 

            Bug.Project =  _context.Projects.First(p => p.ID == Bug.ProjectID);
            Bug.Project.Updated = DateTime.Now;

            _context.Bugs.Add(Bug);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { id = Bug.ProjectID}) ;
        }
    }
}
