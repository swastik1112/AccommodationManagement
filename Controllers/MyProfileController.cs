// Controllers/MyProfileController.cs
using AccommodationManagement.Data;
using AccommodationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "User")]
public class MyProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public MyProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // GET: View Profile
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        if (user.RoomId.HasValue)
        {
            user.Room = await _context.Rooms
                .Include(r => r.Beds)
                    .ThenInclude(b => b.Allocation!)
                        .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(r => r.Id == user.RoomId.Value);
        }

        // Fetch THIS user's bed number
        var myBed = await _context.RoomAllocations
            .Include(a => a.Bed)
            .Where(a => a.UserId == user.Id)
            .Select(a => a.Bed.BedNumber)
            .FirstOrDefaultAsync();

        ViewBag.MyBedNumber = myBed ?? "Not Assigned";

        return View(user);
    }

    // GET: Edit Profile
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var model = new EditProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            EmergencyContact = user.EmergencyContact,
            RoomNumber = user.Room?.RoomNumber,
            BedNumber = GetUserBedNumber(user),
            JoinDate = user.CreatedDate
        };

        return View(model);
    }

    // POST: Save changes
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.Phone = model.Phone;
        user.EmergencyContact = model.EmergencyContact;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }

    private string? GetUserBedNumber(ApplicationUser user)
    {
        if (user.RoomId == null) return null;

        var allocation = _context.RoomAllocations
            .Include(a => a.Bed)
            .FirstOrDefault(a => a.UserId == user.Id);

        return allocation?.Bed?.BedNumber;
    }
}