using Bogus;
using DeliverX.Entities;
using DeliverX.Interfaces;
using DeliverX.Services;
using NSubstitute;

namespace XUnit.UnitTests.NSubstitute
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

            var mockRepo = Substitute.For<IOrderRepository>();
            mockRepo.GetAllOrders().Returns(orders);
            mockRepo.GetOrderById(Arg.Any<Guid>()).Returns(callInfo => orders.FirstOrDefault(o => o.Id == callInfo.Arg<Guid>()));
            mockRepo.AddOrder(Arg.Any<Order>());

            return new OrderService(mockRepo);
        }
    }
}
