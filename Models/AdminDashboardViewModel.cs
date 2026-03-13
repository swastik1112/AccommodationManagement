namespace AccommodationManagement.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int TotalBeds { get; set; }
        public int OccupiedBeds { get; set; }
        public int VacantBeds { get; set; }
        public int TotalUsers { get; set; }
        public int TotalWardens { get; set; }
        public int TotalGirls { get; set; }
        public int PendingComplaints { get; set; }
        public List<RoomAllocation> RecentAllocations { get; set; } = new();
    }
}
