using System.Collections.Generic;
using LarmoireWeb.Models;

namespace LarmoireWeb.ViewModels
{
    public class RenovationIndexVm
    {
        // Terme de recherche sur nom / prénom / téléphone
        public string SearchTerm { get; set; }

        // Résultat à afficher
        public IEnumerable<RenovationRequest> Requests { get; set; }
    }
}
