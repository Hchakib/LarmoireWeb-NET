using System.Collections.Generic;
using System.Threading.Tasks;
using LarmoireWeb.Models;

namespace LarmoireWeb.Data.Services
{
    public interface IProduitService
    {
        // Ajout du filtre par catégorie
        Task<IEnumerable<Produit>> GetAllAsync(int? categorieId = null);
        Task<Produit> GetByIdAsync(int id);
        Task<bool> AddNewAsync(Produit produit);
        Task<Produit> UpdateAsync(int id, Produit produit);
        Task DeleteAsync(int id);
    }
}
