using DeliverX.Entities;
using DeliverX.Interfaces;
using DeliverX.Results;
using FluentResults;

namespace DeliverX.Services;
public class OrderService
{
    private readonly IOrderRepository orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        this.orderRepository = orderRepository;
    }

    public IEnumerable<Order> GetAllOrders()
    {
        return orderRepository.GetAllOrders();
    }

    public Result<Order> GetOrderById(Guid id)
    {
        var order = orderRepository.GetOrderById(id);

        return order != null
            ? Result.Ok(order)
            : Result.Fail(new ResultFailOrder(id, "Order not found.", ResultFailOrder.OperationId.OrderNotFound));
    }

    public void AddOrder(Order order)
    {
        orderRepository.AddOrder(order);
    }

    public void UpdateOrderStatus(Guid id, OrderStatus status)
    {
        var order = GetOrderById(id);

        if (order.IsSuccess)
        {
            order.Value.Status = status;
        }
    }
}

