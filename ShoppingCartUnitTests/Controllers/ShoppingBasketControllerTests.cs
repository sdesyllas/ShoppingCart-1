using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Controllers;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using ShoppingCart.UnitTests.Controllers.Validators;
using SimpleFixture;

namespace ShoppingCart.UnitTests.Controllers
{
    [TestClass]
    public class ShoppingBasketControllerTests
    {

        private Fixture fixture;

        [TestInitialize]
        public void Initialize()
        {
            fixture = new Fixture();
        }

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
            var cart = fixture.Generate<Cart>();

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            cartReposioryMock
                .Setup(m => m.GetByName(cart.Name))
                .Returns(cart);

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Get(cart.Name);

            // Assert
            new CartResultValidator(cart.Name).ValidateAndThrow(response);
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
            var body = fixture.Generate<CartItem>(constraints: new { Quantity = fixture.Generate<int>(constraints: new { min = 1 }) });

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Put("cart1", body);

            // Assert
            new NotFoundResultValidator("Cart cart1 not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public void TestPutForNotExistingProduct()
        {
            // Arrange
            var body = fixture.Generate<CartItem>(constraints: new { Quantity = fixture.Generate<int>(constraints: new { min = 1 }) });
            var cart = fixture.Generate<Cart>();

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            cartReposioryMock
                .Setup(m => m.GetByName(cart.Name))
                .Returns(cart);

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Put(cart.Name, body);

            // Assert
            new NotFoundResultValidator($"Product with id { body.ID } not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public void TestPutForNotEnoughQuantitiy()
        {
            // Arrange
            var body = fixture.Generate<CartItem>(constraints: new { Quantity = 10 });
            var cart = fixture.Generate<Cart>();
            var product = fixture.Generate<Product>(constraints: new { Stock = 5 });

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            cartReposioryMock
                .Setup(m => m.GetByName(cart.Name))
                .Returns(cart);

            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            productReposioryMock
                .Setup(m => m.GetById(body.ID))
                .Returns(product);

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Put(cart.Name, body);

            // Assert
            new BadRequestResultValidator("Not enough quantity").ValidateAndThrow(response);
        }
        [TestMethod]
        public void TestPutForNegativeQuantity()
        {
            // Arrange
            var body = fixture.Generate<CartItem>(constraints: new { Quantity = -1 });

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Put(string.Empty, body);

            // Assert
            new BadRequestResultValidator("Invalid quantity").ValidateAndThrow(response);
        }

        [TestMethod]
        public void TestPutForSuccess()
        {
            // Arrange
            var body = fixture.Generate<CartItem>(constraints: new { Quantity = 10 });
            var cart = fixture.Generate<Cart>();
            var product = fixture.Generate<Product>(constraints: new { Stock = 10 });

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            cartReposioryMock
                .Setup(m => m.GetByName(cart.Name))
                .Returns(cart);

            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            productReposioryMock
                .Setup(m => m.GetById(body.ID))
                .Returns(product);

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = controller.Put(cart.Name, body);

            // Assert
            new OkResultValidator("Product added").ValidateAndThrow(response);
            CollectionAssert.Contains(cart.Items, body);
        }

        //TODO test quantity aggeregation
    }
}
