using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatform.DataAccess;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.DataAccess.Maintenance;
using OnlineShoppingPlatform.Business.Operations.Maintenance;
using OnlineShoppingPlatform.Business.Operations.Maintenance.Dtos;
using OnlineShoppingPlatform.Presentation.Models.Main;
using OnlineShoppingPlatform.Business.Operations.Product.Dtos;
using OnlineShoppingPlatform.Business.Operations.Product;
using OnlineShoppingPlatform.Presentation.Models.Maintenance;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenance _maintenanceService;

        public MaintenanceController(IMaintenance maintenance)
        {
            _maintenanceService = maintenance;
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleLastMaintenanceModeOnOff()
        {
            //var maintenance = await _context.MaintenanceModes.FirstOrDefaultAsync();
            var result = await _maintenanceService.ToggleLastMaintenanceModeOnOff();

            if (result.IsSucceed)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }

        }


        [HttpPost("toggle/{id}")]
        public async Task<IActionResult> ToggleMaintenanceModeOnOff(int id)
        {
            //var maintenance = await _context.MaintenanceModes.FirstOrDefaultAsync();
            var result = await _maintenanceService.ToggleMaintenanceModeOnOff(id);

            if (result.IsSucceed)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddMaintenanceMode([FromBody] AddMaintenanceRequest addMaintenanceRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addMaintenanceDto = new AddMaintenanceDto
            {
                IsActive = addMaintenanceRequest.IsActive,
                StartTime = addMaintenanceRequest.StartTime,
                EndTime = addMaintenanceRequest.EndTime,
                Message = addMaintenanceRequest.Message
            };

            var result = await _maintenanceService.AddMaintenanceAsync(addMaintenanceDto);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaintenanceMode(int id, [FromBody] UpdateMaintenanceRequest updateMaintenanceRequest)
        {

            var updateMaintenanceDto = new UpdateMaintenanceDto
            {
               MaintenanceId = id,
               IsActive = updateMaintenanceRequest.IsActive,
               Message = updateMaintenanceRequest.Message,
               StartTime = updateMaintenanceRequest.StartTime,
               EndTime = updateMaintenanceRequest.EndTime,
            };

            // Kontrol 1: Gönderilen ID ile ürünün ID'si eşleşiyor mu?
            if (id != updateMaintenanceRequest.MaintenanceId)
            {
                return BadRequest("The provided ID does not match the product ID.");
            }

            // Kontrol 2: Gönderilen ürün verisi null mı?
            if (updateMaintenanceDto == null)
            {
               return BadRequest("Maintenance data cannot be null.");
            }

            // Kontrol 3: Veritabanında ürünün mevcut olup olmadığını kontrol et

            var existingMaintenance = await _maintenanceService.GetMaintenanceByIdAsync(id);

            if (existingMaintenance == null)
            {
                return NotFound("The Maintenance with the specified ID does not exist.");
            }

            var result = await _maintenanceService.UpdateMaintenanceAsync(updateMaintenanceDto);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }

        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMaintenanceMode(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Geçersiz ID. Lütfen geçerli bir ID girin.");
            }

            var result = await _maintenanceService.DeleteMaintenanceAsync(id);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }

        }


    }
}
