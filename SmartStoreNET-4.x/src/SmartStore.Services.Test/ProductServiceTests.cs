using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using Shouldly;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.Catalog;
using SmartStore.Core.Domain.Orders;
using SmartStore.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace SmartStore.Services.Test
{
    [ExcludeFromCodeCoverage]
    [Category("DevSkill")]
    public class ProductServiceTests
    {
        private AutoMock _mock;
        private Mock<IRepository<Product>> _productRepositoryMock;
        private Mock<IRepository<ShoppingCartItem>> _shoppingCartItemRepositoryMock;
        private Mock<IRepository<ProductBundleItem>> _productBundleItemRepositoryMock;
        private Mock<IRepository<RelatedProduct>> _relatedProductRepositoryMock;
        private Mock<IDbContext> _dbContextMock;
        private IProductService _productService;

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
            _productRepositoryMock = _mock.Mock<IRepository<Product>>();
            _productBundleItemRepositoryMock = _mock.Mock<IRepository<ProductBundleItem>>();
            _shoppingCartItemRepositoryMock = _mock.Mock<IRepository<ShoppingCartItem>>();
            _relatedProductRepositoryMock = _mock.Mock<IRepository<RelatedProduct>>();
            _dbContextMock = _mock.Mock<IDbContext>();
            _productService = _mock.Create<ProductService>();
        }

        [TearDown]
        public void Clean()
        {
            _productRepositoryMock.Reset();
            _productBundleItemRepositoryMock.Reset();
            _shoppingCartItemRepositoryMock.Reset();
            _relatedProductRepositoryMock.Reset();
        }

        [Test]
        public void GetProductByName_ProductNameNull_ReturnNull()
        {
            //Arrange

            //Act
            var product = _productService.GetProductByName(null);

            //Assert
            product.ShouldBeNull();
        }

        [Test]
        public void GetProductByName_ProductName_ReturnProduct()
        {
            //Arrange
            var product = new List<Product>
            {
                new Product{ Id = 1, Name = "Mobile", Deleted = false },
                new Product{ Id = 2, Name = "Tv", Deleted = false },
                new Product{ Id = 3, Name = "Desktop", Deleted = false },
                new Product{ Id = 4, Name = "Laptop", Deleted = false },
            }.AsQueryable();

            var productToMatch = new Product
            {
                Id = 2,
                Name = "Tv",
                Deleted = false
            };

            //var productToMatch2 = new Product
            //{
            //    Id = 6,
            //    Name = "Phone",
            //    Deleted = false
            //};

            _productRepositoryMock.Setup(x => x.Table).Returns(product).Verifiable();

            //Act
            var productToReturn = _productService.GetProductByName(productToMatch.Name);
            productToReturn.ShouldBe(productToMatch);

            //Assert
            _productRepositoryMock.VerifyAll();
        }

        [Test]
        public void GetProductsByIds_ProductIdLists_ReturnsProductList()
        {
            //Arrange
            var productIdList = new int[] { 2, 1, 4 };

            var product = new List<Product>
            {
                new Product{ Id = 1, Name = "Mobile", Deleted = false },
                new Product{ Id = 2, Name = "Tv", Deleted = false },
                new Product{ Id = 3, Name = "Desktop", Deleted = false },
                new Product{ Id = 4, Name = "Laptop", Deleted = false },
            }.AsQueryable();

            var productToMatch = new List<Product>
            {
                new Product{ Id = 2, Name = "Tv", Deleted = false },
                new Product{ Id = 1, Name = "Mobile", Deleted = false },
                new Product{ Id = 4, Name = "Laptop", Deleted = false },
            }.AsQueryable();

            _productRepositoryMock.Setup(x => x.Table).Returns(product).Verifiable();

            //Act
            var finalResult = _productService.GetProductsByIds(productIdList, 0);

            //Assert
            finalResult.ShouldBe(productToMatch);
            _productRepositoryMock.VerifyAll();
        }

        [Test]
        public void DeleteBundleItem_ParentCartItemExixts_DeleteBundleItem()
        {
            //Arrange
            var untrackedCartItem = new List<ShoppingCartItem>
            {
                new ShoppingCartItem{ Id = 1, ParentItemId = 1, BundleItemId = 1, Quantity = 1 },
                new ShoppingCartItem{ Id = 2, ParentItemId = 2, BundleItemId = 2, Quantity = 2 },
                new ShoppingCartItem{ Id = 3, ParentItemId = 1, BundleItemId = 1, Quantity = 3 },
                new ShoppingCartItem{ Id = 4, ParentItemId = 4, BundleItemId = 4, Quantity = 4 }
            }.AsQueryable();

            var productBundleItem = new ProductBundleItem
            {
                Id = 1,
                Quantity = 2
            };

            var shoppingCartItem = new ShoppingCartItem 
            { 
                Id = 3, ParentItemId = 1, BundleItemId = 1, Quantity = 3 
            };

            _shoppingCartItemRepositoryMock.Setup(x => x.TableUntracked).Returns(untrackedCartItem).Verifiable();
            _shoppingCartItemRepositoryMock.Setup(x => x.Table).Returns(untrackedCartItem).Verifiable();
            _shoppingCartItemRepositoryMock.Setup(x => x.Delete(It.Is<ShoppingCartItem>(y => y.Id == shoppingCartItem.Id))).Verifiable();
            _productBundleItemRepositoryMock.Setup(x => x.Delete(It.Is<ProductBundleItem>(y => y.Id == productBundleItem.Id))).Verifiable();


            //Act
            _productService.DeleteBundleItem(productBundleItem);


            //Assert
            _shoppingCartItemRepositoryMock.VerifyAll();
            _productBundleItemRepositoryMock.VerifyAll();
        }

        [Test]
        public void GetRelatedProductsByProductId1_Product1Exists_RetrunRelatedPrdouctsList()
        {
            //Arrange
            var product = new List<Product>
            {
                new Product{ Id = 1, Name = "Mobile", Deleted = false, Published = true },
                new Product{ Id = 2, Name = "Tv", Deleted = false, Published = true  },
                new Product{ Id = 3, Name = "Desktop", Deleted = false, Published = true  },
                new Product{ Id = 4, Name = "Laptop", Deleted = false, Published = true  },
                new Product{ Id = 5, Name = "Xyz", Deleted = false, Published = true  }
            }.AsQueryable();

            var relatedProduct = new List<RelatedProduct>
            {
                new RelatedProduct{ Id = 1, ProductId1 = 1, ProductId2 = 2, DisplayOrder = 1},
                new RelatedProduct{ Id = 2, ProductId1 = 2, ProductId2 = 1, DisplayOrder = 3},
                new RelatedProduct{ Id = 3, ProductId1 = 1, ProductId2 = 4, DisplayOrder = 4},
                new RelatedProduct{ Id = 4, ProductId1 = 1, ProductId2 = 3, DisplayOrder = 2}
            }.AsQueryable();

            var resultToMatch = new List<RelatedProduct>
            {
                new RelatedProduct{ Id = 1, ProductId1 = 1, ProductId2 = 2, DisplayOrder = 1},
                new RelatedProduct{ Id = 4, ProductId1 = 1, ProductId2 = 3, DisplayOrder = 2},
                new RelatedProduct{ Id = 3, ProductId1 = 1, ProductId2 = 4, DisplayOrder = 4}
            };

            var produtcId1 = 1;

            _relatedProductRepositoryMock.Setup(x => x.Table).Returns(relatedProduct).Verifiable();
            _productRepositoryMock.Setup(x => x.Table).Returns(product).Verifiable();

            //Act
            var resultList = _productService.GetRelatedProductsByProductId1(produtcId1);


            //Assert
            resultList.ShouldBe(resultToMatch);
            _relatedProductRepositoryMock.VerifyAll();
            _productRepositoryMock.VerifyAll();
        }

        [Test]
        public void DeleteProduct_GroupedProductExists_DeleteSuccess()
        {
            //Arrange
            var productList = new List<Product>
            {
                new Product{ Id = 1, Name = "Mobile", Deleted = false, Published = true, ParentGroupedProductId = 1 },
                new Product{ Id = 2, Name = "Tv", Deleted = false, Published = true, ParentGroupedProductId = 3 },
                new Product{ Id = 3, Name = "Desktop", Deleted = false, Published = true, ParentGroupedProductId = 1 },
                new Product{ Id = 4, Name = "Laptop", Deleted = false, Published = true, ParentGroupedProductId = 2 },
                new Product{ Id = 5, Name = "Xyz", Deleted = false, Published = true, ParentGroupedProductId = 4 }
            }.AsQueryable();

            var product = new Product
            {
                Id = 1,
                Name = "Mobile",
                Deleted = false,
                Published = true,
                ParentGroupedProductId = 1,
                ProductType = ProductType.GroupedProduct
            };

            _productRepositoryMock.Setup(x => x.Update(It.Is<Product>(y => y.Id == product.Id))).Verifiable();
            _productRepositoryMock.Setup(x => x.Table).Returns(productList).Verifiable();
            _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

            //Act
            _productService.DeleteProduct(product);

            //Assert
            _productRepositoryMock.VerifyAll();
            _dbContextMock.VerifyAll();
        }
    }
}
