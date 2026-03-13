using AccommodationManagement.Models;
using AccommodationManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccommodationManagement.Controllers
{
    [Authorize(Roles = "User")]
    public class MyRoomController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyRoomController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.RoomId == null)
            {
                // No room assigned
                return View((Room)null);
            }

            var room = await _context.Rooms
                .Include(r => r.Beds)
                    .ThenInclude(b => b.Allocation!)
                        .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(r => r.Id == user.RoomId);

            if (room == null)
            {
                return View((Room)null);
            }

            // ──────────────────────────────
            // Find THIS user's bed information
            // ──────────────────────────────
            var myAllocation = room.Beds
                .SelectMany(b => b.Allocation != null ? new[] { b.Allocation } : Array.Empty<RoomAllocation>())
                .FirstOrDefault(a => a.UserId == user.Id);

            if (myAllocation != null)
            {
                ViewBag.BedNumber = myAllocation.Bed?.BedNumber ?? "Not found";
                ViewBag.AllocationDate = myAllocation.AllocationDate;
            }
            else
            {
                ViewBag.BedNumber = "Not assigned";
                ViewBag.AllocationDate = null;
            }

            return View(room);
        }
    }
}
