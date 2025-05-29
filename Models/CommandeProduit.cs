// CommandeProduit.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LarmoireWeb.Models
{
    public class CommandeProduit
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "L'identifiant de la commande est requis.")]
        public int CommandeId { get; set; }

        [Required(ErrorMessage = "L'identifiant du produit est requis.")]
        public int ProduitId { get; set; }

        [Required(ErrorMessage = "La quantité est requise.")]
        [Range(1, 100, ErrorMessage = "La quantité doit être comprise entre 1 et 100.")]
        public int Quantite { get; set; }

        [Required(ErrorMessage = "Le prix unitaire est requis.")]
        [Range(0.01, 10000, ErrorMessage = "Le prix unitaire doit être compris entre 0,01 et 10 000.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrixUnitaire { get; set; }

        // Navigation properties
        [ForeignKey("CommandeId")]
        public Commande Commande { get; set; }

        [ForeignKey("ProduitId")]
        public Produit Produit { get; set; }
    }
}