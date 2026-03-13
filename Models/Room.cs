using AccommodationManagement.Models;

namespace AccommodationManagement.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int TotalBeds { get; set; }
        public int OccupiedBeds { get; set; }

        public ICollection<Bed> Beds { get; set; } = new List<Bed>();
        public ICollection<RoomAllocation> Allocations { get; set; } = new List<RoomAllocation>();

        // Calculated
        public int VacantBeds => TotalBeds - OccupiedBeds;
    }
}