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
using System.Threading.Tasks;

namespace ShoppingCart.UnitTests.Controllers
{
    [TestClass]
    public class ShoppingBasketControllerPutTests
    {
        private Fixture fixture;
        private Mock<IRepository<Cart>> cartReposioryMock;
        private Mock<IQueryableByIdRepository<Product>> productReposioryMock;
        private Mock<IMapperProvider<Cart, CartDto>> mapperProviderMock;
        private Mock<ILogger<ShoppingBasketController>> loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            fixture = new Fixture();
            cartReposioryMock = new Mock<IRepository<Cart>>();
            productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            mapperProviderMock = new Mock<IMapperProvider<Cart, CartDto>>();
            loggerMock = new Mock<ILogger<ShoppingBasketController>>();
        }

        [TestMethod]
        public async Task Should_Return400_When_NoBody()
        {
            // Arrange
            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.Put("cart1", null);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400)
                .AssertMessage("Empty body");
        }

        [TestMethod]
        public async Task Should_Return404_When_NoBasketFound()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var body = fixture.Generate<AddCartItemDto>(constraints: new { Quantity = fixture.Generate<int>(constraints: new { min = 1 }) });

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.Put("cart1", body);

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404)
                .AssertMessage("Cart cart1 not found");
        }

        [TestMethod]
        public async Task Shoudl_Return404_When_ProductNotFound()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var body = fixture.Generate<AddCartItemDto>(constraints: new { Quantity = fixture.Generate<int>(constraints: new { min = 1 }) });
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
            var response = await controller.Put(cart.Result.Name, body);

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404)
                .AssertMessage($"Product with id { body.ProductId } not found");
        }

        [TestMethod]
        public async Task Should_Return400_When_QuanitiyIsLargerThanStock()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var body = fixture.Generate<AddCartItemDto>(constraints: new { Quantity = 10 });
            var cart = Task.FromResult(fixture.Generate<Cart>());
            var product = Task.FromResult(fixture.Generate<Product>(constraints: new { Stock = 5 }));

            cartReposioryMock
                .Setup(m => m.GetByNameAsync(cart.Result.Name))
                .Returns(cart);

            productReposioryMock
                .Setup(m => m.GetByIdAsync(body.ProductId))
                .Returns(product);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.Put(cart.Result.Name, body);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400)
                .AssertMessage("Not enough stock");
        }

        [TestMethod]
        public async Task Should_Return400_When_QuantityLowerThan0()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var body = fixture.Generate<AddCartItemDto>(constraints: new { Quantity = -1 });

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.Put(string.Empty, body);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400)
                .AssertMessage("Invalid quantity");
        }

        [TestMethod]
        public async Task Should_Return400_When_BasketIsCheckedOut()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, true);
            fixture.Customize<AddCartItemDto>().Set(x => x.Quantity, 1);

            var body = fixture.Generate<AddCartItemDto>();
            var cart = Task.FromResult(fixture.Generate<Cart>());

            cartReposioryMock.Setup(x => x.GetByNameAsync(cart.Result.Name))
                .Returns(cart);
            
            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.Put(cart.Result.Name, body);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400)
                .AssertMessage("Cart is checked out");
        }

        [TestMethod]
        public async Task Should_Return200_When_CorrectDataProvided()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var body = fixture.Generate<AddCartItemDto>(constraints: new { Quantity = 10 });
            var cart = Task.FromResult(fixture.Generate<Cart>());
            var product = Task.FromResult(fixture.Generate<Product>(constraints: new { Stock = 10 }));

            cartReposioryMock
                .Setup(m => m.GetByNameAsync(cart.Result.Name))
                .Returns(cart);

            productReposioryMock
                .Setup(m => m.GetByIdAsync(body.ProductId))
                .Returns(product);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object,
                loggerMock.Object);

            // Act
            var response = await controller.Put(cart.Result.Name, body);

            // Assert
            var cartResponse = response.AssertResponseType<OkObjectResult>(200)
                .AssertMessage("Product added");

            cart.Result.Items.Should().Contain(x => x.ProductId == body.ProductId && x.Quantity == body.Quantity);
        }
    }
}
