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
using Microsoft.AspNetCore.Authorization;

namespace BugTrackingApplication.Pages
{
    [Authorize]
    public class AddProjectModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AddProjectModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Project Project { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {

            Project.User = _userManager.GetUserId(HttpContext.User);

            var emptyProject = new Project();

            if (await TryUpdateModelAsync<Project>(
                emptyProject, "project",
                p => p.Name, p => p.Description, p => p.Link,
                p => p.Created, p => p.Updated, p => p.User)
                )
            {
                _context.Projects.Add(Project);

                Project.Created = DateTime.Now;
                Project.Updated = DateTime.Now;

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
