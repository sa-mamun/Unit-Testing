using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using Shouldly;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.Customers;
using SmartStore.Services.Customers;
using SmartStore.Services.Payments;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SmartStore.Services.Test
{
    [ExcludeFromCodeCoverage]
    [Category("DevSkill")]
    public class CustomerServicesTests
    {
        private AutoMock _mock;
        private Mock<IRepository<Customer>> _customerRepository;
        private ICustomerService _customerService;

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
            _customerRepository = _mock.Mock<IRepository<Customer>>();
            _customerService = _mock.Create<CustomerService>();
        }

        [TearDown]
        public void Clean()
        {
            _customerRepository.Reset();
        }

        [Test]
        public void InsertCustomer_EmailAlreadyExists_ThrowException()
        {
            //Arrange
            var customer = new Customer
            {
                Email = "sam@gmail.com",
                Username = "sam"
            };

            //Act
            Should.Throw<SmartException>(() =>
                _customerService.InsertCustomer(customer)
            );

            //Assert
        }
    }
}
