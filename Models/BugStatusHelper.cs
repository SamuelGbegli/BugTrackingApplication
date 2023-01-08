namespace BugTrackingApplication.Models
{
    public class BugStatusHelper
    {
        public int Project { get; set; }
        public int BugID { get; set; }
        public string Sort { get; set; }
        public string Order { get; set; }

        public bool LowSeverity { get; set; }
        public bool MidSeverity { get; set; }
        public bool HighSeverity { get; set; }
        public string OpenFilter { get; set; }
    }
}
