// Controllers/NoticeController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccommodationManagement.Data;
using AccommodationManagement.Models;

[Authorize(Roles = "Admin,Warden")]
public class NoticeController : Controller
{
    private readonly ApplicationDbContext _context;

    public NoticeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: List all notices
    public async Task<IActionResult> Index()
    {
        var notices = await _context.Notices
            .OrderByDescending(n => n.PostedDate)
            .ToListAsync();

        return View(notices);
    }

    // GET: Create new notice
    public IActionResult Create()
    {
        return View();
    }

    // POST: Save new notice
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Notice notice)
    {
        if (ModelState.IsValid)
        {
            notice.PostedBy = User.Identity?.Name ?? "System";
            notice.PostedDate = DateTime.Now;

            _context.Add(notice);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Notice posted successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(notice);
    }

    // GET: Edit notice
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var notice = await _context.Notices.FindAsync(id);
        if (notice == null) return NotFound();

        return View(notice);
    }

    // POST: Update notice
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Notice notice)
    {
        if (id != notice.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(notice);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Notice updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoticeExists(notice.Id)) return NotFound();
                throw;
            }
        }

        return View(notice);
    }

    // POST: Delete notice
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var notice = await _context.Notices.FindAsync(id);
        if (notice != null)
        {
            _context.Notices.Remove(notice);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Notice deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool NoticeExists(int id)
    {
        return _context.Notices.Any(e => e.Id == id);
    }
}