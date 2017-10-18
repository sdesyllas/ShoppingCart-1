using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Repository;
using ShoppingCart.Repository.Exceptions;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using SimpleFixture;
using System.Threading.Tasks;

namespace ShoppingCart.UnitTests.Controllers
{
    [TestClass]
    public class ShoppingBasketControllerGetTests : AbstractShoppingBasketContollerTest
    {
        [TestMethod]
        public async Task Should_Return404_When_BasketDoesNotExist()
        {
            // Arrange
            CartReposioryMock.Setup(x => x.GetByNameAsync(It.IsAny<string>()))
                .ThrowsAsync(new CartNotFoundException());
            var controller = InitController();

            // Act
            var response = await controller.GetAsync("cart1");

            // Assert
            response.AssertResponseType<NotFoundObjectResult>(404)
               .AssertMessage("Cart not found");
        }

        [TestMethod]
        public async Task Should_Reutn500_When_ProductFromBasketDoesNotExistInRepository()
        {
            CartReposioryMock
                .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(Fixture.Generate<Cart>());

            CartMapperMock.Setup(x => x.Map<CartDto>(It.IsAny<Cart>()))
                .Throws(new ProdcutNotFoundException());

            MapperProviderMock.Setup(x => x.Provide())
                .Returns(CartMapperMock.Object);

            var controller = InitController();

            // Act
            var response = await controller.GetAsync("cart1");

            // Assert
            response.AssertResponseType<ObjectResult>(500)
                .AssertMessage("Inconsistent database");
        }

        [TestMethod]
        public async Task Should_ReturnBasket_When_BasketExists()
        {
            // Arrange
            var cart = Task.FromResult(Fixture.Generate<Cart>());

            CartReposioryMock
                .Setup(m => m.GetByNameAsync(cart.Result.Name))
                .Returns(cart);

            foreach (var item in cart.Result.Items)
                ProductReposioryMock
                    .Setup(m => m.GetByIdAsync(item.ProductId))
                    .ReturnsAsync(Fixture.Generate<Product>(constraints: new { Identifier = item.ProductId }));
            
            CartMapperMock.Setup(m => m.Map<CartDto>(cart.Result))
                .Returns(new CartDto() { Name = cart.Result.Name } );
            
            MapperProviderMock.Setup(m => m.Provide())
                .Returns(CartMapperMock.Object);

            var controller = InitController();

            // Act
            var response = await controller.GetAsync(cart.Result.Name);

            // Assert
            response.AssertResponseType<OkObjectResult>(200)
                .Which.Value.Should().BeAssignableTo<CartDto>()
                .Which.Name.Should().Be(cart.Result.Name);
        }
    }
}
