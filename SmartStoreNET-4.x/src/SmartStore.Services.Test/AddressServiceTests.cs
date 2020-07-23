using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.Common;
using SmartStore.Services.Common;
using System.Diagnostics.CodeAnalysis;

namespace SmartStore.Services.Test
{
    [ExcludeFromCodeCoverage]
    [Category("DevSkill")]
    public class AddressServiceTests
    {
        private AutoMock _mock;
        private Mock<IRepository<Address>> _addressRepositoryMock;
        private AddressService _addressService;

        [OneTimeSetUp]
        public void ClassSetup()
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
            _addressRepositoryMock = _mock.Mock<IRepository<Address>>();
            _addressService = _mock.Create<AddressService>();
        }

        [TearDown]
        public void Clean()
        {
            _addressRepositoryMock.Reset();
        }

        [Test]
        public void GetAddressById_AddressIdExists_ReturnsAddress()
        {
            //Arrange
            var address = new Address
            {
                Id = 1,
                Address1 = "Dhaka"
            };

            var AddressIdToMacth = 1;

            _addressRepositoryMock.Setup(x => x.GetById(It.Is<Address>(y => y.Id == AddressIdToMacth))).Returns(address).Verifiable();

            //Act
            _addressService.GetAddressById(AddressIdToMacth);


            //Assert
            _addressRepositoryMock.VerifyAll();
        }

        [Test]
        public void DeleteAddress_AddressIdNotNull_DeleteAddress()
        {
            //Arrange
            var id = 1;

            var address = new Address
            {
                Id = 1,
                City = "Dhaka",
                Email = "abc@gmail.com"
            };

            _addressRepositoryMock.Setup(x => x.GetById(It.Is<Address>(y => y.Id == id))).Returns(address).Verifiable();
            _addressRepositoryMock.Setup(x => x.Delete(It.Is<Address>(y => y.Id == y.Id))).Verifiable();

            //Act
            _addressService.DeleteAddress(id);


            //Assert
            _addressRepositoryMock.VerifyAll();
        }
    }
}