using AccommodationManagement.Data;
using AccommodationManagement.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccommodationManagement.Controllers
{
    [Authorize(Roles = "Admin,Warden")]
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms.Include(r => r.Beds).ToListAsync();
            return View(rooms);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                // Auto-create beds
                for (int i = 1; i <= room.TotalBeds; i++)
                {
                    _context.Beds.Add(new Bed
                    {
                        RoomId = room.Id,
                        BedNumber = $"Bed-{i}",
                        IsOccupied = false
                    });
                }
                room.OccupiedBeds = 0;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }



        // ────────────────────────────────────────────────
        // DETAILS / VIEW BEDS
        // ────────────────────────────────────────────────
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Beds)
                    .ThenInclude(b => b.Allocation)
                        .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return NotFound();

            return View(room);
        }

        // ────────────────────────────────────────────────
        // EDIT ROOM
        // ────────────────────────────────────────────────
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Only allow changing room number and total beds
                    var existing = await _context.Rooms.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.RoomNumber = room.RoomNumber;

                    // If total beds changed, we can handle bed adjustment logic here (advanced)
                    // For simplicity: just update total (beds remain as-is)
                    if (room.TotalBeds != existing.TotalBeds)
                    {
                        // Optional: warn if reducing beds below occupied count
                        if (room.TotalBeds < existing.OccupiedBeds)
                        {
                            ModelState.AddModelError("TotalBeds", "Cannot reduce beds below current occupied count.");
                            return View(room);
                        }

                        existing.TotalBeds = room.TotalBeds;
                    }

                    _context.Update(existing);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Room updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Id)) return NotFound();
                    throw;
                }
            }

            return View(room);
        }

        // ────────────────────────────────────────────────
        // DELETE ROOM
        // ────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Beds)
                .Include(r => r.Allocations)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return Json(new { success = false, message = "Room not found." });

            if (room.Allocations.Any())
            {
                return Json(new { success = false, message = "Cannot delete room with active allocations. Reassign residents first." });
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Room {room.RoomNumber} deleted successfully." });
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
