using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OnMuhasebeApp.Application.Interfaces;
using OnMuhasebeApp.Core.Entities;
using OnMuhasebeApp.Presentation.Commands;

namespace OnMuhasebeApp.Presentation.ViewModels
{
    public class EFaturaListViewModel
    {
        private readonly IEFaturaService _eFaturaService;
        private readonly IFaturaService _localFaturaService;
        public ObservableCollection<Fatura> Faturalar { get; set; }
        
        public ICommand FetchInvoicesCommand { get; }
        public ICommand SaveToDbCommand { get; }
        public ICommand LoadFromDbCommand { get; } // 👈 YENİ: Veritabanından getirme komutu

        public EFaturaListViewModel(IEFaturaService eFaturaService, IFaturaService localFaturaService)
        {
            _eFaturaService = eFaturaService;
            _localFaturaService = localFaturaService;
            Faturalar = new ObservableCollection<Fatura>();

            FetchInvoicesCommand = new RelayCommand(async _ => await SorgulaAsync());
            SaveToDbCommand = new RelayCommand(async _ => await KaydetAsync());
            LoadFromDbCommand = new RelayCommand(async _ => await VeritabanindanGetirAsync()); // 👈 Butona basılınca çalışacak metot
        }

        private async Task SorgulaAsync()
        {
            var veriler = await _eFaturaService.GetGelenFaturalarAsync();
            Faturalar.Clear();
            foreach (var f in veriler) Faturalar.Add(f);
        }

        private async Task KaydetAsync()
        {
            try
            {
                if (Faturalar.Count == 0) {
                    MessageBox.Show("Kaydedilecek fatura bulunamadı!");
                    return;
                }

                await _localFaturaService.SaveInvoicesAsync(Faturalar.ToList());
                MessageBox.Show("Faturalar başarıyla veritabanına mühürlendi!", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                string hataDetayi = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Veritabanı reddetti! Asıl Sebep:\n\n{hataDetayi}", "Veritabanı Kural İhlali", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 👇 YENİ EKLENEN METOT: SQLite veritabanındaki kayıtları okur 👇
        private async Task VeritabanindanGetirAsync()
        {
            try
            {
                var kayitliFaturalar = await _localFaturaService.GetAllFromDbAsync();
                
                Faturalar.Clear(); // Ekrandaki eski listeyi temizle
                foreach (var f in kayitliFaturalar)
                {
                    Faturalar.Add(f); // Veritabanından gelenleri ekrana diz
                }

                if (Faturalar.Count == 0)
                {
                    MessageBox.Show("Veritabanında henüz hiç fatura yok. Önce GİB'den sorgulayıp kaydedin.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Faturalar okunurken hata oluştu:\n\n{ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}