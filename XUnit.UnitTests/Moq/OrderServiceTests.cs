using DeliverX.Entities;
using DeliverX.Interfaces;
using DeliverX.Results;
using DeliverX.Services;
using Moq;

namespace XUnit.UnitTests.Moq
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockRepo;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockRepo = new Mock<IOrderRepository>();
            _orderService = new OrderService(_mockRepo.Object);
        }

        [Theory]
        [InlineData(OrderStatus.InProgress, true)]
        [InlineData(OrderStatus.Delivered, true)]
        [InlineData(OrderStatus.Canceled, true)]
        public void When_OrderExists_Expect_ResultOkWithOrder(OrderStatus status, bool expectedResult)
        {
            var orderId = Guid.NewGuid();
            var order = TestDataFactory.CreateOrder(orderId, status);

            SetupGetOrderById(orderId, order);

            var result = _orderService.GetOrderById(orderId);

            Assert.Equal(expectedResult, result.IsSuccess);
            Assert.Equal(orderId, result.Value.Id);
        }

        [Theory]
        [InlineData(OrderStatus.InProgress, false)]
        [InlineData(OrderStatus.Delivered, false)]
        [InlineData(OrderStatus.Canceled, false)]
        public void When_OrderDoesNotExist_Expect_ResultFailWithError(OrderStatus status, bool expectedResult)
        {
            var nonExistentId = Guid.NewGuid();
            SetupGetOrderByIdNotFound(nonExistentId);

            var result = _orderService.GetOrderById(nonExistentId);

            Assert.Equal(expectedResult, result.IsSuccess);
            Assert.IsType<ResultFailOrder>(result.Errors.First());
        }

        [Fact]
        public void When_OrderIsAdded_Expect_RepositoryAddCalled()
        {
            var newOrder = TestDataFactory.CreateOrder();

            _orderService.AddOrder(newOrder);

            _mockRepo.Verify(repo => repo.AddOrder(It.IsAny<Order>()), Times.Once);
        }

        [Theory]
        [InlineData(OrderStatus.InProgress)]
        [InlineData(OrderStatus.Delivered)]
        [InlineData(OrderStatus.Canceled)]
        public void When_OrderExists_Expect_StatusUpdated(OrderStatus newStatus)
        {
            var orderId = Guid.NewGuid();
            var order = TestDataFactory.CreateOrder(orderId, OrderStatus.InProgress);
            SetupGetOrderById(orderId, order);

            _orderService.UpdateOrderStatus(orderId, newStatus);

            _mockRepo.Verify(repo => repo.GetOrderById(orderId), Times.Once);
        }

        [Fact]
        public void When_OrderDoesNotExist_Expect_StatusNotUpdated()
        {
            var nonExistentId = Guid.NewGuid();
            SetupGetOrderByIdNotFound(nonExistentId);

            _orderService.UpdateOrderStatus(nonExistentId, OrderStatus.Canceled);

            _mockRepo.Verify(repo => repo.GetOrderById(nonExistentId), Times.Once);
        }

        private void SetupGetOrderById(Guid orderId, Order order)
        {
            _mockRepo.Setup(repo => repo.GetOrderById(orderId)).Returns(order);
        }

        private void SetupGetOrderByIdNotFound(Guid orderId)
        {
            _mockRepo.Setup(repo => repo.GetOrderById(orderId)).Returns((Order)null);
        }
    }
}
