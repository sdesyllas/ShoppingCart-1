using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Mappers;
using ShoppingCart.Shared.Model;
using SimpleFixture;
using System.Linq;

namespace ShoppingCart.Shared.UnitTests.Mappers
{
    [TestClass]
    public partial class CartMapperProviderTests
    {
        private Fixture fixture;
        [TestInitialize]
        public void Initialize()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void Should_ProvideValidMapper()
        {
            // Arrange
            var cart = fixture.Generate<Cart>();
            var productResolverMock = new Mock<IValueResolver<CartItem, CartItemDto, CartProductDto>>();
            productResolverMock.Setup(x => x.Resolve(It.IsAny<CartItem>(), It.IsAny<CartItemDto>(), It.IsAny<CartProductDto>(), It.IsAny<ResolutionContext>()))
                .Returns<CartItem, CartItemDto, CartProductDto, ResolutionContext>(MapCartProduct);

            var mapper = new CartMapperProvider(productResolverMock.Object).Provide();
            // Act
            var result = mapper.Map<CartDto>(cart);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(cart.Name);
            result.Items.Select(x => x.Product.Id).Should().BeEquivalentTo(cart.Items.Select(x => x.ProductId));
        }

        private CartProductDto MapCartProduct(CartItem ci,
            CartItemDto ciDto,
            CartProductDto cpDto,
            ResolutionContext rContext)
        {
            return fixture.Generate<CartProductDto>(constraints: new { Id = ci.ProductId });
        }
    }
}
