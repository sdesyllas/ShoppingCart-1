using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Controllers;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using SimpleFixture;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCart.UnitTests.Controllers
{
    [TestClass]
    public class ShoppingBasketControllerGetTests
    {

        private Fixture fixture;
        private Mock<IRepository<Cart>> cartReposioryMock;
        private Mock<IQueryableByIdRepository<Product>> productReposioryMock;
        private Mock<IMapperProvider<Cart, CartDto>> mapperProviderMock;
        private Mock<IMapper> mapperMock;

        [TestInitialize]
        public void Initialize()
        {
            fixture = new Fixture();
            cartReposioryMock = new Mock<IRepository<Cart>>();
            productReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            mapperProviderMock = new Mock<IMapperProvider<Cart, CartDto>>();
            mapperMock = new Mock<IMapper>();
        }

        [TestMethod]
        public async Task Should_Return400_When_BasketDoesNotExist()
        {
            // Arrange
            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object);

            // Act
            var response = await controller.Get("cart1");

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404)
               .AssertMessage("Cart cart1 not found");
        }

        [TestMethod]
        public async Task Should_Reutn500_When_ProductFromBasketDoesNotExistInRepository()
        {
            cartReposioryMock
                .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(fixture.Generate<Cart>()));

            mapperMock.Setup(x => x.Map<CartDto>(It.IsAny<Cart>()))
                .Returns(new CartDto() { Items = new List<CartItemDto>() { new CartItemDto() { Product = null } } });

            mapperProviderMock.Setup(x => x.Provide())
                .Returns(mapperMock.Object);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object);

            // Act
            var response = await controller.Get("cart1");

            // Assert
            response.AssertResponseType<ObjectResult>(500)
                .AssertMessage("Inconsistent database state");
        }

        [TestMethod]
        public async Task Should_ReturnBasket_When_BasketExists()
        {
            // Arrange
            var cart = Task.FromResult(fixture.Generate<Cart>());

            cartReposioryMock
                .Setup(m => m.GetByNameAsync(cart.Result.Name))
                .Returns(cart);

            foreach (var item in cart.Result.Items)
                productReposioryMock
                    .Setup(m => m.GetByIdAsync(item.ID))
                    .Returns(Task.FromResult(fixture.Generate<Product>(constraints: new { Identifier = item.ID })));
            
            mapperMock.Setup(m => m.Map<CartDto>(cart.Result))
                .Returns(new CartDto() { Name = cart.Result.Name } );

            
            mapperProviderMock.Setup(m => m.Provide())
                .Returns(mapperMock.Object);

            var controller = new ShoppingBasketController(cartReposioryMock.Object,
                productReposioryMock.Object,
                mapperProviderMock.Object);

            // Act
            var response = await controller.Get(cart.Result.Name);

            // Assert
            response.AssertResponseType<OkObjectResult>(200)
                .Subject.Value.Should().BeAssignableTo<CartDto>()
                .Which.Name.Should().Be(cart.Result.Name);
        }
    }
}
