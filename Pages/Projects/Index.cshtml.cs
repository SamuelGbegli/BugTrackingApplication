using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugTrackingApplication.Data;
using BugTrackingApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace BugTrackingApplication.Pages.Projects
{
    public class IndexModel : PageModel
    {
        private readonly BugTrackingApplication.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(BugTrackingApplication.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;            
        }

        public IList<Project> Project { get;set; } = default!;


        public async Task OnGetAsync()
        {
            var user = _userManager.GetUserId(HttpContext.User);

            var projectsIQ = from p in _context.Projects
                             select p;
            projectsIQ = projectsIQ.Where(p => p.User == user);

            projectsIQ = projectsIQ.OrderByDescending(p => p.Updated);

            if (_context.Projects != null)
            {
                Project = await projectsIQ.ToListAsync();
                
            }
        }
    }
}
