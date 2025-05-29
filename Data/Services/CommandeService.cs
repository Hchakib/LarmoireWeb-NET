using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LarmoireWeb.Data;
using LarmoireWeb.Models;

namespace LarmoireWeb.Data.Services
{
    public class CommandeService : ICommandeService
    {
        private readonly AppDbContext _ctx;
        public CommandeService(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Commande>> GetAllAsync() =>
            await _ctx.Commandes
                      .Include(c => c.CommandeProduits)
                        .ThenInclude(cp => cp.Produit)
                      .Include(c => c.Utilisateur)
                      .OrderByDescending(c => c.Date)
                      .ToListAsync();

        public async Task<IEnumerable<Commande>> GetByUserAsync(string userId) =>
            await _ctx.Commandes
                      .Where(c => c.UtilisateurId == userId)
                      .Include(c => c.CommandeProduits)
                        .ThenInclude(cp => cp.Produit)
                      .OrderByDescending(c => c.Date)
                      .ToListAsync();

        public async Task<Commande> GetByIdAsync(int id) =>
            await _ctx.Commandes
                      .Include(c => c.CommandeProduits)
                        .ThenInclude(cp => cp.Produit)
                      .Include(c => c.Utilisateur)
                      .FirstOrDefaultAsync(c => c.Id == id);

        public async Task PlaceOrderAsync(string userId)
        {
            var cartItems = await _ctx.Paniers
                                      .Include(p => p.Produit)
                                      .Where(p => p.UtilisateurId == userId)
                                      .ToListAsync();
            if (!cartItems.Any())
                return;

            var total = cartItems.Sum(p => p.Quantite * p.Produit.Prix);

            var order = new Commande
            {
                UtilisateurId = userId,
                Date = DateTime.UtcNow,
                Total = total,
                Statut = "En attente"
            };
            _ctx.Commandes.Add(order);
            await _ctx.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                _ctx.CommandeProduits.Add(new CommandeProduit
                {
                    CommandeId = order.Id,
                    ProduitId = item.ProduitId,
                    Quantite = item.Quantite,
                    PrixUnitaire = item.Produit.Prix
                });

                // ← Mise à jour du stock
                item.Produit.Stock -= item.Quantite;
                _ctx.Produits.Update(item.Produit);
            }

            _ctx.Paniers.RemoveRange(cartItems);
            await _ctx.SaveChangesAsync();
        }

    }
}
