using Microsoft.AspNetCore.Mvc;
using OnMuhasebeApp.Infrastructure.Context;
// 👇 Burayı Core olarak düzelttik, artık hata vermeyecek
using OnMuhasebeApp.Core.Entities; 

namespace OnMuhasebeApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CariController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CariController(AppDbContext context)
        {
            _context = context;
        }

        // Listeleme: GET http://localhost:5018/api/Cari
        [HttpGet]
        public IActionResult Get()
        {
            var liste = _context.Cariler.ToList();
            return Ok(liste);
        }

        // Kaydetme: POST http://localhost:5018/api/Cari
        [HttpPost]
        public IActionResult Post([FromBody] Cari yeniCari)
        {
            if (yeniCari == null) return BadRequest("Cari verisi boş olamaz.");
            
            _context.Cariler.Add(yeniCari);
            _context.SaveChanges();
            
            return Ok(yeniCari);
        }
    }
}