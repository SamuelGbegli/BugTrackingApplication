using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingApplication.Models
{
    public class Comment
    {
        public int ID { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;

        public bool CanEdit { get; set; }

        public int BugID { get; set; }
        public Bug Bug { get; set; }



        public IdentityUser User { get; set; }
    }
}