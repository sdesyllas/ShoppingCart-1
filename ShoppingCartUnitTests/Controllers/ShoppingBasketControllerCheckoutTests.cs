using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Repository;
using ShoppingCart.Shared.Model;
using System;
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
            CartReposioryMock
                .Setup(x => x.CheckoutAsync(It.IsAny<string>(), It.IsAny<Func<long, Task<Product>>>()))
                .Throws(new NotEnoughStockException());

            var controller = InitController();

            // Act
            var response = await controller.CheckoutAsync(string.Empty);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400).AssertMessage("Items out of stock");
        }

        [TestMethod]
        public async Task Should_Return400_When_BasketAlreadyCheckedOut()
        {
            // Arrange
            CartReposioryMock
                .Setup(x => x.CheckoutAsync(It.IsAny<string>(), It.IsAny<Func<long, Task<Product>>>()))
                .Throws(new CartCheckedOutException());

            var controller = InitController();

            // Act
            var response = await controller.CheckoutAsync(string.Empty);

            // Assert
            response.AssertResponseType<BadRequestObjectResult>(400).AssertMessage("Cart is checked out");
        }

        [TestMethod]
        public async Task Should_Return404_When_ProductInBasketDoesNotExistInRepository()
        {
            // Arrange
            CartReposioryMock
                .Setup(x => x.CheckoutAsync(It.IsAny<string>(), It.IsAny<Func<long, Task<Product>>>()))
                .Throws(new ProdcutNotFoundException());

            var controller = InitController();

            // Act
            var response = await controller.CheckoutAsync(string.Empty);

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404)
                .AssertMessage("Cart product not found");
        }

        [TestMethod]
        public async Task Should_Return404_When_BasketDoesNotExist()
        {
            // Arrange
            CartReposioryMock
                .Setup(x => x.CheckoutAsync(It.IsAny<string>(), It.IsAny<Func<long, Task<Product>>>()))
                .Throws(new CartNotFoundException());

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
            CartReposioryMock
                .Setup(x => x.CheckoutAsync(It.IsAny<string>(), It.IsAny<Func<long, Task<Product>>>()))
                .Returns(Task.CompletedTask);

            var controller = InitController();

            // Act
            var response = await controller.CheckoutAsync(string.Empty);

            // Assert
            response.AssertResponseType<OkObjectResult>(200).AssertMessage("Cart checked out");
        }
    }
}
