using System.ComponentModel.DataAnnotations;

namespace AccommodationManagement.Models
{
    public class MessMenu
    {
        public int Id { get; set; }

        [Required]
        public DateTime MenuDate { get; set; }

        public string Breakfast { get; set; } = string.Empty;

        public string Lunch { get; set; } = string.Empty;

        public string Dinner { get; set; } = string.Empty;

        public string? SpecialNote { get; set; }

        public string AddedBy { get; set; } = string.Empty; // Warden
    }
}
