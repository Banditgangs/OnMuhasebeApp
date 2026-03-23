using Microsoft.AspNetCore.Mvc;
using OnMuhasebeApp.Infrastructure.Context;
using OnMuhasebeApp.Core.Entities; // Core katmanındaki KasaIslemi nesnesi için

namespace OnMuhasebeApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KasaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public KasaController(AppDbContext context) => _context = context;

        // Kasa Hareketlerini Listele: GET http://localhost:5018/api/Kasa
        [HttpGet]
        public IActionResult Get()
        {
            var islemler = _context.KasaIslemleri.OrderByDescending(x => x.Tarih).ToList();
            return Ok(islemler);
        }

        // Yeni Kasa Hareketi Ekle (Gelir/Gider): POST http://localhost:5018/api/Kasa
        [HttpPost]
        public IActionResult Post([FromBody] KasaIslemi yeniIslem)
        {
            if (yeniIslem == null) return BadRequest();
            
            yeniIslem.Tarih = DateTime.Now; // Tarihi otomatik sunucuda verelim
            _context.KasaIslemleri.Add(yeniIslem);
            _context.SaveChanges();
            
            return Ok(yeniIslem);
        }
    }
}