namespace DeliverX.Entities;
public class DeliveryAgent
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public VehicleType Vehicle { get; set; }
}

public enum VehicleType
{
    Bicycle,
    Motorcycle,
    Car,
    Van,
    Truck,
    Scooter
}

