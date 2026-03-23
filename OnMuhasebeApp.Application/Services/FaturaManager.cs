using System; // Hata fırlatmak (Exception) için eklendi
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnMuhasebeApp.Application.Interfaces;
using OnMuhasebeApp.Core.Entities;

namespace OnMuhasebeApp.Application.Services
{
    public class FaturaManager : IFaturaService
    {
        private readonly IAppDbContext _context;

        public FaturaManager(IAppDbContext context)
        {
            _context = context;
        }

        // 👇 GÜNCELLENDİ: Mükerrer (Çift) Kayıt Engelleme Kuralı Eklendi
        public async Task<Fatura> AddAsync(Fatura fatura)
        {
            // 1. KONTROL: Bu fatura numarası içeride zaten var mı?
            bool varMi = await _context.Faturalar.AnyAsync(f => f.FaturaNo == fatura.FaturaNo);
            
            if (varMi)
            {
                // Varsa, işlemi durdur ve UI (Arayüz) tarafına hata fırlat
                throw new Exception($"'{fatura.FaturaNo}' numaralı e-Fatura zaten sistemde kayıtlı!\nAynı faturayı iki kez işleyemezsiniz.");
            }

            // 2. KAYIT: Eğer içeride yoksa, güvenle veritabanına mühürle
            _context.Faturalar.Add(fatura);
            await _context.SaveChangesAsync();
            return fatura;
        }

        public async Task SaveInvoicesAsync(List<Fatura> faturalar)
        {
            // 1. ADIM: Veritabanında varsayılan bir Cari (Müşteri) var mı bak, yoksa oluştur.
            var defaultCari = await _context.Cariler.FirstOrDefaultAsync(c => c.Unvan == "E-Fatura Müşterisi");
            if (defaultCari == null)
            {
                defaultCari = new Cari 
                { 
                    Unvan = "E-Fatura Müşterisi", 
                    VergiNo = "1111111111", 
                    VergiDairesi = "Sistem", 
                    Telefon = "0000000000", 
                    Email = "efatura@sistem.com", 
                    Adres = "Sistem Kaydı" 
                };
                _context.Cariler.Add(defaultCari);
                await _context.SaveChangesAsync(); // Kaydet ki sistem buna bir ID versin
            }

            // 2. ADIM: Sahipsiz faturaları bu Cari'ye bağla ve kaydet
            foreach (var fat in faturalar)
            {
                var varMi = await _context.Faturalar.AnyAsync(x => x.FaturaNo == fat.FaturaNo);
                
                if (!varMi)
                {
                    fat.CariId = defaultCari.Id; 
                    _context.Faturalar.Add(fat);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Fatura>> GetAllFromDbAsync()
        {
            return await _context.Faturalar.AsNoTracking().ToListAsync();
        }
    }
}