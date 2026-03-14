using System.ComponentModel.DataAnnotations;

namespace AccommodationManagement.Models
{
    public class EmergencyRequest
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTime RequestTime { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
