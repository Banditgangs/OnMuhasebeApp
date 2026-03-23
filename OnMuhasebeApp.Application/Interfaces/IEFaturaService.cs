using System.Collections.Generic;
using System.Threading.Tasks;
using OnMuhasebeApp.Core.Entities;

namespace OnMuhasebeApp.Application.Interfaces // 👈 Sistemin aradığı ve bulamadığı adres tam olarak bu!
{
    public interface IEFaturaService
    {
        Task<bool> FaturaGonderAsync(Fatura fatura);
        Task<List<Fatura>> GetGelenFaturalarAsync();
    }
}