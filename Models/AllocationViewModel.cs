namespace AccommodationManagement.Models
{
    // ViewModels/AllocationViewModel.cs
    public class AllocationViewModel
    {
        public string? UserId { get; set; }
        public int RoomId { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public IEnumerable<Room> AvailableRooms { get; set; } = new List<Room>();
    }
}
