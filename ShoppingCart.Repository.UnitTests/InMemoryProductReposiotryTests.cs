using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Repository.Exceptions;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using SimpleFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Repository.UnitTests
{
    [TestClass]
    public class InMemoryProductReposiotryTests
    {
        [TestMethod]
        [ExpectedException(typeof(ProdcutNotFoundException))]
        public async Task GetByIdAsync_When_ProductNotFound_Then_ExceptionIsThrown()
        {
            // Arrange
            var dataProvider = new Mock<IDataProvider<Product>>();
            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(Enumerable.Empty<Product>());

            var repository = new InMemoryProductReposiotry(dataProvider.Object);

            // Act
            await repository.GetByIdAsync(0);

            // Assert exception
        }

        [TestMethod]
        public async Task GetByIdAsync_When_ProductIsFound_Then_ProductIsReturned()
        {
            var product = new Fixture().Generate<Product>();
            var dataProvider = new Mock<IDataProvider<Product>>();
            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(new List<Product>() { product });

            var repository = new InMemoryProductReposiotry(dataProvider.Object);

            // Act
            await repository.GetByIdAsync(product.Id);

            // Assert
            product.Should().NotBeNull();
            product.Should().Be(product);
        }

        [TestMethod]
        [ExpectedException(typeof(ProdcutNotFoundException))]
        public async Task GetByNameAsync_When_ProductNotFound_Then_ExceptionIsThrown()
        {
            var dataProvider = new Mock<IDataProvider<Product>>();
            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(Enumerable.Empty<Product>());

            var repository = new InMemoryProductReposiotry(dataProvider.Object);

            // Act
            await repository.GetByNameAsync(string.Empty);

            // Assert exception
        }

        [TestMethod]
        public async Task GetByNameAsync_When_ProductNotFound_Then_ProductIsReturned()
        {
            var product = new Fixture().Generate<Product>();
            var dataProvider = new Mock<IDataProvider<Product>>();
            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(new List<Product>() { product });

            var repository = new InMemoryProductReposiotry(dataProvider.Object);

            // Act
            var result = await repository.GetByNameAsync(product.Name);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(product);
        }
    }
}
