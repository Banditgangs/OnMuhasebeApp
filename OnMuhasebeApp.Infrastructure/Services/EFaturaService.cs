using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OnMuhasebeApp.Application.Interfaces; 
using OnMuhasebeApp.Core.Entities;

namespace OnMuhasebeApp.Infrastructure.Services
{
    // 👇 İŞTE DEĞİŞİKLİK BURADA: Arabirimin tam adresini (Açık Adres) yazdık! 👇
    public class EFaturaService : OnMuhasebeApp.Application.Interfaces.IEFaturaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://api.efaturasaglayici.com/v1/invoices";
        private readonly string _token = "DUMMY_BEARER_TOKEN_12345";

        public EFaturaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Fatura>> GetGelenFaturalarAsync()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                var response = await _httpClient.GetAsync(_apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Fatura>>(jsonString) ?? new List<Fatura>();
                }
                return await GetSimuleFaturalar();
            }
            catch (Exception)
            {
                return await GetSimuleFaturalar();
            }
        }

        public async Task<bool> FaturaGonderAsync(Fatura fatura)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                var jsonContent = JsonSerializer.Serialize(fatura);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_apiUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("e-Fatura gönderimi sırasında hata oluştu.", ex);
            }
        }

        private async Task<List<Fatura>> GetSimuleFaturalar()
        {
            await Task.Delay(1000); 
            return new List<Fatura>
            {
                new Fatura { FaturaNo = "GIB202600000001", ToplamTutar = 1500.50m, FaturaTuru = "SATIŞ", Tarih = DateTime.Now.AddDays(-1) },
                new Fatura { FaturaNo = "GIB202600000002", ToplamTutar = 2750.00m, FaturaTuru = "HİZMET", Tarih = DateTime.Now }
            };
        }
    }
}