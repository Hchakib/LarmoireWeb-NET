using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LarmoireWeb.Data;
using LarmoireWeb.Models;

namespace LarmoireWeb.Data.Services
{
    public class ProduitService : IProduitService
    {
        private readonly AppDbContext _ctx;
        public ProduitService(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Produit>> GetAllAsync(int? categorieId = null)
        {
            var query = _ctx.Produits.Include(p => p.Categorie).AsQueryable();
            if (categorieId.HasValue)
                query = query.Where(p => p.CategorieId == categorieId.Value);
            return await query.OrderBy(p => p.Nom).ToListAsync();
        }

        public async Task<Produit> GetByIdAsync(int id) =>
            await _ctx.Produits.Include(p => p.Categorie)
                               .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<bool> AddNewAsync(Produit produit)
        {
            if (await _ctx.Produits.AnyAsync(p => p.Nom == produit.Nom))
                return false;
            _ctx.Produits.Add(produit);
            await _ctx.SaveChangesAsync();
            return true;
        }

        // Data/Services/ProduitService.cs
        public async Task<Produit> UpdateAsync(int id, Produit produit)
        {
            // 1) Récupère l'entité existante (trackée)
            var existing = await _ctx.Produits.FindAsync(id);
            if (existing == null)
            {
                return null; // ou throw new KeyNotFoundException(...)
            }

            // 2) Met à jour uniquement les champs modifiables
            existing.Nom = produit.Nom;
            existing.Description = produit.Description;
            existing.Prix = produit.Prix;
            existing.CategorieId = produit.CategorieId;
            existing.Stock = produit.Stock;
            existing.ImageUrl = produit.ImageUrl;

            // 3) Enregistre
            await _ctx.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteAsync(int id)
        {
            var p = await _ctx.Produits.FindAsync(id);
            if (p != null)
            {
                _ctx.Produits.Remove(p);
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
