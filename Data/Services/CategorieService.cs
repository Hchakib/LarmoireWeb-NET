// Data/Services/CategorieService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LarmoireWeb.Data;
using LarmoireWeb.Models;

namespace LarmoireWeb.Data.Services
{
    public class CategorieService : ICategorieService
    {
        private readonly AppDbContext _ctx;
        public CategorieService(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Categorie>> GetAllAsync() =>
            await _ctx.Categories.Include(c => c.Produits)
                                 .OrderBy(c => c.Nom)
                                 .ToListAsync();

        public async Task<Categorie> GetByIdAsync(int id) =>
            await _ctx.Categories.Include(c => c.Produits)
                                 .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<bool> AddNewAsync(Categorie categorie)
        {
            if (await _ctx.Categories.AnyAsync(c => c.Nom == categorie.Nom))
                return false;
            _ctx.Categories.Add(categorie);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<Categorie> UpdateAsync(int id, Categorie categorie)
        {
            _ctx.Categories.Update(categorie);
            await _ctx.SaveChangesAsync();
            return categorie;
        }

        public async Task DeleteAsync(int id)
        {
            var c = await _ctx.Categories.FindAsync(id);
            if (c != null)
            {
                _ctx.Categories.Remove(c);
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
