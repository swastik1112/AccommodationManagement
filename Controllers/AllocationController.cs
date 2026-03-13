using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccommodationManagement.Data;
using AccommodationManagement.Models;

namespace AccommodationManagement.Controllers
{
    [Authorize(Roles = "Admin,Warden")]
    public class AllocationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AllocationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: List all current allocations
        public async Task<IActionResult> Index()
        {
            var allocations = await _context.RoomAllocations
                .Include(a => a.User)
                .Include(a => a.Room)
                .Include(a => a.Bed)
                .OrderByDescending(a => a.AllocationDate)
                .ToListAsync();

            return View(allocations);
        }

        // GET: Form to assign a user to a room/bed
        public async Task<IActionResult> Assign()
        {
            var model = new AllocationViewModel
            {
                Users = await _userManager.GetUsersInRoleAsync("User"),
                AvailableRooms = await _context.Rooms
                    .Where(r => r.TotalBeds > r.OccupiedBeds)  // ← fixed
                    .OrderBy(r => r.RoomNumber)
                    .ToListAsync()
            };

            return View(model);
        }

        // POST: Assign user to a room (auto-selects first vacant bed)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AllocationViewModel model)
        {
            if (model.UserId == null || model.RoomId == 0)
            {
                ModelState.AddModelError("", "Please select both a girl and a room.");
                await LoadDropdowns(model);
                return View(model);
            }

            var room = await _context.Rooms
                .Include(r => r.Beds)
                .FirstOrDefaultAsync(r => r.Id == model.RoomId);

            if (room == null || room.VacantBeds <= 0)
            {
                ModelState.AddModelError("", "Selected room is full or does not exist.");
                await LoadDropdowns(model);
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                await LoadDropdowns(model);
                return View(model);
            }

            // Check if user is already allocated somewhere
            var existing = await _context.RoomAllocations
                .FirstOrDefaultAsync(a => a.UserId == model.UserId);

            if (existing != null)
            {
                ModelState.AddModelError("", "This girl is already allocated to a room. Use Transfer instead.");
                await LoadDropdowns(model);
                return View(model);
            }

            // Find first vacant bed
            var bed = room.Beds.FirstOrDefault(b => !b.IsOccupied);
            if (bed == null)
            {
                ModelState.AddModelError("", "No vacant bed found in this room.");
                await LoadDropdowns(model);
                return View(model);
            }

            var allocation = new RoomAllocation
            {
                UserId = model.UserId,
                RoomId = model.RoomId,
                BedId = bed.Id,
                AllocationDate = DateTime.Now
            };

            bed.IsOccupied = true;
            room.OccupiedBeds++;

            user.RoomId = room.Id;

            _context.RoomAllocations.Add(allocation);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{user.FullName} has been assigned to Room {room.RoomNumber} - Bed {bed.BedNumber}";
            return RedirectToAction(nameof(Index));
        }

        // GET: Transfer form
        // In Transfer GET
        public async Task<IActionResult> Transfer(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentAllocation = await _context.RoomAllocations
                .Include(a => a.Room)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            var model = new TransferViewModel
            {
                UserId = userId,
                UserName = user.FullName,
                CurrentRoom = currentAllocation?.Room?.RoomNumber ?? "Not Assigned",
                AvailableRooms = await _context.Rooms
                    .Where(r => r.TotalBeds > r.OccupiedBeds)  // ← fixed here
                    .OrderBy(r => r.RoomNumber)
                    .ToListAsync()
            };

            return View(model);
        }

        // POST: Perform transfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(TransferViewModel model)
        {
            if (model.NewRoomId == 0)
            {
                ModelState.AddModelError("", "Please select a new room.");
                await LoadTransferDropdowns(model);
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var oldAllocation = await _context.RoomAllocations
                .Include(a => a.Bed)
                .FirstOrDefaultAsync(a => a.UserId == model.UserId);

            if (oldAllocation != null)
            {
                // Free old bed
                oldAllocation.Bed.IsOccupied = false;
                var oldRoom = await _context.Rooms.FindAsync(oldAllocation.RoomId);
                if (oldRoom != null) oldRoom.OccupiedBeds--;

                _context.RoomAllocations.Remove(oldAllocation);
            }

            var newRoom = await _context.Rooms
                .Include(r => r.Beds)
                .FirstOrDefaultAsync(r => r.Id == model.NewRoomId);

            if (newRoom == null || newRoom.VacantBeds <= 0)
            {
                ModelState.AddModelError("", "Selected room is full or does not exist.");
                await LoadTransferDropdowns(model);
                return View(model);
            }

            var newBed = newRoom.Beds.FirstOrDefault(b => !b.IsOccupied);
            if (newBed == null)
            {
                ModelState.AddModelError("", "No vacant bed in selected room.");
                await LoadTransferDropdowns(model);
                return View(model);
            }

            var newAllocation = new RoomAllocation
            {
                UserId = model.UserId,
                RoomId = model.NewRoomId,
                BedId = newBed.Id,
                AllocationDate = DateTime.Now
            };

            newBed.IsOccupied = true;
            newRoom.OccupiedBeds++;
            user.RoomId = model.NewRoomId;

            _context.RoomAllocations.Add(newAllocation);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{user.FullName} transferred to Room {newRoom.RoomNumber} - Bed {newBed.BedNumber}";
            return RedirectToAction(nameof(Index));
        }

        // POST: Remove / Unassign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(string userId)
        {
            var allocation = await _context.RoomAllocations
                .Include(a => a.Bed)
                .Include(a => a.Room)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (allocation == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                user.RoomId = null;

            allocation.Bed.IsOccupied = false;
            allocation.Room.OccupiedBeds--;

            _context.RoomAllocations.Remove(allocation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Allocation removed successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Helper methods
        private async Task LoadDropdowns(AllocationViewModel model)
        {
            model.Users = await _userManager.GetUsersInRoleAsync("User");
            model.AvailableRooms = await _context.Rooms
                .Where(r => r.VacantBeds > 0)
                .ToListAsync();
        }

        private async Task LoadTransferDropdowns(TransferViewModel model)
        {
            model.AvailableRooms = await _context.Rooms
                .Where(r => r.VacantBeds > 0)
                .ToListAsync();
        }
    }
}