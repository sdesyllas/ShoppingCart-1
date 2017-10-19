using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Repository.Exceptions;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using SimpleFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Repository.UnitTests
{
    [TestClass]
    public class InMemoryCartRepositoryTests
    {
        private Mock<IDataProvider<Cart>> dataProvider;
        private Fixture fixture;

        [TestInitialize]
        public void Initialize()
        {
            dataProvider = new Mock<IDataProvider<Cart>>();
            fixture = new Fixture();
        }

        [TestMethod]
        [ExpectedException(typeof(CartNotFoundException))]
        public async Task GetByNameAsync_When_ProductNotFound_Then_ExceptionIsThrown()
        {
            // Arrange
            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(Enumerable.Empty<Cart>());

            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            await repository.GetAsync(x => x.Name == string.Empty);

            // Assert exception
        }

        [TestMethod]
        public async Task GetByNameAsync_When_ProductNotFound_Then_ProductIsReturned()
        {
            // Arrange
            var cart = fixture.Generate<Cart>();
            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(new List<Cart>() { cart });

            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            var result = await repository.GetAsync(x => x.Name == cart.Name);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(cart);
        }

        [TestMethod]
        [ExpectedException(typeof(CartNotFoundException))]
        public async Task AddItemToCart_When_CartNotFound_Then_ExceptionIsThrown()
        {
            // Arrange
            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(Enumerable.Empty<Cart>());

            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            await repository.AddItemToCartAsync(string.Empty, (x) => Task.FromResult(null as Product), null);

            // Assert Exception
        }

        [TestMethod]
        [ExpectedException(typeof(CartCheckedOutException))]
        public async Task AddItemToCart_When_CartIsCheckedOut_Then_ExceptionIsThrown()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, true);
            var cart = fixture.Generate<Cart>();

            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(new List<Cart> { cart });

            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            await repository.AddItemToCartAsync(cart.Name, (x) => Task.FromResult(null as Product), null);

            //Assert Exception
        }

        [TestMethod]
        [ExpectedException(typeof(ProdcutNotFoundException))]
        public async Task AddItemToCart_When_ProductNotFound_Then_ExceptionIsThrown()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            var cart = fixture.Generate<Cart>();

            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(new List<Cart> { cart });

            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            await repository.AddItemToCartAsync(cart.Name, (x) => throw new ProdcutNotFoundException(), fixture.Generate<CartItem>());

            //Assert Exception
        }

        [TestMethod]
        [ExpectedException(typeof(NotEnoughStockException))]
        public async Task AddItemToCart_When_ProductOutOfStock_Then_ExceptionIsThrown()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            var cart = fixture.Generate<Cart>();
            var cartItem = fixture.Generate<CartItem>();

            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(new List<Cart> { cart });
            
            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            await repository.AddItemToCartAsync(cart.Name,
                (x) => Task.FromResult(fixture.Generate<Product>(constraints: new { Stock = cartItem.Quantity - 1 })),
                cartItem);

            // Assert Exception
        }

        [TestMethod]
        public async Task AddItemToCart_When_DataIsValid_Then_ItemIsAdded()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            var cart = fixture.Generate<Cart>();
            var cartItem = fixture.Generate<CartItem>();

            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(new List<Cart> { cart });

            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            await repository.AddItemToCartAsync(cart.Name,
                (x) => Task.FromResult(fixture.Generate<Product>(constraints: new { Stock = cartItem.Quantity })),
                cartItem);

            // Assert
            cart.Items.Should().Contain(cartItem);
        }

        [TestMethod]
        [ExpectedException(typeof(CartNotFoundException))]
        public async Task CheckoutAsync_When_CartNotFound_Then_ExceptionsIsThrown()
        {
            // Arrange
            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(Enumerable.Empty<Cart>());

            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            await repository.CheckoutAsync(string.Empty, null);

            // Assert Exception
        }

        [TestMethod]
        [ExpectedException(typeof(CartCheckedOutException))]
        public async Task CheckoutAsync_When_CartIsCheckedOut_Then_ExceptionsIsThrown()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, true);
            var cart = fixture.Generate<Cart>();

            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(new List<Cart> { cart });

            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            await repository.CheckoutAsync(cart.Name, null);

            // Assert Exception
        }

        [TestMethod]
        [ExpectedException(typeof(ProdcutNotFoundException))]
        public async Task CheckoutAsync_When_ProductNotFound_Then_ExceptionsIsThrown()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            var cart = fixture.Generate<Cart>();

            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(new List<Cart> { cart });

            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            await repository.CheckoutAsync(cart.Name, (x) => throw new ProdcutNotFoundException());

            // Assert Exception
        }

        [TestMethod]
        [ExpectedException(typeof(NotEnoughStockException))]
        public async Task CheckoutAsync_When_InsufficientStock_Then_ExceptionsIsThrown()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            fixture.Customize<CartItem>().Set(x => x.ProductId, 2);
            fixture.Customize<CartItem>().Set(x => x.Quantity, 10);

            var cart = fixture.Generate<Cart>();

            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(new List<Cart> { cart });

            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            await repository.CheckoutAsync(cart.Name,
                (x) => Task.FromResult(fixture.Generate<Product>(constraints: new { Stock = 15 })));
        }

        [TestMethod]
        public async Task CheckoutAsync_When_CartIsValid_Then_CartIsCheckedOut()
        {
            // Arrange
            fixture.Customize<Cart>().Set(x => x.IsCheckedOut, false);
            fixture.Customize<CartItem>().Set(x => x.ProductId, 2);
            fixture.Customize<CartItem>().Set(x => x.Quantity, 10);
            fixture.Customize<Product>().Set(x => x.Stock, int.MaxValue);

            var cart = fixture.Generate<Cart>();
            var product = fixture.Generate<Product>();

            dataProvider.Setup(x => x.ProvideAsync())
                .ReturnsAsync(new List<Cart> { cart });

            var repository = new InMemoryCartRepository(dataProvider.Object);

            // Act
            await repository.CheckoutAsync(cart.Name, (x) => Task.FromResult(product));

            // Assert
            cart.IsCheckedOut.Should().BeTrue();
            product.Stock.Should().Be(int.MaxValue - cart.Items.Sum(x => x.Quantity));
        }
    }
}
