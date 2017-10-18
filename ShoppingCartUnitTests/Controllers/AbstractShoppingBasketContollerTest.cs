using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Controllers;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using SimpleFixture;

namespace ShoppingCart.UnitTests.Controllers
{
    public abstract class AbstractShoppingBasketContollerTest
    {
        protected Fixture Fixture { get; private set; }
        protected Mock<ICartRepository> CartReposioryMock { get; private set; }
        protected Mock<IQueryableByIdRepository<Product>> ProductReposioryMock { get; private set; }
        protected Mock<IMapperProvider<Cart, CartDto>> MapperProviderMock { get; private set; }
        protected Mock<IMapperProvider<AddCartItemDto, CartItem>> AddCartItemMapperProviderMock { get; private set; }
        protected Mock<ILogger<ShoppingBasketController>> LoggerMock { get; private set; }
        protected Mock<IMapper> CartMapperMock { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            Fixture = new Fixture();
            CartReposioryMock = new Mock<ICartRepository>();
            ProductReposioryMock = new Mock<IQueryableByIdRepository<Product>>();
            MapperProviderMock = new Mock<IMapperProvider<Cart, CartDto>>();
            AddCartItemMapperProviderMock = new Mock<IMapperProvider<AddCartItemDto, CartItem>>();
            LoggerMock = new Mock<ILogger<ShoppingBasketController>>();
            CartMapperMock = new Mock<IMapper>();
        }

        public ShoppingBasketController InitController()
        {
            return new ShoppingBasketController(CartReposioryMock.Object,
                ProductReposioryMock.Object,
                MapperProviderMock.Object,
                AddCartItemMapperProviderMock.Object,
                LoggerMock.Object);
        }
    }
}
