using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IOrderReserverService
{
    Task ReserveOrderItems(int orderId, IEnumerable<OrderItem> orderItems);
}