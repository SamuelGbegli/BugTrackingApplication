using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugTrackingApplication.Data;
using BugTrackingApplication.Models;

namespace BugTrackingApplication.Pages.Bugs
{
    public class CreateModel : PageModel
    {
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;

        public CreateModel(BugTrackingApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["ProjectID"] = new SelectList(_context.Projects, "ID", "Name");
            return Page();
        }

        [BindProperty]
        public Bug Bug { get; set; }
        

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
            }) ;

            Bug.Created = DateTime.Now;
            Bug.Updated = DateTime.Now;

            _context.Bugs.Add(Bug);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
