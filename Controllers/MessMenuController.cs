using AccommodationManagement.Data;
using AccommodationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccommodationManagement.Controllers
{
    [Authorize(Roles = "Warden")]
    public class MessMenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessMenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(MessMenu menu)
        {
            if (ModelState.IsValid)
            {
                menu.AddedBy = User.Identity.Name ?? "Warden";
                _context.MessMenus.Add(menu);
                _context.SaveChanges();

                TempData["Success"] = "Menu added successfully";
                return RedirectToAction("Index");
            }

            return View(menu);
        }

        public IActionResult Index()
        {
            var menus = _context.MessMenus
                .OrderByDescending(x => x.MenuDate)
                .ToList();

            return View(menus);
        }

        // GET: Edit Menu
        public IActionResult Edit(int id)
        {
            var menu = _context.MessMenus.Find(id);

            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }


        // POST: Edit Menu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MessMenu model)
        {
            if (ModelState.IsValid)
            {
                _context.MessMenus.Update(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Delete Menu
        public IActionResult Delete(int id)
        {
            var menu = _context.MessMenus.Find(id);

            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }


        // POST: Delete Menu
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var menu = _context.MessMenus.Find(id);

            if (menu != null)
            {
                _context.MessMenus.Remove(menu);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}

