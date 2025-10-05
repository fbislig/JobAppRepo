using JobApplicationsShared.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobApplicationsShared.Models
{
    public class JobApplication
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string Company { get; set; }
  
        public string? Location { get; set; }
        public DateTime DateApplied { get; set; } = DateTime.Today;
        public Status Status { get; set; } = Status.Applied;
        public string UserId { get; set; } = string.Empty;
    }
}
