using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Controllers;
using ShoppingCart.Shared;
using ShoppingCart.UnitTests.Controllers.Validators;
using SimpleFixture;
using System.Threading.Tasks;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using CartDto = ShoppingCart.Shared.Dto.Cart;
using CartModel = ShoppingCart.Shared.Model.Cart;
using CartItemDto = ShoppingCart.Shared.Dto.CartItem;
using ProductDto = ShoppingCart.Shared.Dto.Product;
using ProductModel = ShoppingCart.Shared.Model.Product;
using System.Linq;

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
            var cartReposioryMock = new Mock<IRepository<CartModel>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<ProductModel>>();

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
            var cart = Task.FromResult(fixture.Generate<CartModel>());

            var cartReposioryMock = new Mock<IRepository<CartModel>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<ProductModel>>();
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
            var cartReposioryMock = new Mock<IRepository<CartModel>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<ProductModel>>();

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
            var body = fixture.Generate<AddCartItem>(constraints: new { Quantity = fixture.Generate<int>(constraints: new { min = 1 }) });

            var cartReposioryMock = new Mock<IRepository<CartModel>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<ProductModel>>();

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
            var body = fixture.Generate<AddCartItem>(constraints: new { Quantity = fixture.Generate<int>(constraints: new { min = 1 }) });
            var cart = Task.FromResult(fixture.Generate<CartModel>());

            var cartReposioryMock = new Mock<IRepository<CartModel>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<ProductModel>>();
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
            var body = fixture.Generate<AddCartItem>(constraints: new { Quantity = 10 });
            var cart = Task.FromResult(fixture.Generate<CartModel>());
            var product = Task.FromResult(fixture.Generate<ProductModel>(constraints: new { Stock = 5 }));

            var cartReposioryMock = new Mock<IRepository<CartModel>>();
            cartReposioryMock
                .Setup(m => m.GetByName(cart.Result.Name))
                .Returns(cart);

            var productReposioryMock = new Mock<IQueryableByIdRepository<ProductModel>>();
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
            var body = fixture.Generate<AddCartItem>(constraints: new { Quantity = -1 });

            var cartReposioryMock = new Mock<IRepository<CartModel>>();
            var productReposioryMock = new Mock<IQueryableByIdRepository<ProductModel>>();
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
            var body = fixture.Generate<AddCartItem>(constraints: new { Quantity = 10 });
            var cart = Task.FromResult(fixture.Generate<CartModel>());
            var product = Task.FromResult(fixture.Generate<ProductModel>(constraints: new { Stock = 10 }));

            var cartReposioryMock = new Mock<IRepository<CartModel>>();
            cartReposioryMock
                .Setup(m => m.GetByName(cart.Result.Name))
                .Returns(cart);

            var productReposioryMock = new Mock<IQueryableByIdRepository<ProductModel>>();
            productReposioryMock
                .Setup(m => m.GetById(body.ID))
                .Returns(product);

            var controller = new ShoppingBasketController(cartReposioryMock.Object, productReposioryMock.Object);

            // Act
            var response = await controller.Put(cart.Result.Name, body);

            // Assert
            new OkResultValidator("Product added").ValidateAndThrow(response);
            Assert.IsTrue(cart.Result.Items.Any(x => x.ID == body.ID && x.Quantity == body.Quantity));
            NUnit.Framework.Assert.That(cart.Result.Items, NUnit.Framework.Has.Member(body).Using(new AddCartItemToCartItemModelComparer()));
        }

        //TODO test quantity aggeregation
        //Put to closed basket
    }
}
