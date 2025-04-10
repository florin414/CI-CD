
using Bogus;
using DeliverX.Entities;
using DeliverX.Interfaces;
using DeliverX.Services;
using Moq;

namespace XUnit.UnitTests.Moq
{
    public static class TestDataFactory
    {
        private static readonly Faker _faker = new Faker();

        public static Order CreateOrder(Guid? id = null, OrderStatus? status = null)
        {
            return new Order
            {
                Id = id ?? Guid.NewGuid(),
                CustomerName = _faker.Name.FullName(),
                Address = _faker.Address.StreetAddress(),
                Status = status ?? _faker.PickRandom<OrderStatus>(),
                OrderDate = _faker.Date.Recent()
            };
        }

        public static OrderService CreateOrderServiceWithFakeData()
        {
            var orders = new List<Order>
            {
                CreateOrder(),
                CreateOrder(status: OrderStatus.Delivered),
                CreateOrder(status: OrderStatus.Canceled)
            };

            var mockRepo = new Mock<IOrderRepository>();
            mockRepo.Setup(repo => repo.GetAllOrders()).Returns(orders);
            mockRepo.Setup(repo => repo.GetOrderById(It.IsAny<Guid>())).Returns<Guid>(id => orders.Find(o => o.Id == id));
            mockRepo.Setup(repo => repo.AddOrder(It.IsAny<Order>())).Callback<Order>(order => orders.Add(order));

            return new OrderService(mockRepo.Object);
        }
    }
}
