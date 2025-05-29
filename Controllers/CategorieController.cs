// Controllers/CategorieController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using LarmoireWeb.Data.Services;
using LarmoireWeb.Models;

namespace LarmoireWeb.Controllers
{
    public class CategorieController : Controller
    {
        private readonly ICategorieService _srv;
        private readonly UserManager<ApplicationUser> _userMgr;

        public CategorieController(ICategorieService srv, UserManager<ApplicationUser> um)
        {
            _srv = srv;
            _userMgr = um;
        }

        private async Task<bool> IsAdmin() =>
            (await _userMgr.GetUserAsync(User))?.Role == "Admin";

        // PUBLIC
        public async Task<IActionResult> Index()
            => View(await _srv.GetAllAsync());

        public async Task<IActionResult> Details(int id)
        {
            var c = await _srv.GetByIdAsync(id);
            if (c == null) return View("NotFound");
            return View(c);
        }

        // ADMIN CRUD
        public async Task<IActionResult> Create()
        {
            if (!await IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom")] Categorie c)
        {
            if (!await IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (!ModelState.IsValid) return View(c);
            if (!await _srv.AddNewAsync(c))
            {
                ModelState.AddModelError("Nom", "Cette catégorie existe déjà.");
                return View(c);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!await IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            var c = await _srv.GetByIdAsync(id);
            if (c == null) return View("NotFound");
            return View(c);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom")] Categorie c)
        {
            if (!await IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id != c.Id || !ModelState.IsValid) return View(c);
            await _srv.UpdateAsync(id, c);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!await IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            var c = await _srv.GetByIdAsync(id);
            if (c == null) return View("NotFound");
            return View(c);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            await _srv.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
