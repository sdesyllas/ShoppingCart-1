using AutoMapper;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Mappers;
using ShoppingCart.Shared.Model;
using SimpleFixture;
using System.Threading.Tasks;

namespace ShoppingCart.Shared.UnitTests.Mappers
{
    [TestClass]
    public class ProductDtoResolverTests
    {
        private Fixture fixture;
        [TestInitialize]
        public void Initialize()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void ProductDtoResolverResolvesDto()
        {
            // Arrange
            var cartItem = fixture.Generate<CartItem>();

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<CartProductDto>(It.IsAny<Product>()))
                .Returns<Product>(x => fixture.Generate<CartProductDto>(constraints: new { ID = x.ID }));

            var mapperProviderMock = new Mock<IMapperProvider<Product, CartProductDto>>();
            mapperProviderMock.Setup(x => x.Provide())
                .Returns(mapperMock.Object);

            var productRepositoryMock = new Mock<IQueryableByIdRepository<Product>>();
            productRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>()))
                .Returns<long>(x => Task.FromResult(fixture.Generate<Product>(constraints: new { Identifier = x })));

            var resolver = new ProductDtoResolver(productRepositoryMock.Object, mapperProviderMock.Object);

            // Act
            var result = resolver.Resolve(cartItem, null, null, null);

            // Assert
            new CartItemDtoValidator(cartItem).ValidateAndThrow(result);
        }
    }
}
