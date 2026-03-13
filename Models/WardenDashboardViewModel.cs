namespace AccommodationManagement.Models
{
    public class WardenDashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int OccupiedBeds { get; set; }
        public int VacantBeds { get; set; }
        public int PendingComplaints { get; set; }
        public List<Room> RoomsAlmostFull { get; set; } = new();
    }
}
