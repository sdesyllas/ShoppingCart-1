using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Controllers;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using ShoppingCart.UnitTests.Controllers.Validators;
using SimpleFixture;
using System.Linq;
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

        [TestInitialize]
        public void Initialize()
        {
            fixture = new Fixture();
            cartReposioryMock = new Mock<IRepository<Cart>>();
            productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            mapperProviderMock = new Mock<IMapperProvider<Cart, CartDto>>();
        }

        [TestMethod]
        public async Task Should_Return400_When_NoBody()
        {
            // Arrange
            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object);

            // Act
            var response = await controller.Put("cart1", null);

            // Assert
            new BadRequestResultValidator("Empty body").ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task Should_Return404_When_NoBasketFound()
        {
            // Arrange
            var body = fixture.Generate<AddCartItemDto>(constraints: new { Quantity = fixture.Generate<int>(constraints: new { min = 1 }) });

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object);

            // Act
            var response = await controller.Put("cart1", body);

            // Assert
            new NotFoundResultValidator("Cart cart1 not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task Shoudl_Return404_When_ProductNotFound()
        {
            // Arrange
            var body = fixture.Generate<AddCartItemDto>(constraints: new { Quantity = fixture.Generate<int>(constraints: new { min = 1 }) });
            var cart = Task.FromResult(fixture.Generate<Cart>());

            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            cartReposioryMock
                .Setup(m => m.GetByNameAsync(cart.Result.Name))
                .Returns(cart);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object);

            // Act
            var response = await controller.Put(cart.Result.Name, body);

            // Assert
            new NotFoundResultValidator($"Product with id { body.ID } not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task Should_Return400_When_QuanitiyIsLargerThanStock()
        {
            // Arrange
            var body = fixture.Generate<AddCartItemDto>(constraints: new { Quantity = 10 });
            var cart = Task.FromResult(fixture.Generate<Cart>());
            var product = Task.FromResult(fixture.Generate<Product>(constraints: new { Stock = 5 }));

            cartReposioryMock
                .Setup(m => m.GetByNameAsync(cart.Result.Name))
                .Returns(cart);

            productReposioryMock
                .Setup(m => m.GetByIdAsync(body.ID))
                .Returns(product);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object);

            // Act
            var response = await controller.Put(cart.Result.Name, body);

            // Assert
            new BadRequestResultValidator("Not enough quantity").ValidateAndThrow(response);
        }
        [TestMethod]
        public async Task Should_Return400_When_QuantityLowerThan0()
        {
            // Arrange
            var body = fixture.Generate<AddCartItemDto>(constraints: new { Quantity = -1 });

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object);

            // Act
            var response = await controller.Put(string.Empty, body);

            // Assert
            new BadRequestResultValidator("Invalid quantity").ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task Should_Return200_When_CorrectDataProvided()
        {
            // Arrange
            var body = fixture.Generate<AddCartItemDto>(constraints: new { Quantity = 10 });
            var cart = Task.FromResult(fixture.Generate<Cart>());
            var product = Task.FromResult(fixture.Generate<Product>(constraints: new { Stock = 10 }));

            cartReposioryMock
                .Setup(m => m.GetByNameAsync(cart.Result.Name))
                .Returns(cart);

            productReposioryMock
                .Setup(m => m.GetByIdAsync(body.ID))
                .Returns(product);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object);

            // Act
            var response = await controller.Put(cart.Result.Name, body);

            // Assert
            new OkResultValidator("Product added").ValidateAndThrow(response);
            Assert.IsTrue(cart.Result.Items.Any(x => x.ID == body.ID && x.Quantity == body.Quantity));
            NUnit.Framework.Assert.That(cart.Result.Items, NUnit.Framework.Has.Member(body).Using(new AddCartItemToCartItemModelComparer()));
        }

        //TODO test quantity aggeregation
        //TODO Put to closed basket
    }
}
