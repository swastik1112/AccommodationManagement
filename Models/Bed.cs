using AccommodationManagement.Models;

namespace AccommodationManagement.Models
{
    public class Bed
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string BedNumber { get; set; } = string.Empty;
        public bool IsOccupied { get; set; } = false;

        public Room Room { get; set; } = null!;
        public RoomAllocation? Allocation { get; set; }
    }
}