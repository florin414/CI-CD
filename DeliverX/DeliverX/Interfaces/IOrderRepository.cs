using DeliverX.Entities;

namespace DeliverX.Interfaces;
public interface IOrderRepository
{
    IEnumerable<Order> GetAllOrders();
    Order GetOrderById(Guid id);
    void AddOrder(Order order);
}