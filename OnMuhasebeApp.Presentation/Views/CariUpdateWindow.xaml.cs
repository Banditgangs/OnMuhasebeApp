using System;
using System.Linq;
using System.Windows;
using OnMuhasebeApp.Application.Services;
using OnMuhasebeApp.Core.Entities;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace OnMuhasebeApp.Presentation.Views
{
    public partial class CariUpdateWindow : Window
    {
        private readonly ICariService _cariService;
        public Cari GuncellenecekCari { get; set; }

        public CariUpdateWindow(ICariService cariService, Cari cari)
        {
            InitializeComponent();
            _cariService = cariService;
            // Mevcut cariyi kopyalıyoruz ki iptal ederse asıl veri bozulmasın
            GuncellenecekCari = cari; 
            this.DataContext = this;
        }

        // --- GİRİŞ ENGELLEME SİSTEMİ ---
        private void SadeceRakamEngeli(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SadeceHarfEngeli(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-ZğüşıöçĞÜŞİÖÇ ]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!text.All(char.IsDigit)) e.CancelCommand();
            }
        }

        // --- ANA KONTROL VE GÜNCELLEME ---
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Ünvan ve Vergi Dairesi Boşluk Kontrolü
            if (string.IsNullOrWhiteSpace(GuncellenecekCari.Unvan) || GuncellenecekCari.Unvan.Trim().Length < 3)
            {
                MessageBox.Show("Ünvan çok kısa veya boş!", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // ⛔ Geçiş Yok!
            }

            if (string.IsNullOrWhiteSpace(GuncellenecekCari.VergiDairesi))
            {
                MessageBox.Show("Vergi Dairesi boş olamaz!", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Vergi No Kesin Kontrol (10 veya 11 hane)
            string vNo = GuncellenecekCari.VergiNo?.Trim() ?? "";
            if (vNo.Length != 10 && vNo.Length != 11)
            {
                MessageBox.Show("Vergi No 10 veya 11 hane olmalıdır!", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // ⛔ 9 hane sızamaz!
            }

            // 3. Telefon Kesin Kontrol (90 ile başlamalı, 12 hane)
            string tel = GuncellenecekCari.Telefon?.Trim() ?? "";
            if (tel.Length != 12 || !tel.StartsWith("90"))
            {
                MessageBox.Show("Telefon 90 ile başlamalı ve tam 12 hane olmalıdır!", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _cariService.UpdateAsync(GuncellenecekCari);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası: " + ex.Message);
            }
        }
    }
}