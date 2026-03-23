using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnMuhasebeApp.Application.Interfaces;
using OnMuhasebeApp.Application.Services;
using OnMuhasebeApp.Infrastructure.Context;
using OnMuhasebeApp.Infrastructure.Services;
using OnMuhasebeApp.Presentation.ViewModels;

namespace OnMuhasebeApp.Presentation
{
    public partial class App : System.Windows.Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider(); // Pasta burada fırına veriliyor
        }

        private void ConfigureServices(IServiceCollection services)
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OnMuhasebeApp.db");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            services.AddScoped<ICariService, CariManager>();
            services.AddScoped<IEFaturaService, EFaturaService>();
            
            // 👇 DOĞRU YER BURASI! (Diğer servislerin yanı) 👇
            services.AddScoped<IFaturaService, FaturaManager>(); 
            
            services.AddHttpClient();
            services.AddTransient<KasaViewModel>();

            services.AddTransient<CariListViewModel>();
            services.AddTransient<EFaturaListViewModel>();
            services.AddTransient<MainWindow>();
            
            services.AddTransient<MainViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (s, ev) =>
            {
                if (ev.ExceptionObject is Exception ex)
                {
                    MessageBox.Show($"Kritik Hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    dbContext.Database.EnsureCreated(); 
                }

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Uygulama başlatılırken bir hata oluştu: {ex.Message}", "Başlatma Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    } 
}