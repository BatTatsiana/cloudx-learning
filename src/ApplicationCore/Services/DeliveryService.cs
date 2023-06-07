using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Requests;
using Address = Microsoft.eShopWeb.ApplicationCore.Requests.Address;
using OrderItem = Microsoft.eShopWeb.ApplicationCore.Requests.OrderItem;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class DeliveryService:IDeliveryService
{
    private readonly HttpClient _httpClient;

    public DeliveryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ProcessDelivery(Order order)
    {
        var req = new DeliveryOrderRequest()
        {
            Address = new Address()
            {
                City = order.ShipToAddress.City,
                Country = order.ShipToAddress.Country,
                State = order.ShipToAddress.State,
                Street = order.ShipToAddress.Street,
                ZipCode = order.ShipToAddress.ZipCode
            },
            OrderId = order.Id,
            Items = order
                .OrderItems
                .Select(i => new OrderItem { Id = i.ItemOrdered.CatalogItemId, Quantity = i.Units })
                .ToArray(),
            Price = order.Total(),
        };
        var request = new
        {
            OrderId = order.Id,
            Address = order.ShipToAddress,
            Price = order.Total(),
            Items = order.OrderItems.Select(i => new { Id = i.ItemOrdered.CatalogItemId, Quantity = i.Units })
                .ToArray()
        };

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("", jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            throw new System.Exception("Error occurred during delivery processing");
        }
    }
}
