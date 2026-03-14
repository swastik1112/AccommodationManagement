// Models/Notice.cs
using System.ComponentModel.DataAnnotations;

namespace AccommodationManagement.Models
{
    public class Notice
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Message / Description")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Posted By")]
        public string PostedBy { get; set; } = string.Empty; // FullName of Admin/Warden

        [Display(Name = "Posted On")]
        public DateTime PostedDate { get; set; } = DateTime.Now;

        [Display(Name = "Important / Urgent")]
        public bool IsImportant { get; set; } = false;

        // Optional: expiry date if you want notices to auto-hide later
        public DateTime? ExpiryDate { get; set; }
    }
}