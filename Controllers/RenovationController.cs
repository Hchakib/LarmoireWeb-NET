using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using LarmoireWeb.Data.Services;
using LarmoireWeb.Models;
using LarmoireWeb.ViewModels;

namespace LarmoireWeb.Controllers
{
    [Authorize]
    public class RenovationController : Controller
    {
        private readonly IRenovationService _srv;
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly IWebHostEnvironment _env;

        public RenovationController(
            IRenovationService srv,
            UserManager<ApplicationUser> um,
            IWebHostEnvironment env)
        {
            _srv = srv;
            _userMgr = um;
            _env = env;
        }

        private async Task<bool> IsAdmin() =>
            (await _userMgr.GetUserAsync(User))?.Role == "Admin";

        //── USER : créer la demande
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string description, List<IFormFile> photos)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                ModelState.AddModelError("", "Description requise");
                return View();
            }

            var u = await _userMgr.GetUserAsync(User);
            var imageUrls = new List<string>();

            if (photos?.Any() == true)
            {
                var uploadsPath = Path.Combine(
                    _env.WebRootPath, "uploads", "renovations", u.Id);
                Directory.CreateDirectory(uploadsPath);

                foreach (var photo in photos)
                {
                    if (photo.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                        var filePath = Path.Combine(uploadsPath, fileName);
                        using var stream = new FileStream(filePath, FileMode.Create);
                        await photo.CopyToAsync(stream);
                        imageUrls.Add($"/uploads/renovations/{u.Id}/{fileName}");
                    }
                }
            }

            await _srv.SubmitAsync(new RenovationRequest
            {
                UserId = u.Id,
                Description = description,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ImageUrls = string.Join(";", imageUrls)
            });

            TempData["SuccessMessage"] =
                "Merci pour votre demande de rénovation, nous vous appellerons très bientôt !";

            return RedirectToAction(nameof(MyRequests));
        }

        //── USER : lister ses demandes
        public async Task<IActionResult> MyRequests()
        {
            var u = await _userMgr.GetUserAsync(User);
            return View(await _srv.GetByUserAsync(u.Id));
        }

        [Authorize]
        // ADMIN : voir et filtrer les demandes
        public async Task<IActionResult> Index(string searchTerm)
        {
            if (!await IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            // Chargement
            var all = await _srv.GetAllAsync();
            var q = all.AsQueryable();

            // Filtrage sur nom / prénom / téléphone
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                q = q.Where(r =>
                    r.User.Prenom.ToLower().Contains(term) ||
                    r.User.Nom.ToLower().Contains(term) ||
                    (r.User.PhoneNumber ?? "").Contains(term));
            }

            var vm = new RenovationIndexVm
            {
                SearchTerm = searchTerm,
                Requests = q.OrderByDescending(r => r.CreatedAt).ToList()
            };
            return View(vm);
         }

        // GET: /Renovation/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userMgr.GetUserAsync(User);
            var req = await _srv.GetByIdAsync(id);

            if (req == null)
                return View("NotFound");

            // Seul l’admin ou l’auteur de la demande y accèdent
            if (req.UserId != user.Id && !await IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            return View(req);
        }



        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            if (!await IsAdmin())
                return RedirectToAction("AccessDenied", "Home");
            await _srv.UpdateStatusAsync(id, RequestStatus.Accepted);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)  // ← nouveau
        {
            if (!await IsAdmin())
                return RedirectToAction("AccessDenied", "Home");
            await _srv.UpdateStatusAsync(id, RequestStatus.Rejected);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            if (!await IsAdmin())
                return RedirectToAction("AccessDenied", "Home");
            await _srv.UpdateStatusAsync(id, RequestStatus.Completed);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            if (!await IsAdmin())
                return RedirectToAction("AccessDenied", "Home");


            var req = await _srv.GetByIdAsync(id);
            if (req == null)
                return NotFound();

            await _srv.DeleteAsync(id);
            return RedirectToAction(nameof(Index));   // toujours vers la liste admin
        }


    }
}
