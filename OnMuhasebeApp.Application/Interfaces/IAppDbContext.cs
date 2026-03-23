using Microsoft.EntityFrameworkCore;
using OnMuhasebeApp.Core.Entities;

namespace OnMuhasebeApp.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Cari> Cariler { get; set; }
        DbSet<Fatura> Faturalar { get; set; }
        DbSet<FaturaSatiri> FaturaSatirlari { get; set; }
        DbSet<KasaIslemi> KasaIslemleri { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}