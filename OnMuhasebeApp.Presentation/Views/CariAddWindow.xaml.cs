using System;
using System.Linq;
using System.Windows;
using OnMuhasebeApp.Application.Services;
using OnMuhasebeApp.Core.Entities;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace OnMuhasebeApp.Presentation.Views
{
    public partial class CariAddWindow : Window
    {
        private readonly ICariService _cariService;
        public Cari YeniCari { get; set; }

        public CariAddWindow(ICariService cariService)
        {
            InitializeComponent();
            _cariService = cariService;
            YeniCari = new Cari();
            this.DataContext = this; 
        }

        // --- 1. TURNİKE: KLAVYE GİRİŞ ENGELLEME ---
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

        // --- 2. TURNİKE: KOPYALA-YAPIŞTIR ENGELLEME ---
        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                // Eğer yapıştırılan metin rakam dışında bir şey içeriyorsa iptal et
                if (!text.All(char.IsDigit)) 
                {
                    e.CancelCommand();
                }
            }
        }

        // --- 3. ANA GÜMRÜK: KAYDET BUTONU (KESİN KONTROL) ---
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // --- ÜNVAN KONTROLÜ ---
            if (string.IsNullOrWhiteSpace(YeniCari.Unvan) || YeniCari.Unvan.Trim().Length < 3)
            {
                GosterUyari("Cari Ünvanı geçersiz! En az 3 karakterli resmi ünvan giriniz.");
                return; // ⛔ HATA VAR, AŞAĞIYA GEÇME!
            }

            // --- VERGİ DAİRESİ KONTROLÜ ---
            if (string.IsNullOrWhiteSpace(YeniCari.VergiDairesi))
            {
                GosterUyari("Vergi Dairesi boş bırakılamaz!");
                return; // ⛔ DUR!
            }

            // --- VERGİ NO KONTROLÜ (10 veya 11 HANE ZORUNLULUĞU) ---
            string vNo = YeniCari.VergiNo?.Trim() ?? "";
            if (vNo.Length != 10 && vNo.Length != 11)
            {
                GosterUyari("Hatalı Vergi No!\n\n- Kurumlar için 10 hane\n- Şahıslar için 11 hane (TCKN) olmalıdır.");
                return; // ⛔ GEÇİT YOK!
            }

            // --- TELEFON KONTROLÜ (90 İLE BAŞLAMALI VE TAM 12 HANE OLMALI) ---
            string tel = YeniCari.Telefon?.Trim() ?? "";
            if (tel.Length != 12 || !tel.StartsWith("90"))
            {
                GosterUyari("Hatalı Telefon Formatı!\n\nNumara '90' ile başlamalı ve TAM 12 hane olmalıdır.\nÖrn: 905XXXXXXXXX");
                return; // ⛔ BURADAN ÖTEYE GEÇEMEZSİN!
            }

            // --- EĞER BURAYA KADAR GELDİYSEK VERİLER TERTEMİZDİR ---
            try
            {
                YeniCari.Email ??= "";
                YeniCari.Adres ??= "";

                await _cariService.AddAsync(YeniCari);
                this.DialogResult = true; 
                this.Close();
            }
            catch (Exception ex)
            {
                string gercekHata = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Kayıt Hatası: {gercekHata}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GosterUyari(string mesaj)
        {
            MessageBox.Show(mesaj, "Doğrulama Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}