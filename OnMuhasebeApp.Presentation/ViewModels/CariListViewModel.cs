using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using OnMuhasebeApp.Application.Services;
using OnMuhasebeApp.Core.Entities;
using OnMuhasebeApp.Presentation.Commands;
using OnMuhasebeApp.Presentation.Views;
using OnMuhasebeApp.Application.Interfaces;
using OnMuhasebeApp.Infrastructure.Services; 

namespace OnMuhasebeApp.Presentation.ViewModels
{
    public class CariListViewModel : INotifyPropertyChanged
    {
        private readonly ICariService _cariService;
        private readonly IFaturaService _faturaService; // 👇 VERİTABANI İÇİN EKLENDİ

        private ObservableCollection<Cari> _cariler;
        private List<Cari> _tumCariler;

        private Cari _selectedCari;
        public Cari SelectedCari
        {
            get => _selectedCari;
            set { _selectedCari = value; OnPropertyChanged(); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                AramaYap();
            }
        }

        public ObservableCollection<Cari> Cariler
        {
            get => _cariler;
            set { _cariler = value; OnPropertyChanged(); }
        }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand FaturaIceriAktarCommand { get; } 

        // 👇 CONSTRUCTOR GÜNCELLENDİ: Artık FaturaService'i de içeri alıyor
        public CariListViewModel(ICariService cariService, IFaturaService faturaService)
        {
            _cariService = cariService;
            _faturaService = faturaService; // 👇 ATAMASI YAPILDI
            _cariler = new ObservableCollection<Cari>();
            _tumCariler = new List<Cari>();

            LoadCommand = new RelayCommand(async _ => await LoadCarilerAsync());

            AddCommand = new RelayCommand(_ => {
                var addWindow = new CariAddWindow(_cariService);
                if (addWindow.ShowDialog() == true) { _ = LoadCarilerAsync(); }
            });

            DeleteCommand = new RelayCommand(async _ => {
                if (SelectedCari == null) 
                { 
                    MessageBox.Show("Lütfen silmek için listeden bir cari seçin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning); 
                    return; 
                }

                var cevap = MessageBox.Show($"{SelectedCari.Unvan} adlı cariyi silmek istediğinize emin misiniz?\n\nNot: Eğer bu cariye bağlı faturalar varsa sistem silmeye izin vermeyecektir.", "Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (cevap == MessageBoxResult.Yes) 
                { 
                    try 
                    {
                        await _cariService.DeleteAsync(SelectedCari.Id); 
                        await LoadCarilerAsync(); 
                        MessageBox.Show("Cari başarıyla sistemden kaldırıldı.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        var asilHata = ex;
                        while (asilHata.InnerException != null)
                        {
                            asilHata = asilHata.InnerException;
                        }

                        MessageBox.Show($"Bu cariyi silemezsiniz!\n\nSebep: {asilHata.Message}", 
                                        "Veri Bütünlüğü Kısıtlaması", 
                                        MessageBoxButton.OK, 
                                        MessageBoxImage.Error);
                    }
                }
            });

            UpdateCommand = new RelayCommand(_ => {
                if (SelectedCari == null) { MessageBox.Show("Lütfen düzenlemek için listeden bir cari seçin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                var updateWindow = new CariUpdateWindow(_cariService, SelectedCari);
                if (updateWindow.ShowDialog() == true) { _ = LoadCarilerAsync(); }
            });

            // 👇 GÜNCELLENDİ: Faturayı Okuyup Veritabanına Kaydeden Ana Komut
            FaturaIceriAktarCommand = new RelayCommand(async _ => { // async eklendi
                
                // 1. KONTROL: Müşteri seçili mi?
                if (SelectedCari == null)
                {
                    MessageBox.Show("Lütfen faturayı işlemek için listeden bir müşteri (Cari) seçin!", 
                                    "Müşteri Seçilmedi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; 
                }

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "GİB e-Fatura XML Dosyasını Seçiniz";
                openFileDialog.Filter = "XML Dosyaları (*.xml)|*.xml|Tüm Dosyalar (*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        var invoiceService = new InvoiceService();
                        var okunanFatura = invoiceService.FaturayiOku(openFileDialog.FileName);

                        // 2. EŞLEŞTİRME: Faturayı seçilen cariye (müşteriye) bağla
                        okunanFatura.CariId = SelectedCari.Id;
                        okunanFatura.Tarih = DateTime.Now;
                        okunanFatura.FaturaTuru = "Gelen E-Fatura";

                        // 3. VERİTABANINA KAYIT: İşte mühür vurduğumuz an!
                        await _faturaService.AddAsync(okunanFatura);

                        MessageBox.Show($"E-Fatura Çözümlendi ve {SelectedCari.Unvan} Hesabına Eklendi!\n\n" +
                                        $"🏢 Şirket Ünvanı: {okunanFatura.SirketAdi}\n" +
                                        $"🧾 Fatura No: {okunanFatura.FaturaNo}\n" +
                                        $"💰 Toplam Tutar: {okunanFatura.ToplamTutar} TL\n\n" +
                                        $"✅ Fatura başarıyla veritabanına kaydedildi.", 
                                        "İşlem Başarılı", 
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "XML Okuma veya Kayıt Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });

            _ = LoadCarilerAsync();
        }

        private async Task LoadCarilerAsync()
        {
            var data = await _cariService.GetAllAsync();
            _tumCariler = data;
            SearchText = string.Empty; 
            Cariler.Clear();
            foreach (var item in _tumCariler)
            {
                Cariler.Add(item);
            }
        }

        private void AramaYap()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Cariler.Clear();
                foreach (var item in _tumCariler) Cariler.Add(item);
                return;
            }

            var kucukHarfArama = SearchText.ToLower();
            var filtrelenmisListe = _tumCariler.Where(c => 
                (c.Unvan != null && c.Unvan.ToLower().Contains(kucukHarfArama)) ||
                (c.VergiNo != null && c.VergiNo.Contains(kucukHarfArama))
            ).ToList();

            Cariler.Clear();
            foreach (var item in filtrelenmisListe) Cariler.Add(item);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}