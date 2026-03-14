using System.ComponentModel.DataAnnotations;

namespace AccommodationManagement.Models
{
    // ViewModels/EditProfileViewModel.cs
    public class EditProfileViewModel
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Emergency Contact")]
        public string? EmergencyContact { get; set; }

        [Display(Name = "Room Number")]
        public string? RoomNumber { get; set; }

        [Display(Name = "Bed Number")]
        public string? BedNumber { get; set; }

        [Display(Name = "Joined On")]
        public DateTime JoinDate { get; set; }
    }
}
