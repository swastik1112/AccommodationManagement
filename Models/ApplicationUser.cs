using AccommodationManagement.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AccommodationManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        // New field
        [Display(Name = "Emergency Contact")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? EmergencyContact { get; set; } = string.Empty;
        public int? RoomId { get; set; }                     // Quick reference
        public Room? Room { get; set; }
        public string Role { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now; // for Join Date
    }
}