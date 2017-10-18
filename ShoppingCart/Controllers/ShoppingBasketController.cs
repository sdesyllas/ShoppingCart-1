using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    public class ShoppingBasketController : Controller
    {
        private readonly IRepository<Cart> _cartsRepository;
        private readonly IQueryableByIdRepository<Product> _productsRepository;
        private readonly IMapper _cartMapper;
        private readonly IMapper _cartItemMapper;
        private readonly ILogger _logger;

        public ShoppingBasketController(IRepository<Cart> cartsRepository,
            IQueryableByIdRepository<Product> productsRepository,
            IMapperProvider<Cart, CartDto> cartMapperProvider,
            IMapperProvider<AddCartItemDto, CartItem> cartItemMapper,
            ILogger<ShoppingBasketController> logger)
        {
            _cartsRepository = cartsRepository;
            _productsRepository = productsRepository;
            _cartMapper = cartMapperProvider.Provide();
            _cartItemMapper = cartItemMapper.Provide();
            _logger = logger;
        }

        [HttpGet("{cartName}")]
        [SwaggerResponse(200, typeof(CartDto), "Cart exists")]
        [SwaggerResponse(404, typeof(ResultMessageDto), "Cart not found")]
        [SwaggerResponse(500, typeof(ResultMessageDto), "Cart contains item with invalid product")]
        public async Task<ActionResult> GetAsync(string cartName)
        {
            _logger.LogDebug($"Get called with parameter: {cartName}");

            var cart = await _cartsRepository.GetByNameAsync(cartName);
            var cartDto = _cartMapper.Map<CartDto>(cart);

            return ValidateForGetCart(cart, cartDto) ?? Ok(cartDto);
        }

        private ActionResult ValidateForGetCart(Cart cart, CartDto cartDto)
        {
            if (cart == null)
            {
                return NotFound(new ResultMessageDto("Cart not found"));
            }

            if (cartDto.Items != null && cartDto.Items.Any(x => x.Product == null))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResultMessageDto("Inconsistent database"));
            }

            return null;
        }

        [HttpPut("{cartName}")]
        [SwaggerResponse(200, typeof(ResultMessageDto), "Product added")]
        [SwaggerResponse(400, typeof(ResultMessageDto), "Empty body / Invalid product quantity / Cart has beed checked out / Not enough stock")]
        [SwaggerResponse(404, typeof(ResultMessageDto), "Cart not found / Product not found")]
        public async Task<ActionResult> PutAsync(string cartName, [FromBody] AddCartItemDto item)
        {
            _logger.LogDebug($"Put called with parameter: {cartName}");

            var addItemValidationResult = ValidateForAddItem(item);
            if(addItemValidationResult != null)
            {
                return addItemValidationResult;
            }

            var cart = await _cartsRepository.GetByNameAsync(cartName);
            var cartValidationResult = ValidateCart(cart);
            if(cartValidationResult != null)
            {
                return cartValidationResult;
            }

            var stockValidationResult = await ValidateStockAsync(item);
            if(stockValidationResult != null)
            {
                return stockValidationResult;
            }

            var model =_cartItemMapper.Map<CartItem>(item);
            cart.Items.Add(model);

            return Ok(new ResultMessageDto("Product added"));
        }

        private ActionResult ValidateCart(Cart cart)
        {
            if (cart == null)
            {
                return NotFound(new ResultMessageDto($"Cart not found"));
            }

            if (cart.IsCheckedOut)
            {
                return BadRequest(new ResultMessageDto("Cart is checked out"));
            }

            return null;
        }

        private ActionResult ValidateForAddItem(AddCartItemDto item)
        {
            if (item == null)
            {
                return BadRequest(new ResultMessageDto("Empty body"));
            }

            if (item.Quantity <= 0)
            {
                return BadRequest(new ResultMessageDto("Invalid quantity"));
            }

            return null;
        }

        private async Task<ActionResult> ValidateStockAsync(AddCartItemDto item)
        {
            var product = await _productsRepository.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                return NotFound(new ResultMessageDto($"Product with id {item.ProductId} not found"));
            }

            if (product.Stock < item.Quantity)
            {
                return BadRequest(new ResultMessageDto("Not enough stock"));
            }

            return null;
        }

        [HttpGet("{cartName}/Checkout")]
        [SwaggerResponse(200, typeof(ResultMessageDto), "Cart checked out")]
        [SwaggerResponse(400, typeof(ResultMessageDto), "Empty body / Invalid product quantity / Cart has beed checked out / Not enough stock")]
        [SwaggerResponse(404, typeof(ResultMessageDto), "Cart not found / Cart item product not found")]
        public async Task<ActionResult> CheckoutAsync(string cartName)
        {
            _logger.LogDebug($"Checkout called with parameter: {cartName}");

            var cart = await _cartsRepository.GetByNameAsync(cartName);
            var cartValidationResult = ValidateCart(cart);
            if (cartValidationResult != null)
            {
                return cartValidationResult;
            }

            var products = await GetProductsFromCartItems(cart.Items);
            var cartStockValidationResult = ValidateCartStock(cart, products);
            if (cartStockValidationResult != null)
            {
                return cartStockValidationResult;
            }

            Checkout(cart, products);

            return Ok(new ResultMessageDto("Cart checked out"));
        }

        private void Checkout(Cart cart, IEnumerable<Product> products)
        {
            foreach (var item in cart.Items)
            {
                products.First(p => p.Id == item.ProductId).Stock -= item.Quantity;
            }

            cart.IsCheckedOut = true;
        }

        private ActionResult ValidateCartStock(Cart cart, IEnumerable<Product> products)
        {
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

            return null;
        }

        private async Task<IEnumerable<Product>> GetProductsFromCartItems(IEnumerable<CartItem> items)
        {
            var productTasks = items
                .Select(x => x.ProductId)
                .Distinct()
                .Select(_productsRepository.GetByIdAsync);
            return await Task.WhenAll(productTasks);
        }
    }
}
