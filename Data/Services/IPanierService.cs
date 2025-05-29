// Data/Services/IPanierService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using LarmoireWeb.Models;

namespace LarmoireWeb.Data.Services
{
    public interface IPanierService
    {
        Task<IEnumerable<Panier>> GetByUserAsync(string userId);
        Task AddOrUpdateAsync(string userId, int produitId, int quantite);
        Task DeleteAsync(int id);
    }
}
