using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Repository;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using SimpleFixture;
using System;
using System.Threading.Tasks;

namespace ShoppingCart.UnitTests.Controllers
{
    [TestClass]
    public class ShoppingBasketControllerPutTests : AbstractShoppingBasketContollerTest
    {
        [TestMethod]
        public async Task Should_Return400_When_NoBody()
        {
            // Arrange
            var controller = InitController();

            // Act
            var response = await controller.PutAsync("cart1", null);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400)
                .AssertMessage("Empty body");
        }

        [TestMethod]
        public async Task Should_Return404_When_NoBasketFound()
        {
            // Arrange
            Fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var body = Fixture.Generate<AddCartItemDto>(constraints: new { Quantity = Fixture.Generate<int>(constraints: new { min = 1 }) });
            InitMocks(new CartNotFoundException());

            var controller = InitController();

            // Act
            var response = await controller.PutAsync("cart1", body);

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404)
                .AssertMessage("Cart not found");
        }

        [TestMethod]
        public async Task Shoudl_Return404_When_ProductNotFound()
        {
            // Arrange
            Fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var body = Fixture.Generate<AddCartItemDto>(constraints: new { Quantity = Fixture.Generate<int>(constraints: new { min = 1 }) });
            var cart = Task.FromResult(Fixture.Generate<Cart>());
            InitMocks(new ProdcutNotFoundException());

            var controller = InitController();

            // Act
            var response = await controller.PutAsync(cart.Result.Name, body);

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404)
                .AssertMessage($"Product with id { body.ProductId } not found");
        }

        [TestMethod]
        public async Task Should_Return400_When_QuanitiyIsLargerThanStock()
        {
            // Arrange
            Fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var body = Fixture.Generate<AddCartItemDto>(constraints: new { Quantity = 10 });
            var cart = Task.FromResult(Fixture.Generate<Cart>());
            var product = Task.FromResult(Fixture.Generate<Product>(constraints: new { Stock = 5 }));

            InitMocks(new NotEnoughStockException());
            var controller = InitController();

            // Act
            var response = await controller.PutAsync(cart.Result.Name, body);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400)
                .AssertMessage("Not enough stock");
        }

        [TestMethod]
        public async Task Should_Return400_When_QuantityLowerThan0()
        {
            // Arrange
            Fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var body = Fixture.Generate<AddCartItemDto>(constraints: new { Quantity = -1 });

            var controller = InitController();

            // Act
            var response = await controller.PutAsync(string.Empty, body);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400)
                .AssertMessage("Invalid quantity");
        }

        [TestMethod]
        public async Task Should_Return400_When_BasketIsCheckedOut()
        {
            // Arrange
            Fixture.Customize<AddCartItemDto>().Set(x => x.Quantity, 1);
            var body = Fixture.Generate<AddCartItemDto>();

            InitMocks(new CartCheckedOutException());
            var controller = InitController();

            // Act
            var response = await controller.PutAsync("name", body);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400)
                .AssertMessage("Cart is checked out");
        }

        [TestMethod]
        public async Task Should_Return200_When_CorrectDataProvided()
        {
            // Arrange
            Fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var body = Fixture.Generate<AddCartItemDto>(constraints: new { Quantity = 10 });
            var cart = Fixture.Generate<Cart>();

            CartReposioryMock
                .Setup(m => m.AddItemToCart(cart.Name, It.IsAny<Func<long, Task<Product>>>(), It.IsAny<CartItem>()))
                .Callback<string, Func<long, Task<Product>>, CartItem>((n, f, i) => cart.Items.Add(i))
                .Returns(Task.CompletedTask);

            AddCartItemMapperProviderMock.Setup(x => x.Provide())
                .Returns(new MapperConfiguration(cfg => cfg.CreateMap<AddCartItemDto, CartItem>()).CreateMapper());

            var controller = InitController();

            // Act
                var response = await controller.PutAsync(cart.Name, body);

            // Assert
            var cartResponse = response.AssertResponseType<OkObjectResult>(200)
                .AssertMessage("Product added");

            cart.Items.Should().Contain(x => x.ProductId == body.ProductId && x.Quantity == body.Quantity);
        }

        private void InitMocks(Exception cartReposioryMockExpectedException)
        {
            CartReposioryMock
                .Setup(m => m.AddItemToCart(It.IsAny<string>(), It.IsAny<Func<long, Task<Product>>>(), It.IsAny<CartItem>()))
                .Throws(cartReposioryMockExpectedException);

            AddCartItemMapperProviderMock.Setup(x => x.Provide())
                .Returns(new MapperConfiguration(cfg => cfg.CreateMap<AddCartItemDto, CartItem>()).CreateMapper());
        }
    }
}
