using AccommodationManagement.Data;
using AccommodationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccommodationManagement.Controllers
{
    [Authorize(Roles = "User")]
    public class IdCardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdCardController(ApplicationDbContext context,
                                UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var allocation = await _context.RoomAllocations
                .Include(x => x.Room)
                .Include(x => x.Bed)
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            var model = new DigitalIdViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                EmergencyContact = user.EmergencyContact,
                RoomNumber = allocation?.Room?.RoomNumber,
                BedNumber = allocation?.Bed?.BedNumber
            };

            return View(model);
        }
    }
}
