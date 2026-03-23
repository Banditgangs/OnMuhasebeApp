using OnMuhasebeApp.Core.Entities;

namespace OnMuhasebeApp.Application.Services
{
    public interface ICariService
    {
        Task<List<Cari>> GetAllAsync();
        Task<Cari?> GetByIdAsync(int id);
        Task<Cari> AddAsync(Cari cari);
        Task UpdateAsync(Cari cari);
        Task DeleteAsync(int id);
    }
}