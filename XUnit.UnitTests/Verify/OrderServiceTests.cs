using DeliverX.Entities;
using DeliverX.Interfaces;
using DeliverX.Services;
using Moq;
using XUnit.UnitTests.Moq;

namespace XUnit.UnitTests.Verify
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
        [InlineData(OrderStatus.InProgress)]
        [InlineData(OrderStatus.Delivered)]
        [InlineData(OrderStatus.Canceled)]
        public void When_OrderExists_Expect_ResultOkWithOrder(OrderStatus status)
        {
            var orderId = Guid.NewGuid();
            var order = TestDataFactory.CreateOrder(orderId, status);

            SetupGetOrderById(orderId, order);

            var result = _orderService.GetOrderById(orderId);

            VerifierHelper.VerifyResult(result.IsSuccess);
        }

        [Fact]
        public void When_OrderDoesNotExist_Expect_ResultFailWithError()
        {
            var nonExistentId = Guid.NewGuid();

            SetupGetOrderByIdNotFound(nonExistentId);

            var result = _orderService.GetOrderById(nonExistentId);

            var resultSnapshot = new
            {
                result.IsSuccess,
                Errors = result.Errors.Select(e => new { e.Message })
            };

            VerifierHelper.VerifyResult(resultSnapshot);
        }

        [Fact]
        public void When_OrderIsAdded_Expect_RepositoryAddCalled()
        {
            var newOrder = TestDataFactory.CreateOrder();

            _mockRepo.Setup(repo => repo.AddOrder(It.IsAny<Order>()))
                .Callback<Order>(order =>
                {
                    VerifierHelper.VerifyResult(new { order.Status }, callerClass: nameof(OrderServiceTests));
                });

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

            VerifierHelper.VerifyResult(new { order.Status });

            _orderService.UpdateOrderStatus(orderId, newStatus);

            _mockRepo.Verify(repo => repo.GetOrderById(orderId), Times.Once);

            var updatedOrder = TestDataFactory.CreateOrder(orderId, newStatus);

            VerifierHelper.VerifyResult(new { updatedOrder.Status });
        }

        [Fact]
        public void When_OrderDoesNotExist_Expect_StatusNotUpdated()
        {
            var nonExistentId = Guid.NewGuid();

            SetupGetOrderByIdNotFound(nonExistentId);

            VerifierHelper.VerifyResult(new { nonExistentId });

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
