//View model for a project and its bugs

namespace BugTrackingApplication.Models.ViewModels
{
    public class ProjectViewModel
    {
        public Project Project { get; set; }
        public IList<Bug> Bugs { get; set; }
    }
}
