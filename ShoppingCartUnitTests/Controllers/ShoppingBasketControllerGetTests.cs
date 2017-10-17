using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Controllers;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using SimpleFixture;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCart.UnitTests.Controllers
{
    [TestClass]
    public class ShoppingBasketControllerGetTests
    {

        private Fixture fixture;
        private Mock<IRepository<Cart>> cartReposioryMock;
        private Mock<IQueryableByIdRepository<Product>> productReposioryMock;
        private Mock<IMapperProvider<Cart, CartDto>> mapperProviderMock;
        private Mock<IMapper> mapperMock;
        private Mock<ILogger<ShoppingBasketController>> loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            fixture = new Fixture();
            cartReposioryMock = new Mock<IRepository<Cart>>();
            productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            mapperProviderMock = new Mock<IMapperProvider<Cart, CartDto>>();
            loggerMock = new Mock<ILogger<ShoppingBasketController>>();
            mapperMock = new Mock<IMapper>();
        }

        [TestMethod]
        public async Task Should_Return404_When_BasketDoesNotExist()
        {
            // Arrange
            mapperProviderMock.Setup(x => x.Provide())
                .Returns(mapperMock.Object);
            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.GetAsync("cart1");

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404)
               .AssertMessage("Cart not found");
        }

        [TestMethod]
        public async Task Should_Reutn500_When_ProductFromBasketDoesNotExistInRepository()
        {
            cartReposioryMock
                .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(fixture.Generate<Cart>()));

            mapperMock.Setup(x => x.Map<CartDto>(It.IsAny<Cart>()))
                .Returns(new CartDto() { Items = new List<CartItemDto>() { new CartItemDto() { Product = null } } });

            mapperProviderMock.Setup(x => x.Provide())
                .Returns(mapperMock.Object);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.GetAsync("cart1");

            // Assert
            response.AssertResponseType<ObjectResult>(500)
                .AssertMessage("Inconsistent database");
        }

        [TestMethod]
        public async Task Should_ReturnBasket_When_BasketExists()
        {
            // Arrange
            var cart = Task.FromResult(fixture.Generate<Cart>());

            cartReposioryMock
                .Setup(m => m.GetByNameAsync(cart.Result.Name))
                .Returns(cart);

            foreach (var item in cart.Result.Items)
                productReposioryMock
                    .Setup(m => m.GetByIdAsync(item.ProductId))
                    .Returns(Task.FromResult(fixture.Generate<Product>(constraints: new { Identifier = item.ProductId })));
            
            mapperMock.Setup(m => m.Map<CartDto>(cart.Result))
                .Returns(new CartDto() { Name = cart.Result.Name } );

            
            mapperProviderMock.Setup(m => m.Provide())
                .Returns(mapperMock.Object);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.GetAsync(cart.Result.Name);

            // Assert
            response.AssertResponseType<OkObjectResult>(200)
                .Which.Value.Should().BeAssignableTo<CartDto>()
                .Which.Name.Should().Be(cart.Result.Name);
        }
    }
}
