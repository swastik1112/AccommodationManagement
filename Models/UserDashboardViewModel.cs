namespace AccommodationManagement.Models
{
    public class UserDashboardViewModel
    {
        public string FullName { get; set; } = "";
        public bool HasRoom { get; set; }
        public string? RoomNumber { get; set; }
        public string? BedNumber { get; set; }
        public int PendingComplaintsCount { get; set; }
    }
}
