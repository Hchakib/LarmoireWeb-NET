// Data/Services/PanierService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LarmoireWeb.Data;
using LarmoireWeb.Models;

namespace LarmoireWeb.Data.Services
{
    public class PanierService : IPanierService
    {
        private readonly AppDbContext _ctx;
        public PanierService(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Panier>> GetByUserAsync(string userId) =>
            await _ctx.Paniers.Include(p => p.Produit)
                              .Where(p => p.UtilisateurId == userId)
                              .ToListAsync();

        public async Task AddOrUpdateAsync(string userId, int produitId, int quantite)
        {
            var item = await _ctx.Paniers
                                 .FirstOrDefaultAsync(p => p.UtilisateurId == userId
                                                        && p.ProduitId == produitId);
            if (item == null)
            {
                _ctx.Paniers.Add(new Panier
                {
                    UtilisateurId = userId,
                    ProduitId = produitId,
                    Quantite = quantite
                });
            }
            else
            {
                item.Quantite = quantite;
                _ctx.Paniers.Update(item);
            }
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _ctx.Paniers.FindAsync(id);
            if (item != null)
            {
                _ctx.Paniers.Remove(item);
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
