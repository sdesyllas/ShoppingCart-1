using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Controllers;
using ShoppingCart.Shared;
using ShoppingCart.UnitTests.Controllers.Validators;
using SimpleFixture;
using System.Threading.Tasks;
using ShoppingCart.Shared.Dto;
using System.Linq;
using ShoppingCart.Shared.Model;
using AutoMapper;
using System.Collections.Generic;

namespace ShoppingCart.UnitTests.Controllers
{
    [TestClass]
    public class ShoppingBasketControllerGetTests
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
            var mapperProvider = new Mock<IMapperProvider<Cart, CartDto>>();

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProvider.Object);

            // Act
            var response = await controller.Get("cart1");

            // Assert
            new NotFoundResultValidator("Cart cart1 not found").ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task TestGetForInvalidBasketProduct()
        {
            var cartReposioryMock = new Mock<IRepository<Cart>>();
            cartReposioryMock
                .Setup(x => x.GetByName(It.IsAny<string>()))
                .Returns(Task.FromResult(fixture.Generate<Cart>()));

            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            var mapperProvider = new Mock<IMapperProvider<Cart, CartDto>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<CartDto>(It.IsAny<Cart>()))
                .Returns(new CartDto() { Items = new List<CartItemDto>() { new CartItemDto() { Product = null } } });

            mapperProvider.Setup(x => x.Provide())
                .Returns(mapperMock.Object);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProvider.Object);

            // Act
            var response = await controller.Get("cart1");

            // Assert
            new InternalServerErrorValidator("Inconsistent database state").ValidateAndThrow(response);
        }

        [TestMethod]
        public async Task TestGetForExistingBasket()
        {
            // Arrange
            var cart = Task.FromResult(fixture.Generate<Cart>());

            var cartReposioryMock = new Mock<IRepository<Cart>>();
            cartReposioryMock
                .Setup(m => m.GetByName(cart.Result.Name))
                .Returns(cart);

            var productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            foreach (var item in cart.Result.Items)
                productReposioryMock
                    .Setup(m => m.GetById(item.ID))
                    .Returns(Task.FromResult(fixture.Generate<Product>(constraints: new { Identifier = item.ID })));
            
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<CartDto>(cart.Result))
                .Returns(new CartDto() { Name = cart.Result.Name } );

            var mapperProviderMock = new Mock<IMapperProvider<Cart, CartDto>>();
            mapperProviderMock.Setup(m => m.Provide())
                .Returns(mapperMock.Object);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object);

            // Act
            var response = await controller.Get(cart.Result.Name);

            // Assert
            new CartResultValidator(cart.Result.Name).ValidateAndThrow(response);
        }
    }
}
