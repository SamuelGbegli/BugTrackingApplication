﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugTrackingApplication.Data;
using BugTrackingApplication.Models;

namespace BugTrackingApplication.Pages.Projects
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
            return Page();
        }

        [BindProperty]
        public Project Project { get; set; }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid)
            {
                return Page();
            }

          Project.Created = DateTime.Now;
            Project.Updated= DateTime.Now;

            _context.Projects.Add(Project);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
