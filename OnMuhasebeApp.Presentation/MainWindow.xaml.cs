using System.Windows;
using OnMuhasebeApp.Presentation.ViewModels;

namespace OnMuhasebeApp.Presentation
{
    public partial class MainWindow : Window
    {
        // Artık CariList değil, MainViewModel (Trafik Polisi) alıyor
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}