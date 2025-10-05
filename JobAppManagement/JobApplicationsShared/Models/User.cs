namespace JobApplicationsShared.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); // unique identifier for auth later
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // Optional: add password hash if you implement auth manually
    }
}
