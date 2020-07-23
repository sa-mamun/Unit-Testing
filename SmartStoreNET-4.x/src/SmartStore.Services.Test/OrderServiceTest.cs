using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using Shouldly;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.Orders;
using SmartStore.Services.Orders;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SmartStore.Services.Test
{
    [ExcludeFromCodeCoverage]
    [Category("DevSkill")]
    public class OrderServiceTest
    {
        private AutoMock _mock;
        private Mock<IRepository<Order>> _orderRepositoryMock;
        private IOrderService _orderService;

        [OneTimeSetUp]
        public void ClassSetUp()
        {
            _mock = AutoMock.GetLoose();
        }

        [OneTimeTearDown]
        public void ClassCleanUp()
        {
            _mock?.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _orderRepositoryMock = _mock.Mock<IRepository<Order>>();
            _orderService = _mock.Create<OrderService>();
        }

        [TearDown]
        public void Clean()
        {
            _orderRepositoryMock.Reset();
        }

        [Test]
        public void InsertOrder_NullBookObject_ReturnExceptions()
        {
            //var order = new Order();
            //Act
            Should.Throw<ArgumentNullException>(() =>
                _orderService.InsertOrder(null)
            );

        }

        [Test]
        public void InsertOrder_BookObject_InsertBook()
        {
            //Arrange
            var order = new Order
            {
                OrderNumber = "101",
                OrderDiscount = 200,
                Id = 1
            };

            var orderToMacth = new Order
            {
                Id = 1
            };

            _orderRepositoryMock.Setup(x => x.Insert(It.Is<Order>(y => y.Id == orderToMacth.Id))).Verifiable();

            //Act
            _orderService.InsertOrder(order);

            //Assert
            _orderRepositoryMock.VerifyAll();
        }
    }
}
