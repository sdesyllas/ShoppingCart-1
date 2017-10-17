using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Mappers;
using ShoppingCart.Shared.Model;
using SimpleFixture;

namespace ShoppingCart.Shared.UnitTests.Mappers
{
    [TestClass]
    public class ProductMapperProviderTests
    {
        private Fixture fixture;

        [TestInitialize]
        public void Initialize()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void Should_ReturnValidDto()
        {
            // Arrange
            var product = fixture.Generate<Product>();
            var mapper = new ProductMapperProvider().Provide();

            // Act
            var result = mapper.Map<CartProductDto>(product);

            // Assert
            result.Description.Should().Be(product.Description);
            result.ID.Should().Be(product.ID);
            result.Name.Should().Be(product.Name);
            result.Price.Should().Be(product.Price);
        }

        [TestMethod]
        public void Should_ReturnNull_When_NullObjectProvided()
        {
            // Arrange
            var product = fixture.Generate<Product>();
            var mapper = new ProductMapperProvider().Provide();

            // Act
            var result = mapper.Map<CartProductDto>(null);

            // Assert
            result.Should().BeNull();
        }
    }
}
