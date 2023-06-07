using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class OrderReserverService : IOrderReserverService
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IAppLogger<OrderReserverService> _logger;
    private const string _orderItemsReserveTopic = "orderreservation";

    public OrderReserverService(ServiceBusClient serviceBusClient, IAppLogger<OrderReserverService> logger)
    {
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }

    public async Task ReserveOrderItems(int orderId, IEnumerable<OrderItem> orderItems)
    {
        var content = JsonSerializer.Serialize(new
        {
            orderId = orderId,
            items = orderItems
                .Select(i => new { id = i.ItemOrdered.CatalogItemId, quantity = i.Units })
                .ToList()
        });

        try
        {
            await using var sender = _serviceBusClient.CreateSender(_orderItemsReserveTopic);
            await sender.SendMessageAsync(new ServiceBusMessage(content));
        }

        catch (Exception ex)
        {
            _logger.LogWarning($"Unable to reserve order items. OrderId:'{orderId}'", ex);
        }
    }
}
