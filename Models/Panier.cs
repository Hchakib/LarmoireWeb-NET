// Panier.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LarmoireWeb.Models
{
    public class Panier
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "L'identifiant de l'utilisateur est requis.")]
        public string UtilisateurId { get; set; }

        [Required(ErrorMessage = "L'identifiant du produit est requis.")]
        public int ProduitId { get; set; }

        [Required(ErrorMessage = "La quantité est requise.")]
        [Range(1, 100, ErrorMessage = "La quantité doit être comprise entre 1 et 100.")]
        public int Quantite { get; set; }

        // Navigation properties
        [ForeignKey("UtilisateurId")]
        public ApplicationUser Utilisateur { get; set; }

        [ForeignKey("ProduitId")]
        public Produit Produit { get; set; }
    }
}