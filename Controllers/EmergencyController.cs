using AccommodationManagement.Data;
using AccommodationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AccommodationManagement.Controllers
{
    public class EmergencyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmergencyController(ApplicationDbContext context,
                                   UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // USER presses SOS
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendSOS()
        {
            var user = await _userManager.GetUserAsync(User);

            var request = new EmergencyRequest
            {
                UserId = user.Id,
                UserName = user.FullName,
                RequestTime = DateTime.Now,
                Status = "Pending"
            };

            _context.EmergencyRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Emergency alert sent to Warden!";
            return RedirectToAction("Index", "Dashboard");
        }

        // ADMIN / WARDEN view emergency requests
        [Authorize(Roles = "Admin,Warden")]
        public IActionResult Requests()
        {
            var data = _context.EmergencyRequests
                        .OrderByDescending(x => x.RequestTime)
                        .ToList();

            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Warden")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.EmergencyRequests.FindAsync(id);

            if (request != null)
            {
                _context.EmergencyRequests.Remove(request);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Requests));
        }
    }
}
