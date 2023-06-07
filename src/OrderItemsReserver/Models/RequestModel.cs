namespace OrderItemsReserver.Models;

public class RequestModel
{
    public string OrderId { get; set; }
    public OrderItem[] Items { get; set; }
}