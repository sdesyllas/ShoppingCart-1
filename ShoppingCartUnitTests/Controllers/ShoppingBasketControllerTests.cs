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
            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Get("cart1");

            // Assert
            new NotFoundResultValidator("Cart cart1 not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public void TestGetForExistingBasket()
        {
            // Arrange
            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            cartReposioryMock
                .Setup(m => m.GetByName("test"))
                .Returns(new Cart("test", new List<CartItem>()));
            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Get("test");

            // Assert
            new CartResultValidator("test").ValidateAndThrow(response);
        }

        [TestMethod]
        public void TestPutForEmptyRequestBody()
        {
            // Arrange
            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Put("cart1", null);

            // Assert
            new BadRequestResultValidator("Empty body").ValidateAndThrow(response);
        }

        [TestMethod]
        public void TestPutForNotExistingBasket()
        {
            // Arrange
            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            var bodyMock = new Mock<CartItem>();
            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Put("cart1", bodyMock.Object);

            // Assert
            new NotFoundResultValidator("Cart cart1 not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public void TestPutForNotExistingProduct()
        {
            // Arrange
            var body = new CartItem()
            {
                ID = 10
            };
            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            cartReposioryMock
                .Setup(m => m.GetByName("cart1"))
                .Returns(new Cart("cart1", new List<CartItem>()));
            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Put("cart1", body);

            // Assert
            new NotFoundResultValidator($"Product with id { body.ID } not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public void TestPutForNotEnoughQuantitiy()
        {
            // Arrange
            var body = new CartItem()
            {
                ID = 10,
                Quantity = 100
            };
            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            cartReposioryMock
                .Setup(m => m.GetByName("cart1"))
                .Returns(new Cart("cart1", new List<CartItem>()));
            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Put("cart1", body);

            // Assert
            new BadRequestResultValidator("Not enough quantity").ValidateAndThrow(response);
        }

        [TestMethod]
        public void TestPutForSuccess()
        {
            // Arrange
            var body = new CartItem()
            {
                ID = 10,
                Quantity = 100
            };
            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            cartReposioryMock
                .Setup(m => m.GetByName("cart1"))
                .Returns(new Cart("cart1", new List<CartItem>()));
            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Put("cart1", body);

            // Assert
            new OkResultValidator("Product added").ValidateAndThrow(response);
        }

        //TODO test quantity aggeregation
        //TODO test quantity less or equal to "0"
    }
}
