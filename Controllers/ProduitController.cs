using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using LarmoireWeb.Data.Services;
using LarmoireWeb.Models;

namespace LarmoireWeb.Controllers
{
    public class ProduitController : Controller
    {
        private readonly IProduitService _srv;
        private readonly ICategorieService _categorieService;
        private readonly UserManager<ApplicationUser> _userMgr;

        public ProduitController(
            IProduitService srv,
            ICategorieService categorieService,
            UserManager<ApplicationUser> um)
        {
            _srv = srv;
            _categorieService = categorieService;
            _userMgr = um;
        }

        private async Task<bool> IsAdmin() =>
            (await _userMgr.GetUserAsync(User))?.Role == "Admin";

        // PUBLIC : liste + filtrage
        public async Task<IActionResult> Index(int? categorieId)
        {
            // 1) Récupère les produits, éventuellement filtrés
            var produits = await _srv.GetAllAsync(categorieId);
            // 2) Récupère la liste des catégories pour le dropdown
            var categories = await _categorieService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Nom", categorieId);
            return View(produits);
        }

        public async Task<IActionResult> Details(int id)
        {
            var p = await _srv.GetByIdAsync(id);
            if (p == null) return View("NotFound");
            return View(p);
        }

        // ADMIN CRUD (inchangé...
        // ADMIN: GET: /Produit/Create
        public async Task<IActionResult> Create()
        {
            if (!await IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            // Récupère les catégories pour le dropdown
            var categories = await _categorieService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Nom");
            return View();
        }

        // POST: /Produit/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Nom,Description,Prix,CategorieId,Stock,ImageUrl")] Produit p)
        {
            if (!await IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            // repopule le dropdown
            var cats = await _categorieService.GetAllAsync();
            ViewBag.Categories = new SelectList(cats, "Id", "Nom", p.CategorieId);

            if (!ModelState.IsValid)
                return View(p);

            if (!await _srv.AddNewAsync(p))
            {
                ModelState.AddModelError("Nom", "Un produit avec ce nom existe déjà.");
                return View(p);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Produit/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!await IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            var p = await _srv.GetByIdAsync(id);
            if (p == null) return View("NotFound");

            var cats = await _categorieService.GetAllAsync();
            ViewBag.Categories = new SelectList(cats, "Id", "Nom", p.CategorieId);
            return View(p);
        }

        // POST: /Produit/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Nom,Description,Prix,CategorieId,Stock,ImageUrl")] Produit p)
        {
            if (!await IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            // repopule le dropdown
            var cats = await _categorieService.GetAllAsync();
            ViewBag.Categories = new SelectList(cats, "Id", "Nom", p.CategorieId);

            if (id != p.Id || !ModelState.IsValid)
                return View(p);

            await _srv.UpdateAsync(id, p);
            return RedirectToAction(nameof(Index));
        }


        // ADMIN: GET /Produit/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (!await IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var p = await _srv.GetByIdAsync(id);
            if (p == null) return View("NotFound");

            // p.Categorie est déjà chargé dans GetByIdAsync
            return View(p);
        }

        // ADMIN: POST /Produit/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            await _srv.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}