using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.Business.Operations.Maintenance.Dtos;
using OnlineShoppingPlatform.Business.Types;
using OnlineShoppingPlatform.DataAccess.Maintenance;

namespace OnlineShoppingPlatform.Business.Operations.Maintenance
{
    public interface IMaintenance
    {
        Task<MaintenanceMode> GetMaintenanceByIdAsync(int id);
        Task<List<MaintenanceMode>> GetAllMaintenanceAsync();

        Task<ServiceMessage<MaintenanceInfoDto>> ToggleLastMaintenanceModeOnOff();
        Task<ServiceMessage<MaintenanceInfoDto>> ToggleMaintenanceModeOnOff(int id);


        Task<ServiceMessage> AddMaintenanceAsync(AddMaintenanceDto addMaintenanceDto);

        Task<ServiceMessage> UpdateMaintenanceAsync(UpdateMaintenanceDto updateMaintenanceDto);
        Task<ServiceMessage> DeleteMaintenanceAsync(int id);
    }
}
