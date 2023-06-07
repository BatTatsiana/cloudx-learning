namespace DeliveryOrderProcessor.Requests;

public class DeliveryOrderRequest
{
    public int OrderId { get; set; }
    public OrderItem[] Items { get; set; }
    public Address Address { get; set; }
    public decimal Price { get; set; }
}
