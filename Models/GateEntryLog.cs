namespace AccommodationManagement.Models
{
    public class GateEntryLog
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";

        public ApplicationUser User { get; set; }

        public DateTime ScanTime { get; set; } = DateTime.Now;

        public string Action { get; set; } = ""; // Entry or Exit
    }
}
