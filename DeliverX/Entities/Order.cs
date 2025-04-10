namespace DeliverX.Entities;
public class Order
{
    public Guid Id { get; set; }
    public required string CustomerName { get; set; }
    public required string Address { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
}

public enum OrderStatus
{
    InProgress,
    Delivered,
    Canceled
}

