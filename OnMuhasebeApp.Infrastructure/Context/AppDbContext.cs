using Microsoft.EntityFrameworkCore;
using OnMuhasebeApp.Core.Entities;
using OnMuhasebeApp.Application.Interfaces;

namespace OnMuhasebeApp.Infrastructure.Context
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Cari> Cariler { get; set; }
        public DbSet<Fatura> Faturalar { get; set; }
        public DbSet<FaturaSatiri> FaturaSatirlari { get; set; }
        public DbSet<KasaIslemi> KasaIslemleri { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cari
            modelBuilder.Entity<Cari>(entity =>
            {
                entity.ToTable("Cariler");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Unvan).IsRequired().HasMaxLength(200);
                entity.Property(e => e.VergiNo).HasMaxLength(50);
            });

            // Fatura
            modelBuilder.Entity<Fatura>(entity =>
            {
                entity.ToTable("Faturalar");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FaturaNo).IsRequired().HasMaxLength(50);

                // İlişki: 1 Cari -> N Fatura
                entity.HasOne(f => f.Cari)
                      .WithMany(c => c.Faturalar)
                      .HasForeignKey(f => f.CariId)
                      .OnDelete(DeleteBehavior.Restrict); // Cari silinirse faturalar silinmesin
            });

            // FaturaSatiri
            modelBuilder.Entity<FaturaSatiri>(entity =>
            {
                entity.ToTable("FaturaSatirlari");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UrunAdi).IsRequired().HasMaxLength(200);

                // İlişki: 1 Fatura -> N FaturaSatiri
                entity.HasOne(fs => fs.Fatura)
                      .WithMany(f => f.FaturaSatirlari)
                      .HasForeignKey(fs => fs.FaturaId)
                      .OnDelete(DeleteBehavior.Cascade); // Fatura silinirse satırları da silinsin
            });

            // KasaIslemi
            modelBuilder.Entity<KasaIslemi>(entity =>
            {
                entity.ToTable("KasaIslemleri");
                entity.HasKey(e => e.Id);
                
                // İlişki: 1 Cari -> N KasaIslemi (Opsiyonel)
                entity.HasOne(k => k.Cari)
                      .WithMany(c => c.KasaIslemleri)
                      .HasForeignKey(k => k.CariId)
                      .OnDelete(DeleteBehavior.SetNull); // Cari silinirse kasa işlemi kalsın, CariId null olsun
            });
        }
    }
}