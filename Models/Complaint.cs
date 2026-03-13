using AccommodationManagement.Models;

namespace AccommodationManagement.Models
{
    public class Complaint
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending / Resolved
        public DateTime Date { get; set; } = DateTime.Now;

        public ApplicationUser User { get; set; } = null!;
    }
}