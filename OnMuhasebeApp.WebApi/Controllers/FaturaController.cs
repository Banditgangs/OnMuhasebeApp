using Microsoft.AspNetCore.Mvc;
using OnMuhasebeApp.Application.Interfaces;
using System.Threading.Tasks;

namespace OnMuhasebeApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaturaController : ControllerBase
    {
        private readonly IFaturaService _faturaService;

        public FaturaController(IFaturaService faturaService)
        {
            _faturaService = faturaService;
        }

        // İnternetten "api/fatura" adresine GET isteği gelirse veritabanındaki faturaları yolla
        [HttpGet]
        public async Task<IActionResult> GetFaturalar()
        {
            var faturalar = await _faturaService.GetAllFromDbAsync();
            if (faturalar == null || faturalar.Count == 0)
            {
                return NotFound("Sistemde kayıtlı fatura bulunamadı.");
            }
            return Ok(faturalar);
        }
    }
}