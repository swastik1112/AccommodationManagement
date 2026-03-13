namespace AccommodationManagement.Models
{
    // ViewModels/TransferViewModel.cs
    public class TransferViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? CurrentRoom { get; set; }
        public int NewRoomId { get; set; }

        public IEnumerable<Room> AvailableRooms { get; set; } = new List<Room>();
    }
}
