// Data/Services/ICategorieService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using LarmoireWeb.Models;

namespace LarmoireWeb.Data.Services
{
    public interface ICategorieService
    {
        Task<IEnumerable<Categorie>> GetAllAsync();
        Task<Categorie> GetByIdAsync(int id);
        Task<bool> AddNewAsync(Categorie categorie);
        Task<Categorie> UpdateAsync(int id, Categorie categorie);
        Task DeleteAsync(int id);
    }
}
