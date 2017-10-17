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
    public class ShoppingBasketControllerCheckoutTests
    {

        private Fixture fixture;
        private Mock<IRepository<Cart>> cartReposioryMock;
        private Mock<IQueryableByIdRepository<Product>> productReposioryMock;
        private Mock<IMapperProvider<Cart, CartDto>> mapperProviderMock;
        private Mock<ILogger<ShoppingBasketController>> loggerMock;
        private Mock<IMapper> mapperMock;

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
        public async Task Should_Return400_When_ItemOutOfStock()
        {
            // Arrange
            fixture.Customize<Product>().Set(x => x.Stock, 10);
            fixture.Customize<CartItem>().Set(x => x.Quantity, 11);
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var cart = fixture.Generate<Cart>();

            cartReposioryMock.Setup(x => x.GetByNameAsync(cart.Name))
                .Returns(Task.FromResult(cart));

            productReposioryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>()))
                .Returns<long>(x => Task.FromResult(fixture.Generate<Product>(constraints: new { Identifier = x })));

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.Checkout(cart.Name);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400).AssertMessage("Items out of stock");
        }

        [TestMethod]
        public async Task Should_Return400_When_ItemAggeregatedOutOfStock()
        {
            // Arrange
            fixture.Customize<Product>().Set(x => x.Stock, 10);
            fixture.Customize<CartItem>().Set(x => x.Quantity, 6).Set(x => x.ProductId, 2);
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            fixture.Customize<Cart>().Set(x => x.Items, new List<CartItem>()
            {
                fixture.Generate<CartItem>(),
                fixture.Generate<CartItem>()
            });

            var cart = fixture.Generate<Cart>();

            cartReposioryMock.Setup(x => x.GetByNameAsync(cart.Name))
                .ReturnsAsync(cart);

            productReposioryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>()))
                .Returns<long>(x => Task.FromResult(fixture.Generate<Product>(constraints: new { Identifier = x })));

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.Checkout(cart.Name);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400).AssertMessage("Items out of stock");
        }

        [TestMethod]
        public async Task Should_Return400_When_BasketAlreadyCheckedOut()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, true);

            var cart = fixture.Generate<Cart>();

            cartReposioryMock.Setup(x => x.GetByNameAsync(cart.Name))
                .ReturnsAsync(cart);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            var response = await controller.Checkout(cart.Name);

            response.AssertResponseType<BadRequestObjectResult>(400).AssertMessage("Cart is checked out");
        }

        [TestMethod]
        public async Task Should_Return404_When_ProductInBasketDoesNotExistInRepository()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            var cart = Task.FromResult(fixture.Generate<Cart>());

            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            cartReposioryMock
                .Setup(m => m.GetByNameAsync(cart.Result.Name))
                .Returns(cart);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.Checkout(cart.Result.Name);

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404)
                .AssertMessage("Cart product not found");
        }

        [TestMethod]
        public async Task Should_Return404_When_BasketDoesNotExist()
        {
            // Arrange
            var controller = new ShoppingBasketController(
                cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.Checkout("cart1");

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404).AssertMessage("Cart not found");
        }

        [TestMethod]
        public async Task Should_Return200AndReduceStocks_When_CheckedOut()
        {
            // Arrange
            fixture.Customize<Product>().Set(x => x.Stock, 10).Set(x => x.Id, 2);
            var product = fixture.Generate<Product>();

            fixture.Customize<CartItem>().Set(x => x.Quantity, 4).Set(x => x.ProductId, product.Id);
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            fixture.Customize<Cart>().Set(x => x.Items, new List<CartItem>()
            {
                fixture.Generate<CartItem>(),
                fixture.Generate<CartItem>()
            });

            var cart = fixture.Generate<Cart>();
            

            cartReposioryMock.Setup(x => x.GetByNameAsync(cart.Name))
                .ReturnsAsync(cart);
            productReposioryMock.Setup(x => x.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.Checkout(cart.Name);

            // Assert
            response.AssertResponseType<OkObjectResult>(200).AssertMessage("Cart checked out");
            cart.IsCheckedOut.Should().BeTrue();
            product.Stock.Should().Be(2);
        }
    }
}
