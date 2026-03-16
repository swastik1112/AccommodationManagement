namespace AccommodationManagement.Models
{
    public class DigitalIdViewModel
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";

        public string? EmergencyContact { get; set; }

        public string? RoomNumber { get; set; }

        public string? BedNumber { get; set; }
    }
}
