using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Controllers;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using ShoppingCart.UnitTests.Controllers.Validators;
using SimpleFixture;
using System.Threading.Tasks;

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
        public async Task TestGetForNotExistingBasket()
        {
            // Arrange
            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = await controller.Get("cart1");

            // Assert
            new NotFoundResultValidator("Cart cart1 not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task TestGetForExistingBasket()
        {
            // Arrange
            var cart = Task.FromResult(fixture.Generate<Cart>());

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            cartReposioryMock
                .Setup(m => m.GetByName(cart.Result.Name))
                .Returns(cart);

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = await controller.Get(cart.Result.Name);

            // Assert
            new CartResultValidator(cart.Result.Name).ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task TestPutForEmptyRequestBody()
        {
            // Arrange
            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = await controller.Put("cart1", null);

            // Assert
            new BadRequestResultValidator("Empty body").ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task TestPutForNotExistingBasket()
        {
            // Arrange
            var body = fixture.Generate<CartItem>(constraints: new { Quantity = fixture.Generate<int>(constraints: new { min = 1 }) });

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = await controller.Put("cart1", body);

            // Assert
            new NotFoundResultValidator("Cart cart1 not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task TestPutForNotExistingProduct()
        {
            // Arrange
            var body = fixture.Generate<CartItem>(constraints: new { Quantity = fixture.Generate<int>(constraints: new { min = 1 }) });
            var cart = Task.FromResult(fixture.Generate<Cart>());

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            cartReposioryMock
                .Setup(m => m.GetByName(cart.Result.Name))
                .Returns(cart);

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = await controller.Put(cart.Result.Name, body);

            // Assert
            new NotFoundResultValidator($"Product with id { body.ID } not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task TestPutForNotEnoughQuantitiy()
        {
            // Arrange
            var body = fixture.Generate<CartItem>(constraints: new { Quantity = 10 });
            var cart = Task.FromResult(fixture.Generate<Cart>());
            var product = Task.FromResult(fixture.Generate<Product>(constraints: new { Stock = 5 }));

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            cartReposioryMock
                .Setup(m => m.GetByName(cart.Result.Name))
                .Returns(cart);

            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            productReposioryMock
                .Setup(m => m.GetById(body.ID))
                .Returns(product);

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = await controller.Put(cart.Result.Name, body);

            // Assert
            new BadRequestResultValidator("Not enough quantity").ValidateAndThrow(response);
        }
        [TestMethod]
        public async Task TestPutForNegativeQuantity()
        {
            // Arrange
            var body = fixture.Generate<CartItem>(constraints: new { Quantity = -1 });

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = await controller.Put(string.Empty, body);

            // Assert
            new BadRequestResultValidator("Invalid quantity").ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task TestPutForSuccess()
        {
            // Arrange
            var body = fixture.Generate<CartItem>(constraints: new { Quantity = 10 });
            var cart = Task.FromResult(fixture.Generate<Cart>());
            var product = Task.FromResult(fixture.Generate<Product>(constraints: new { Stock = 10 }));

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            cartReposioryMock
                .Setup(m => m.GetByName(cart.Result.Name))
                .Returns(cart);

            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            productReposioryMock
                .Setup(m => m.GetById(body.ID))
                .Returns(product);

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = await controller.Put(cart.Result.Name, body);

            // Assert
            new OkResultValidator("Product added").ValidateAndThrow(response);
            CollectionAssert.Contains(cart.Result.Items, body);
        }

        //TODO test quantity aggeregation
        //Put to closed basket
    }
}
