using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LarmoireWeb.Models
{
    public class Produit
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du produit est requis.")]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required(ErrorMessage = "La description est requise.")]
        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Le prix est requis.")]
        [Range(0.01, 10000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Prix { get; set; }

        [Required(ErrorMessage = "La catégorie est requise.")]
        public int CategorieId { get; set; }

        [ValidateNever]
        public Categorie? Categorie { get; set; }

        [Required(ErrorMessage = "La quantité en stock est requise.")]
        [Range(0, int.MaxValue, ErrorMessage = "Le stock ne peut pas être négatif.")]
        public int Stock { get; set; }

        /// <summary>
        /// Statut calculé, pas mappé en base.
        /// </summary>
        [NotMapped]
        public string Statut => Stock > 0 ? "Disponible" : "Rupture de stock";

        [ValidateNever]
        public List<CommandeProduit>? CommandeProduits { get; set; } = new();

        [StringLength(200)]
        public string? ImageUrl { get; set; }
    }
}
