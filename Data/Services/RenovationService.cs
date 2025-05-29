// Data/Services/RenovationService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LarmoireWeb.Data;
using LarmoireWeb.Models;

namespace LarmoireWeb.Data.Services
{
    public class RenovationService : IRenovationService
    {
        private readonly AppDbContext _ctx;
        public RenovationService(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<RenovationRequest>> GetAllAsync() =>
            await _ctx.Set<RenovationRequest>()
                      .Include(r => r.User)
                      .OrderByDescending(r => r.CreatedAt)
                      .ToListAsync();

        public async Task<IEnumerable<RenovationRequest>> GetByUserAsync(string userId) =>
            await _ctx.Set<RenovationRequest>()
                      .Where(r => r.UserId == userId)
                      .OrderByDescending(r => r.CreatedAt)
                      .ToListAsync();

        public async Task<RenovationRequest> GetByIdAsync(int id)
    => await _ctx.Set<RenovationRequest>().Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);


        public async Task SubmitAsync(RenovationRequest request)
        {
            _ctx.Add(request);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, RequestStatus status)
        {
            var r = await _ctx.Set<RenovationRequest>().FindAsync(id);
            if (r != null)
            {
                r.Status = status;
                _ctx.Update(r);
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)                           // ← ajouté
        {
            var r = await _ctx.Set<RenovationRequest>().FindAsync(id);
            if (r != null)
            {
                _ctx.Remove(r);
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
