using AccommodationManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace AccommodationManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int? RoomId { get; set; }                     // Quick reference
        public Room? Room { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}