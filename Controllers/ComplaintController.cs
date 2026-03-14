using AccommodationManagement.Models;
using AccommodationManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccommodationManagement.Controllers
{
    [Authorize]
    public class ComplaintController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ComplaintController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // User: see own complaints
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyComplaints()
        {
            var userId = _userManager.GetUserId(User);
            var complaints = await _context.Complaints
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            return View(complaints);
        }

        [Authorize(Roles = "User")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Complaint complaint)
        {
            complaint.UserId = _userManager.GetUserId(User)!;
            complaint.Date = DateTime.Now;
            complaint.Status = "Pending";

            _context.Add(complaint);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyComplaints));
        }

        // GET: Admin - List all complaints
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> Index(string statusFilter = "All", string search = "")
        {
            var query = _context.Complaints
                .Include(c => c.User)
                .AsQueryable();

            if (statusFilter != "All")
                query = query.Where(c => c.Status == statusFilter);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(c =>
                    c.Title.ToLower().Contains(search) ||
                    (c.User != null && (c.User.FullName.ToLower().Contains(search) || c.User.Email.ToLower().Contains(search))));
            }

            var complaints = await query
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            ViewBag.StatusFilter = statusFilter;
            ViewBag.Search = search;

            return View(complaints);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                return Json(new { success = false, message = "Status is required." });

            var validStatuses = new[] { "Pending", "InProgress", "Resolved" };
            if (!validStatuses.Contains(newStatus))
                return Json(new { success = false, message = "Invalid status value." });

            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
                return Json(new { success = false, message = "Complaint not found." });

            complaint.Status = newStatus;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Status updated to {newStatus}." });
        }

        // GET: View single complaint details
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var complaint = await _context.Complaints
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null) return NotFound();

            // Optional: Only allow user to view their own complaint
            if (complaint.UserId != _userManager.GetUserId(User))
                return Forbid();

            return View(complaint);
        }
    }
}
