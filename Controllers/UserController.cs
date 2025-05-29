// Controllers/UserController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LarmoireWeb.Models;
using LarmoireWeb.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LarmoireWeb.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly SignInManager<ApplicationUser> _signInMgr;

        public UserController(
            UserManager<ApplicationUser> userMgr,
            SignInManager<ApplicationUser> signInMgr)
        {
            _userMgr = userMgr;
            _signInMgr = signInMgr;
        }

        private async Task<bool> IsAdminAsync() =>
            (await _userMgr.GetUserAsync(User))?.Role == "Admin";

        // ─── Profil personnel ─────────────────────

        // GET: /User/Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userMgr.GetUserAsync(User);
            return View(user);
        }

        // GET: /User/EditProfile
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userMgr.GetUserAsync(User);
            ViewBag.IsAdmin = await IsAdminAsync();    // ← indique à la vue si on est admin
            return View(user);
        }


        // POST: /User/EditProfile
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(
       [Bind("Prenom,Nom,Email,Adresse,PhoneNumber,Role")] ApplicationUser vm)
        {
            var user = await _userMgr.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Champs que tout le monde peut modifier
            user.Prenom = vm.Prenom;
            user.Nom = vm.Nom;
            user.Email = vm.Email;
            user.UserName = vm.Email;
            user.Adresse = vm.Adresse;
            user.PhoneNumber = vm.PhoneNumber;

            // Champ Role **seulement** si admin
            if (await IsAdminAsync())
            {
                user.Role = vm.Role;
            }

            var result = await _userMgr.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                ViewBag.IsAdmin = await IsAdminAsync();
                return View(user);
            }

            await _signInMgr.RefreshSignInAsync(user);
            return RedirectToAction(nameof(Profile));
        }


        // ─── Gestion Admin ───────────────────────

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            if (!await IsAdminAsync())
                return RedirectToAction(nameof(Profile));

            var all = _userMgr.Users; // IQueryable<ApplicationUser>
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                all = all.Where(u =>
                    u.Prenom.ToLower().Contains(term) ||
                    u.Nom.ToLower().Contains(term) ||
                    (u.PhoneNumber ?? "").Contains(term));
            }

            var vm = new UserIndexVm
            {
                SearchTerm = searchTerm,
                Users = await all.ToListAsync()
            };
            return View(vm);
        }

        // GET: /User/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            if (!await IsAdminAsync())
                return RedirectToAction(nameof(Profile));

            var u = await _userMgr.FindByIdAsync(id);
            if (u == null)
                return View("NotFound");

            return View(u);
        }

        // GET: /User/EditUser/{id}
        [Authorize]
        public async Task<IActionResult> EditUser(string id)
        {
            if (!await IsAdminAsync())
                return RedirectToAction(nameof(Profile));

            var user = await _userMgr.FindByIdAsync(id);
            if (user == null)
                return View("NotFound");

            ViewBag.IsAdmin = true;  // pour afficher le champ Role
            return View(user);       // renvoie vers EditUser.cshtml
        }

        // POST: /User/EditUser/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(
            string id,
            [Bind("Prenom,Nom,Email,Adresse,PhoneNumber,Role")] ApplicationUser vm)
        {
            if (!await IsAdminAsync())
                return RedirectToAction(nameof(Profile));

            var user = await _userMgr.FindByIdAsync(id);
            if (user == null)
                return View("NotFound");

            // Met à jour les champs autorisés
            user.Prenom = vm.Prenom;
            user.Nom = vm.Nom;
            user.Email = vm.Email;
            user.UserName = vm.Email;
            user.Adresse = vm.Adresse;
            user.PhoneNumber = vm.PhoneNumber;
            user.Role = vm.Role;

            var result = await _userMgr.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                ViewBag.IsAdmin = true;
                return View(user);
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: /User/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            if (!await IsAdminAsync())
                return RedirectToAction(nameof(Profile));

            var u = await _userMgr.FindByIdAsync(id);
            if (u == null)
                return View("NotFound");

            return View(u);
        }

        // POST: /User/Delete/{id}
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!await IsAdminAsync())
                return RedirectToAction(nameof(Profile));

            var u = await _userMgr.FindByIdAsync(id);
            if (u != null)
                await _userMgr.DeleteAsync(u);

            return RedirectToAction(nameof(Index));
        }
    }
}
