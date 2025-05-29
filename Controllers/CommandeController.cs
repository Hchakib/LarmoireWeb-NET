using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LarmoireWeb.Data.Services;
using LarmoireWeb.Models;
using LarmoireWeb.ViewModels;

namespace LarmoireWeb.Controllers
{
    [Authorize]
    public class CommandeController : Controller
    {
        private readonly ICommandeService _srv;
        private readonly UserManager<ApplicationUser> _userMgr;

        public CommandeController(ICommandeService srv, UserManager<ApplicationUser> um)
        {
            _srv = srv;
            _userMgr = um;
        }

        private async Task<bool> IsAdmin()
            => (await _userMgr.GetUserAsync(User))?.Role == "Admin";

        // POST : formulaire non-PayPal
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var u = await _userMgr.GetUserAsync(User);
            if (u.Role == "Admin")
                return RedirectToAction("AccessDenied", "Home");

            await _srv.PlaceOrderAsync(u.Id);
            return RedirectToAction(nameof(MyOrders));
        }

        // GET : finalisation PayPal
        [HttpGet]
        public async Task<IActionResult> CompleteOrder()
        {
            var u = await _userMgr.GetUserAsync(User);
            if (u.Role == "Admin")
                return RedirectToAction("AccessDenied", "Home");

            await _srv.PlaceOrderAsync(u.Id);
            return RedirectToAction(nameof(MyOrders));
        }

        // USER : ses commandes
        public async Task<IActionResult> MyOrders()
        {
            var u = await _userMgr.GetUserAsync(User);
            var orders = await _srv.GetByUserAsync(u.Id);
            return View(orders);            // renvoie vers MyOrders.cshtml
        }



        // ADMIN : liste et recherche des commandes
        public async Task<IActionResult> Index(string searchTerm)
        {
            if (!await IsAdmin())
                return RedirectToAction(nameof(MyOrders));

            // Récupération de toutes les commandes
            var all = await _srv.GetAllAsync();
            var q = all.AsQueryable();

            // Filtre sur prénom, nom, téléphone
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                q = q.Where(c =>
                    c.Utilisateur.Prenom.ToLower().Contains(term) ||
                    c.Utilisateur.Nom.ToLower().Contains(term) ||
                    (c.Utilisateur.PhoneNumber ?? "").Contains(term)
                );
            }

            var vm = new CommandeIndexVm
            {
                SearchTerm = searchTerm,
                Orders = q.OrderByDescending(c => c.Date).ToList()
            };
            return View(vm);
        }

        // Détails d’une commande (propriétaire ou admin)
        public async Task<IActionResult> Details(int id)
        {
            var u = await _userMgr.GetUserAsync(User);
            var ord = await _srv.GetByIdAsync(id);
            if (ord == null ||
               (ord.UtilisateurId != u.Id && !await IsAdmin()))
                return RedirectToAction(nameof(MyOrders));
            return View(ord);
        }
    }
}
