using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using SimpleFixture;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCart.UnitTests.Controllers
{
    [TestClass]
    public class ShoppingBasketControllerCheckoutTests : AbstractShoppingBasketContollerTest
    {
        [TestMethod]
        public async Task Should_Return400_When_ItemOutOfStock()
        {
            // Arrange
            Fixture.Customize<Product>().Set(x => x.Stock, 10);
            Fixture.Customize<CartItem>().Set(x => x.Quantity, 11);
            Fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);

            var cart = Fixture.Generate<Cart>();

            CartReposioryMock.Setup(x => x.GetByNameAsync(cart.Name))
                .Returns(Task.FromResult(cart));

            ProductReposioryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>()))
                .Returns<long>(x => Task.FromResult(Fixture.Generate<Product>(constraints: new { Identifier = x })));

            var controller = InitController();

            // Act
            var response = await controller.CheckoutAsync(cart.Name);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400).AssertMessage("Items out of stock");
        }

        [TestMethod]
        public async Task Should_Return400_When_ItemAggeregatedOutOfStock()
        {
            // Arrange
            Fixture.Customize<Product>().Set(x => x.Stock, 10);
            Fixture.Customize<CartItem>().Set(x => x.Quantity, 6).Set(x => x.ProductId, 2);
            Fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            Fixture.Customize<Cart>().Set(x => x.Items, new List<CartItem>()
            {
                Fixture.Generate<CartItem>(),
                Fixture.Generate<CartItem>()
            });

            var cart = Fixture.Generate<Cart>();

            CartReposioryMock.Setup(x => x.GetByNameAsync(cart.Name))
                .ReturnsAsync(cart);

            ProductReposioryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>()))
                .Returns<long>(x => Task.FromResult(Fixture.Generate<Product>(constraints: new { Identifier = x })));

            var controller = InitController();

            // Act
            var response = await controller.CheckoutAsync(cart.Name);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400).AssertMessage("Items out of stock");
        }

        [TestMethod]
        public async Task Should_Return400_When_BasketAlreadyCheckedOut()
        {
            // Arrange
            Fixture.Customize<Cart>().Set(x => x.IsCheckedOut, true);

            var cart = Fixture.Generate<Cart>();

            CartReposioryMock.Setup(x => x.GetByNameAsync(cart.Name))
                .ReturnsAsync(cart);

            var controller = InitController();

            var response = await controller.CheckoutAsync(cart.Name);

            response.AssertResponseType<BadRequestObjectResult>(400).AssertMessage("Cart is checked out");
        }

        [TestMethod]
        public async Task Should_Return404_When_ProductInBasketDoesNotExistInRepository()
        {
            // Arrange
            Fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            var cart = Task.FromResult(Fixture.Generate<Cart>());

            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            CartReposioryMock
                .Setup(m => m.GetByNameAsync(cart.Result.Name))
                .Returns(cart);

            var controller = InitController();

            // Act
            var response = await controller.CheckoutAsync(cart.Result.Name);

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404)
                .AssertMessage("Cart product not found");
        }

        [TestMethod]
        public async Task Should_Return404_When_BasketDoesNotExist()
        {
            // Arrange
            var controller = InitController();

            // Act
            var response = await controller.CheckoutAsync("cart1");

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404).AssertMessage("Cart not found");
        }

        [TestMethod]
        public async Task Should_Return200AndReduceStocks_When_CheckedOut()
        {
            // Arrange
            Fixture.Customize<Product>().Set(x => x.Stock, 10).Set(x => x.Id, 2);
            var product = Fixture.Generate<Product>();

            Fixture.Customize<CartItem>().Set(x => x.Quantity, 4).Set(x => x.ProductId, product.Id);
            Fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            Fixture.Customize<Cart>().Set(x => x.Items, new List<CartItem>()
            {
                Fixture.Generate<CartItem>(),
                Fixture.Generate<CartItem>()
            });

            var cart = Fixture.Generate<Cart>();
            

            CartReposioryMock.Setup(x => x.GetByNameAsync(cart.Name))
                .ReturnsAsync(cart);
            ProductReposioryMock.Setup(x => x.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            var controller = InitController();

            // Act
            var response = await controller.CheckoutAsync(cart.Name);

            // Assert
            response.AssertResponseType<OkObjectResult>(200).AssertMessage("Cart checked out");
        }
    }
}
