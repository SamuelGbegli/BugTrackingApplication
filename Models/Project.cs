using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingApplication.Models
{
    public class Project
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }

        //Date and time of project being created and updated
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public IdentityUser User { get; set; }

        public ICollection<Bug> Bugs { get; set; }
    }
}
