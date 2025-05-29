// Controllers/PanierController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using LarmoireWeb.Data.Services;
using LarmoireWeb.Models;

namespace LarmoireWeb.Controllers
{
    using LarmoireWeb.Data.Services; // pour IProduitService

    [Authorize]
    public class PanierController : Controller
    {
        private readonly IPanierService _srv;
        private readonly IProduitService _pSrv;     // ← nouveau
        private readonly UserManager<ApplicationUser> _userMgr;

        public PanierController(
            IPanierService srv,
            IProduitService pSrv,               // ← injection
            UserManager<ApplicationUser> um)
        {
            _srv = srv;
            _pSrv = pSrv;
            _userMgr = um;
        }

        // GET: affiche le panier
        public async Task<IActionResult> Index()
        {
            // Affiche un message d’erreur si présent
            ViewBag.CartError = TempData["CartError"];
            var u = await _userMgr.GetUserAsync(User);
            var cart = await _srv.GetByUserAsync(u.Id);
            return View(cart);
        }

        // POST: ajoute ou met à jour
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int produitId, int quantite)
        {
            var product = await _pSrv.GetByIdAsync(produitId);
            if (product == null)
                return NotFound();

            if (quantite > product.Stock)
            {
                TempData["CartError"] =
                    $"Impossible d’ajouter {quantite} exemplaires, seul(s) {product.Stock} disponible(s).";
                return RedirectToAction(nameof(Index));
            }

            var u = await _userMgr.GetUserAsync(User);
            await _srv.AddOrUpdateAsync(u.Id, produitId, quantite);
            return RedirectToAction(nameof(Index));
        }

        // POST: supprime un item du panier
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            await _srv.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }


}
