using FluentValidation;
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
        public void TestProductMapperProviderMapperMapsValue()
        {
            // Arrange
            var product = fixture.Generate<Product>();
            var mapper = new ProductMapperProvider().Provide();

            // Act
            var result = mapper.Map<CartProductDto>(product);

            // Assert
            new ProductDtoMappedValidator(product).ValidateAndThrow(result);
        }
    }
}
