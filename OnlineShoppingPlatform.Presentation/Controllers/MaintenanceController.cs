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
using OnlineShoppingPlatform.Business.Types;
using OnlineShoppingPlatform.Business.Operations.User.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
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

            return BadRequest(result.Message);

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

   
        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceInfoDto>> GetMaintenanceById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz Bakım ID'si."
                });
            }

            var maintenance = await _maintenanceService.GetMaintenanceByIdAsync(id);

            var productInfoDto = new MaintenanceInfoDto
            {
                MaintenanceId = maintenance.MaintenanceId,
                IsActive = maintenance.IsActive,
                Message = maintenance.Message,
                StartTime = maintenance.StartTime,
                EndTime = maintenance.EndTime
            };

            return Ok(productInfoDto);
        }


        [HttpGet("all")]
        public async Task<ActionResult<List<MaintenanceInfoDto>>> GetAllMaintenance()
        {

            var maintenanceList = await _maintenanceService.GetAllMaintenanceAsync();

            var productInfoDtoList = maintenanceList.Select(maintenance => new MaintenanceInfoDto
            {
               MaintenanceId = maintenance.MaintenanceId,
               IsActive = maintenance.IsActive,
               Message = maintenance.Message,
               StartTime = maintenance.StartTime,
               EndTime = maintenance.EndTime

            }).ToList();


            return Ok(productInfoDtoList!);

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz Bakım Kaydı ID'si."
                });
            }

            var updateMaintenanceDto = new UpdateMaintenanceDto
            {
               MaintenanceId = id,
               IsActive = updateMaintenanceRequest.IsActive,
               Message = updateMaintenanceRequest.Message,
               StartTime = updateMaintenanceRequest.StartTime,
               EndTime = updateMaintenanceRequest.EndTime,
            };

    

            if (updateMaintenanceDto == null)
            {
               return BadRequest("Maintenance data cannot be null.");
            }



            var result = await _maintenanceService.UpdateMaintenanceAsync(updateMaintenanceDto);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }
 
            return BadRequest(result.Message);
     

        }

        [HttpDelete("{id}")]
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

            return BadRequest(result);


        }


    }
}
