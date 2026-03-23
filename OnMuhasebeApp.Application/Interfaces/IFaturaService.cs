using System.Collections.Generic;
using System.Threading.Tasks;
using OnMuhasebeApp.Core.Entities;

namespace OnMuhasebeApp.Application.Interfaces
{
    public interface IFaturaService
    {
        // 👇 BİZİM EKLENTİMİZ: Tekli Fatura (XML) kaydetme metodu
        Task<Fatura> AddAsync(Fatura fatura); 

        // 👇 SENİN ORİJİNAL METODLARIN (Bozmuyoruz)
        Task SaveInvoicesAsync(List<Fatura> faturalar);
        Task<List<Fatura>> GetAllFromDbAsync();
    }
}