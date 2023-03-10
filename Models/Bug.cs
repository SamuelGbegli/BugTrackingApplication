
//Class for describing project bugs

using System.ComponentModel.DataAnnotations;

namespace BugTrackingApplication.Models
{
    public class Bug
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public Severity Severity { get; set; }

        public string? Description { get; set; }
        public bool IsOpen { get; set; } = true;

        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;

        public int ProjectID { get; set; }
        public Project Project { get; set; }

        //User ID Property
        public string User { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }

    public enum Severity
    {
        Low, Medium, High
    }

}