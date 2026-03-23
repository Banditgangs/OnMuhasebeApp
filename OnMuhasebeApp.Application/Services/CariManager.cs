using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnMuhasebeApp.Application.Interfaces;
using OnMuhasebeApp.Core.Entities;

namespace OnMuhasebeApp.Application.Services
{
    public class CariManager : ICariService
    {
        private readonly IAppDbContext _context;

        public CariManager(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cari>> GetAllAsync()
        {
            try
            {
                // Listelerken hafızaya alma (Performans için AsNoTracking)
                return await _context.Cariler.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Cariler listelenirken bir hata oluştu.", ex);
            }
        }

        public async Task<Cari?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Cariler.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Cari ({id}) getirilirken bir hata oluştu.", ex);
            }
        }

        public async Task<Cari> AddAsync(Cari cari)
        {
            try
            {
                ClearTracker(); // <-- Hafızayı temizle
                _context.Cariler.Add(cari);
                await _context.SaveChangesAsync();
                return cari;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Cari eklenirken bir hata oluştu.", ex);
            }
        }

        public async Task UpdateAsync(Cari cari)
        {
            try
            {
                ClearTracker(); // <-- ÇAKIŞMAYI ÖNLEYEN SİHİR!
                _context.Cariler.Update(cari);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Cari güncellenirken bir hata oluştu.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                ClearTracker(); // <-- ÇAKIŞMAYI ÖNLEYEN SİHİR!
                
                // Silmek için veriyi getirmeye gerek yok, sadece ID'sini bilmek yeterli (Performans taktiği)
                var cari = new Cari { Id = id };
                _context.Cariler.Remove(cari);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Cari silinirken bir hata oluştu.", ex);
            }
        }

        // Veritabanının takip hafızasını sıfırlayan yardımcı metodumuz
        private void ClearTracker()
        {
            if (_context is DbContext dbContext)
            {
                dbContext.ChangeTracker.Clear();
            }
        }
    }
}