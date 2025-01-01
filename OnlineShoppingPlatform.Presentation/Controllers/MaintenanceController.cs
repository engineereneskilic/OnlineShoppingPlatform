using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatform.DataAccess;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.DataAccess.Entities.Maintenance;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MaintenanceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleMaintenanceModeOnOff()
        {
            var maintenance = await _context.MaintenanceModes.FirstOrDefaultAsync();
            if (maintenance == null)
            {
                //maintenance = new MaintenanceMode { IsActive = true };
                //await _context.MaintenanceModes.AddAsync(maintenance);
                return Ok("Herhangi bir bakım modu bulunmadığı için bakım modunu aktif yada pasif hale getiremezsiniz!");
            }
            else
            {
                maintenance.IsActive = maintenance.IsActive ? false : true; 
            }

            await _context.SaveChangesAsync();
            return Ok(new { Status = maintenance.IsActive ? "Aktif" : "Pasif" });
        }
        [HttpPost("toggle/{id}")]
        public async Task<IActionResult> ToggleMaintenanceModeOnOff(int id)
        {
            // ID'ye göre bakım modunu bul
            var maintenance = await _context.MaintenanceModes.FirstOrDefaultAsync(m => m.Id == id);

            if (maintenance == null)
            {
                return NotFound("Belirtilen bakım modu bulunamadı!");
            }

            // Bakım modunun aktifliğini tersine çevir
            maintenance.IsActive = maintenance.IsActive ? false : true;

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();

            // Sonuç olarak aktif veya pasif olduğunu belirten bir cevap döndür
            return Ok(new { Status = maintenance.IsActive ? "Aktif" : "Pasif" });
        }


        [HttpPost]
        public async Task<IActionResult> AddMaintenanceMode([FromBody] MaintenanceMode model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.MaintenanceModes.Add(model);
            await _context.SaveChangesAsync();
            return Ok("Bakım modu başarıyla eklendi.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaintenanceMode(int id, [FromBody] MaintenanceMode model)
        {
            var maintenance = await _context.MaintenanceModes.FindAsync(id);

            if (maintenance == null)
                return NotFound("Bakım modu bulunamadı.");

            maintenance.StartTime = model.StartTime;
            maintenance.EndTime = model.EndTime;
            maintenance.Message = model.Message;
            maintenance.IsActive = model.IsActive;


            await _context.SaveChangesAsync();
            return Ok("Bakım modu başarıyla güncellendi.");
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMaintenanceMode(int id)
        {
            // ID'ye göre bakım modunu bul
            var maintenance = await _context.MaintenanceModes.FirstOrDefaultAsync(m => m.Id == id);

            if (maintenance == null)
            {
                return NotFound("Belirtilen bakım modu bulunamadı!");
            }

            // Bakım modunu sil
            _context.MaintenanceModes.Remove(maintenance);

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();

            // Silme işlemi başarılıysa, başarılı bir yanıt döndür
            return Ok("Bakım modu başarıyla silindi.");
        }


    }
}
