using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LarmoireWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Le prénom est requis.")]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "Le nom de famille est requis.")]
        [StringLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères.")]
        public string Nom { get; set; }

        [StringLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères.")]
        public string Adresse { get; set; }

        [Required(ErrorMessage = "Le rôle est requis.")]
        [StringLength(20, ErrorMessage = "Le rôle ne peut pas dépasser 20 caractères.")]
        public string Role { get; set; } = "Client";

        // ← On redéclare la propriété PhoneNumber pour la rendre requise et valider le format
        [Required(ErrorMessage = "Le numéro de téléphone est requis.")]
        [Phone(ErrorMessage = "Le format du téléphone est invalide.")]
        public override string PhoneNumber { get; set; }

        // Navigation properties
        public List<Panier> Paniers { get; set; }
        public List<Commande> Commandes { get; set; }
        public List<RenovationRequest> RenovationRequests { get; set; }
    }
}
