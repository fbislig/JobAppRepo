using JobApplicationsShared.Enums;

namespace JobApplicationsShared.Models
{
    public class JobSearchCriteria
    {
        public string? Company { get; set; }
        public string? Position { get; set; }
        public string? Location { get; set; }
        public DateTime? DateApplied { get; set; }
        public Status? Status { get; set; }
    }
}
