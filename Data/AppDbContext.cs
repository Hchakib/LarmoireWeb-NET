// Data/AppDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LarmoireWeb.Models;

namespace LarmoireWeb.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produit> Produits { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Panier> Paniers { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<CommandeProduit> CommandeProduits { get; set; }

        // Exposed for scaffolding EF si nécessaire
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<RenovationRequest> RenovationRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Panier ↔ Produit
            builder.Entity<Panier>()
                   .HasOne(p => p.Produit)
                   .WithMany()
                   .HasForeignKey(p => p.ProduitId);

            // Panier ↔ Utilisateur
            builder.Entity<Panier>()
                   .HasOne(p => p.Utilisateur)
                   .WithMany(u => u.Paniers)
                   .HasForeignKey(p => p.UtilisateurId);

            // Commande ↔ Utilisateur
            builder.Entity<Commande>()
                   .HasOne(c => c.Utilisateur)
                   .WithMany(u => u.Commandes)
                   .HasForeignKey(c => c.UtilisateurId);

            // CommandeProduit ↔ Commande
            builder.Entity<CommandeProduit>()
                   .HasOne(cp => cp.Commande)
                   .WithMany(c => c.CommandeProduits)
                   .HasForeignKey(cp => cp.CommandeId);

            // CommandeProduit ↔ Produit
            builder.Entity<CommandeProduit>()
                   .HasOne(cp => cp.Produit)
                   .WithMany(p => p.CommandeProduits)
                   .HasForeignKey(cp => cp.ProduitId);

            // RenovationRequest ↔ Utilisateur
            builder.Entity<RenovationRequest>()
                   .HasOne(r => r.User)
                   .WithMany(u => u.RenovationRequests)
                   .HasForeignKey(r => r.UserId);
        }
    }
}
