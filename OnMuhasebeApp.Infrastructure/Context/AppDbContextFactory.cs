using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OnMuhasebeApp.Infrastructure.Context
{
    // Bu sınıf sadece migration komutları çalıştırılırken EF Core tarafından kullanılır.
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            
            // SQLite için bağlantı dizesini veriyoruz. Veritabanı dosyası proje dizininde oluşacak.
            optionsBuilder.UseSqlite("Data Source=OnMuhasebeApp.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}