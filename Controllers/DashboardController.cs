using AccommodationManagement.Models;
using AccommodationManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccommodationManagement.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor – this is required to inject dependencies
        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Admin");

            if (User.IsInRole("Warden"))
                return RedirectToAction("Warden");

            if (User.IsInRole("User"))
                return RedirectToAction("UserDashboard");

            // fallback
            return View("Index"); // or error
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var model = new AdminDashboardViewModel
            {
                TotalRooms = await _context.Rooms.CountAsync(),
                TotalBeds = await _context.Beds.CountAsync(),
                OccupiedBeds = await _context.Beds.CountAsync(b => b.IsOccupied),
                VacantBeds = await _context.Beds.CountAsync(b => !b.IsOccupied),
                TotalUsers = await _context.Users.CountAsync(),
                TotalWardens = (await _userManager.GetUsersInRoleAsync("Warden")).Count,
                TotalGirls = (await _userManager.GetUsersInRoleAsync("User")).Count,
                PendingComplaints = await _context.Complaints.CountAsync(c => c.Status == "Pending"),
                RecentAllocations = await _context.RoomAllocations
                    .Include(a => a.User).Include(a => a.Room).Include(a => a.Bed)
                    .OrderByDescending(a => a.AllocationDate)
                    .Take(5).ToListAsync()
            };

            return View(model);
        }

        // ────────────────────────────────────────────────
        // WARDEN DASHBOARD
        // ────────────────────────────────────────────────
        [Authorize(Roles = "Warden")]
        public async Task<IActionResult> Warden()
        {
            var model = new WardenDashboardViewModel
            {
                TotalRooms = await _context.Rooms.CountAsync(),
                OccupiedBeds = await _context.Beds.CountAsync(b => b.IsOccupied),
                VacantBeds = await _context.Beds.CountAsync(b => !b.IsOccupied),
                PendingComplaints = await _context.Complaints.CountAsync(c => c.Status == "Pending"),
                // RoomsAlmostFull = await _context.Rooms
                //     .Where(r => r.VacantBeds <= 1 && r.TotalBeds > 0)
                //     .ToListAsync()
            };

            return View(model);
        }

        // ────────────────────────────────────────────────
        // USER DASHBOARD (Girl Employee)
        // ────────────────────────────────────────────────
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UserDashboard()   // ← renamed from User() to avoid confusion
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return RedirectToAction("Index", "Account"); // or error

            var allocation = await _context.RoomAllocations
                .Include(a => a.Bed)
                .FirstOrDefaultAsync(a => a.UserId == currentUser.Id);

            var model = new UserDashboardViewModel
            {
                FullName = currentUser.FullName,
                HasRoom = currentUser.RoomId != null,
                RoomNumber = currentUser.Room?.RoomNumber,
                BedNumber = allocation?.Bed?.BedNumber,
                PendingComplaintsCount = await _context.Complaints
                    .CountAsync(c => c.UserId == currentUser.Id && c.Status == "Pending")
            };

            return View(model);
        }

        private async Task<string?> GetUserBedNumber(ApplicationUser? user)
        {
            if (user?.RoomId == null) return null;
            var allocation = await _context.RoomAllocations
                .Include(a => a.Bed)
                .FirstOrDefaultAsync(a => a.UserId == user.Id);
            return allocation?.Bed?.BedNumber;
        }
    }
}
