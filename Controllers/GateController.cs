using AccommodationManagement.Data;
using AccommodationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccommodationManagement.Controllers
{
    public class GateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GateController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Scan(string email)
        {
            var user = await _context.Users
                        .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return NotFound();

            var lastLog = await _context.GateEntryLogs
                        .Where(x => x.UserId == user.Id)
                        .OrderByDescending(x => x.ScanTime)
                        .FirstOrDefaultAsync();

            string action = "Entry";

            if (lastLog != null && lastLog.Action == "Entry")
                action = "Exit";

            var log = new GateEntryLog
            {
                UserId = user.Id,
                Action = action,
                ScanTime = DateTime.Now
            };

            _context.GateEntryLogs.Add(log);
            await _context.SaveChangesAsync();

            ViewBag.Action = action;

            return View(user);
        }

        [Authorize(Roles = "Admin,Warden")]
        public IActionResult Logs()
        {
            var logs = _context.GateEntryLogs
                .Include(x => x.User)
                .OrderByDescending(x => x.ScanTime)
                .ToList();

            return View(logs);
        }

        [Authorize(Roles = "Admin,Warden")]
        public IActionResult Scanner()
        {
            return View();
        }
    }
}
