using System.Collections.Generic;
using LarmoireWeb.Models;

namespace LarmoireWeb.ViewModels
{
    public class CommandeIndexVm
    {
        // Terme de recherche sur Prénom, Nom ou Téléphone
        public string SearchTerm { get; set; }

        // Liste des commandes filtrées
        public IEnumerable<Commande> Orders { get; set; }
    }
}