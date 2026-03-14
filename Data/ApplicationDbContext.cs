using AccommodationManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccommodationManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<RoomAllocation> RoomAllocations { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        // Data/ApplicationDbContext.cs
        public DbSet<Notice> Notices { get; set; }

        public DbSet<EmergencyRequest> EmergencyRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Room → RoomAllocation : NO CASCADE (use Restrict or NoAction)
            builder.Entity<RoomAllocation>()
                .HasOne(a => a.Room)
                .WithMany(r => r.Allocations)               // navigation property in Room model
                .HasForeignKey(a => a.RoomId)
                .OnDelete(DeleteBehavior.Restrict);         // ← this is the important line

            // Bed → RoomAllocation : KEEP CASCADE (logical: bed deleted → allocation gone)
            builder.Entity<RoomAllocation>()
                .HasOne(a => a.Bed)
                .WithOne(b => b.Allocation)                 // one-to-one
                .HasForeignKey<RoomAllocation>(a => a.BedId)
                .OnDelete(DeleteBehavior.Cascade);

            // User → RoomAllocation : usually CASCADE is fine
            builder.Entity<RoomAllocation>()
                .HasOne(a => a.User)
                .WithMany()                                 // or .WithOne() if you have navigation
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: ApplicationUser → Room (usually SetNull or Restrict)
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Room)
                .WithMany()
                .HasForeignKey(u => u.RoomId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}