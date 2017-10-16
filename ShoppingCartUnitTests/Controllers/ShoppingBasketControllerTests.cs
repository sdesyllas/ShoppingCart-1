using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Controllers;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using ShoppingCartUnitTests.Controllers.Validators;
using System.Collections.Generic;

namespace ShoppingCartUnitTests.Controllers
{
    [TestClass]
    public class ShoppingBasketControllerTests
    {
        [TestMethod]
        public void TestGetForNotExistingBasket()
        {
            // Arrange
            var reposioryMock = new Mock<IRepository<Cart>>();
            var controller = new ShoppingBasketController(reposioryMock.Object);

            // Act
            var response = controller.Get("cart1");

            // Assert
            new NotFoundResultValidator("Cart cart1 not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public void TestGetForExistingBasket()
        {
            // Arrange
            var reposioryMock = new Mock<IRepository<Cart>>();
            reposioryMock
                .Setup(m => m.GetByName("test"))
                .Returns(new Cart("test", new List<CartItem>()));
            var controller = new ShoppingBasketController(reposioryMock.Object);

            // Act
            var response = controller.Get("test");

            // Assert
            new CartResultValidator("test").ValidateAndThrow(response);
        }
    }
}
