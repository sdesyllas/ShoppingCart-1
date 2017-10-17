using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    public class ShoppingBasketController : Controller
    {
        private readonly IRepository<Cart> _cartsRepository;
        private readonly IQueryableByIdRepository<Product> _productsRepository;
        private readonly IMapper _cartMapper;
        private readonly ILogger _logger;

        public ShoppingBasketController(IRepository<Cart> cartsRepository,
            IQueryableByIdRepository<Product> productsRepository,
            IMapperProvider<Cart, CartDto> cartMapperProvider,
            ILogger<ShoppingBasketController> logger)
        {
            _cartsRepository = cartsRepository;
            _productsRepository = productsRepository;
            _cartMapper = cartMapperProvider.Provide();
            _logger = logger;
        }

        [HttpGet("{cartName}")]
        [SwaggerResponse(200, typeof(CartDto), "Cart exists")]
        [SwaggerResponse(404, typeof(ResultMessageDto), "Cart not found")]
        [SwaggerResponse(500, typeof(ResultMessageDto), "Cart contains item with invalid product")]
        public async Task<ActionResult> Get(string cartName)
        {
            _logger.LogDebug($"Get called with parameter: {cartName}");

            var cart = await _cartsRepository.GetByNameAsync(cartName);
            if (cart == null)
            {
                return NotFound(new ResultMessageDto($"Cart {cartName} not found"));
            }        

            var cartDto = _cartMapper.Map<CartDto>(cart);

            if (cartDto.Items != null && cartDto.Items.Any(x => x.Product == null))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResultMessageDto("Inconsistent database"));
            }

            return Ok(cartDto);
        }

        [HttpPut("{cartName}")]
        [SwaggerResponse(200, typeof(ResultMessageDto), "Product added")]
        [SwaggerResponse(400, typeof(ResultMessageDto), "Empty body / Invalid product quantity / Cart has beed checked out / Not enough stock")]
        [SwaggerResponse(404, typeof(ResultMessageDto), "Cart not found / Product not found")]
        public async Task<ActionResult> Put(string cartName, [FromBody] AddCartItemDto item)
        {
            _logger.LogDebug($"Put called with parameter: {cartName}");

            if (item == null)
            {
                return BadRequest(new ResultMessageDto("Empty body"));
            }

            if(item.Quantity <= 0)
            {
                return BadRequest(new ResultMessageDto("Invalid quantity"));
            }

            var cart = await _cartsRepository.GetByNameAsync(cartName);
            if(cart == null)
            {
                return NotFound(new ResultMessageDto($"Cart {cartName} not found"));
            }

            if (cart.IsCheckedOut)
            {
                return BadRequest(new ResultMessageDto("Cart is checked out"));
            }

            var product = await _productsRepository.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                return NotFound(new ResultMessageDto($"Product with id {item.ProductId} not found"));
            }

            if (product.Stock < item.Quantity)
            {
                return BadRequest(new ResultMessageDto("Not enough stock"));
            }

            CartItem model = new CartItem()
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };
            cart.Items.Add(model);

            return Ok(new ResultMessageDto("Product added"));
        }

        [HttpGet("{cartName}/Checkout")]
        [SwaggerResponse(200, typeof(ResultMessageDto), "Cart checked out")]
        [SwaggerResponse(400, typeof(ResultMessageDto), "Empty body / Invalid product quantity / Cart has beed checked out / Not enough stock")]
        [SwaggerResponse(404, typeof(ResultMessageDto), "Cart not found / Cart item product not found")]
        public async Task<ActionResult> Checkout(string cartName)
        {
            _logger.LogDebug($"Checkout called with parameter: {cartName}");

            var cart = await _cartsRepository.GetByNameAsync(cartName);
            if (cart == null)
            {
                return NotFound(new ResultMessageDto("Cart not found"));
            }

            if (cart.IsCheckedOut)
            {
                return BadRequest(new ResultMessageDto("Cart already checked out"));
            }

            var products = await GetProductsFromCartItems(cart);
            if (products.Any(x => x == null))
            {
                return NotFound(new ResultMessageDto("Cart product not found"));
            }

            var groupedItems = cart.Items
                .GroupBy(x => x.ProductId)
                .Select(x => new
                {
                    Id = x.Key,
                    CartSum = x.Sum(y => y.Quantity),
                    Product = products.First(prod => prod.Id == x.Key)
                })
                .ToList();

            if (groupedItems.Any(x => x.Product.Stock < x.CartSum))
            {
                return BadRequest(new ResultMessageDto("Items out of stock"));
            }

            groupedItems.ForEach(x => x.Product.Stock -= x.CartSum);
            cart.IsCheckedOut = true;

            return Ok(new ResultMessageDto("Cart checked out"));
        }

        private async Task<IEnumerable<Product>> GetProductsFromCartItems(Cart cart)
        {
            var productTasks = cart.Items
                .Select(x => x.ProductId)
                .Distinct()
                .Select(_productsRepository.GetByIdAsync);
            return await Task.WhenAll(productTasks);
        }
    }
}
