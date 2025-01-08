using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities.Enums
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Completed,
        Canceled,
        Refunded,
        Failed,
        OnHold,
        InTransit,
        OutforDelivery,
        Returned
    }
}
