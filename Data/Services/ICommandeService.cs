using System.Collections.Generic;
using System.Threading.Tasks;
using LarmoireWeb.Models;

namespace LarmoireWeb.Data.Services
{
    public interface ICommandeService
    {
        Task<IEnumerable<Commande>> GetAllAsync();
        Task<IEnumerable<Commande>> GetByUserAsync(string userId);
        Task<Commande> GetByIdAsync(int id);
        Task PlaceOrderAsync(string userId);   // ← nouveau
    }
}
