using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using OnMuhasebeApp.Presentation.Commands;

namespace OnMuhasebeApp.Presentation.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        // Komutlarımız
        public ICommand ShowCariCommand { get; }
        public ICommand ShowKasaCommand { get; }
        public ICommand ShowEFaturaCommand { get; } // 👈 Yeni ekledik

        // Sayfalarımız (ViewModel'ler)
        private readonly CariListViewModel _cariListViewModel;
        private readonly KasaViewModel _kasaViewModel;
        private readonly EFaturaListViewModel _eFaturaListViewModel; // 👈 Yeni ekledik

        public MainViewModel(
            CariListViewModel cariListViewModel, 
            KasaViewModel kasaViewModel, 
            EFaturaListViewModel eFaturaListViewModel) // 👈 Buraya da ekledik
        {
            _cariListViewModel = cariListViewModel;
            _kasaViewModel = kasaViewModel;
            _eFaturaListViewModel = eFaturaListViewModel;

            // İlk açılışta Cariler gelsin
            CurrentView = _cariListViewModel;

            // Butonlara görevlerini atıyoruz
            ShowCariCommand = new RelayCommand(_ => CurrentView = _cariListViewModel);
            ShowKasaCommand = new RelayCommand(_ => CurrentView = _kasaViewModel);
            ShowEFaturaCommand = new RelayCommand(_ => CurrentView = _eFaturaListViewModel); // 👈 Yeni görev
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}