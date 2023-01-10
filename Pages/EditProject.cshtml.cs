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
using Microsoft.AspNetCore.Authorization;

namespace BugTrackingApplication.Pages
{
    [Authorize]
    public class EditProjectModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditProjectModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Projects == null) return NotFound();

            var project =  await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();
            if(project.User != _userManager.GetUserId(HttpContext.User)) return Forbid();

            Project = project;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {            

            var projectToUpdate = await _context.Projects.FindAsync(id);
            if (projectToUpdate == null) return NotFound();
            if (projectToUpdate.User != _userManager.GetUserId(HttpContext.User)) return Forbid();

            if (await TryUpdateModelAsync<Project>(
                projectToUpdate, "project",
                p => p.Name, p => p.Description, p => p.Link))
            {
                projectToUpdate.Updated = DateTime.Now;

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }

        private bool ProjectExists(int id)
        {
          return _context.Projects.Any(e => e.ID == id);
        }
    }
}
