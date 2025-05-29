// Commande.cs
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LarmoireWeb.Models
{
    public class Commande
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "L'identifiant de l'utilisateur est requis.")]
        public string UtilisateurId { get; set; }

        [Required(ErrorMessage = "La date de la commande est requise.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Le total est requis.")]
        [Range(0.01, 100000, ErrorMessage = "Le total doit être compris entre 0,01 et 100 000.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Le statut est requis.")]
        [StringLength(50, ErrorMessage = "Le statut ne peut pas dépasser 50 caractères.")]
        public string Statut { get; set; } // Ex. : En attente, Expédiée, Livrée

        // Navigation properties
        [ForeignKey("UtilisateurId")]
        [ValidateNever]
        public ApplicationUser Utilisateur { get; set; }

        [ValidateNever]
        public List<CommandeProduit> CommandeProduits { get; set; }
    }
}