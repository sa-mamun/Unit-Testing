using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using Shouldly;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.News;
using SmartStore.Services.News;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace SmartStore.Services.Test
{
    [ExcludeFromCodeCoverage]
    [Category("DevSkill")]
    public class NewsServiceTests
    {
        private AutoMock _mock;
        private Mock<IRepository<NewsItem>> _newsItemRepository;
        private INewsService _newsService;

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
            _newsItemRepository = _mock.Mock<IRepository<NewsItem>>();
            _newsService = _mock.Create<NewsService>();
        }

        [TearDown]
        public void Clean()
        {
            _newsItemRepository.Reset();
        }

        [Test]
        public void UpdateNews_NewsItemNotExists_ThrowsException()
        {
            //Arrange
            //var newsItem = new NewsItem
            //{
            //    LanguageId = 1,
            //    Title = "abc"
            //};

            //Act
            Should.Throw<ArgumentNullException>(() =>
                _newsService.UpdateNews(null)
            );

            //Assert

        }

        [Test]
        public void UpdateNews_NewsItemObject_UpdateNewsItem()
        {
            //Arrange
            var newsItem = new NewsItem
            {
                LanguageId = 1,
                Title = "abc"
            };

            _newsItemRepository.Setup(x => x.Update(It.Is<NewsItem>(y => y.Id == y.Id))).Verifiable();

            //Act
            _newsService.UpdateNews(newsItem);

            //Assert
            _newsItemRepository.VerifyAll();
        }
    }
}
