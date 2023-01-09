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

namespace BugTrackingApplication.Pages
{
    public class AddBugModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AddBugModel(ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            var project = await _context.Projects.FindAsync(id);

            if (project is null) return NotFound();
            if (project.User != _userManager.GetUserId(HttpContext.User)) return Forbid();
            
            Bug.ProjectID = (int)id;
            Bug.Project = project;
            Bug.User = _userManager.GetUserId(HttpContext.User);

            return Page();
        }

        [BindProperty]
        public Bug Bug { get; set; } = new Bug();


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(int id)
        {
            Bug.Project = await _context.Projects.FindAsync(id);
            var newBug = new Bug();

            if(await TryUpdateModelAsync<Bug>(
                newBug, "bug",
                b =>b.Title, b=> b.Description, b => b.Severity,
                b => b.ProjectID, b => b.User))
            {

                _context.Bugs.Add(newBug);

                _context.Comments.Add(new Comment
                {
                    Text = "Created on " + DateTime.Now.ToString(),
                    BugID = Bug.ID,
                    Bug = newBug,
                    CanEdit = false,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    User = _userManager.GetUserId(HttpContext.User)
                });

                Bug.Project.Updated = DateTime.Now;

                await _context.SaveChangesAsync();
                return RedirectToPage("./Bugs", new {id = newBug.ProjectID});
            }
            return Page();

        }
    }
}
