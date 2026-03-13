using AccommodationManagement.Models;

namespace AccommodationManagement.Models
{
    public class RoomAllocation
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public int BedId { get; set; }
        public DateTime AllocationDate { get; set; } = DateTime.Now;

        public ApplicationUser User { get; set; } = null!;
        public Room Room { get; set; } = null!;
        public Bed Bed { get; set; } = null!;
    }
}