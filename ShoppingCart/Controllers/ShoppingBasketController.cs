using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingCart.Repository;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Threading.Tasks;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    public class ShoppingBasketController : Controller
    {
        private readonly ICartRepository _cartsRepository;
        private readonly IQueryableByIdRepository<Product> _productsRepository;
        private readonly IMapper _cartMapper;
        private readonly IMapper _cartItemMapper;
        private readonly ILogger _logger;

        public ShoppingBasketController(ICartRepository cartsRepository,
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
            try
            {
                var cart = await _cartsRepository.GetByNameAsync(cartName);
                var cartDto = _cartMapper.Map<CartDto>(cart);
                return Ok(cartDto);
            }
            catch (CartNotFoundException)
            {
                _logger.LogDebug($"Cart {cartName} not found");
                return NotFound(new ResultMessageDto("Cart not found"));
            }
            catch (ProdcutNotFoundException)
            {
                _logger.LogDebug($"Product on cart {cartName} does not exist in database");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResultMessageDto("Inconsistent database"));
            }
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

            var model = _cartItemMapper.Map<CartItem>(item);
            try
            {
                await _cartsRepository.AddItemToCart(cartName, _productsRepository.GetByIdAsync, model);
                return Ok(new ResultMessageDto("Product added"));
            }
            catch (ProdcutNotFoundException)
            {
                _logger.LogDebug($"Product not found on {cartName}");
                return NotFound(new ResultMessageDto($"Product with id {item.ProductId} not found"));
            }
            catch (CartNotFoundException)
            {
                _logger.LogDebug($"Cart {cartName} not found");
                return NotFound(new ResultMessageDto("Cart not found"));
            }
            catch (CartCheckedOutException)
            {
                _logger.LogDebug($"Cart {cartName} checked out");
                return BadRequest(new ResultMessageDto("Cart is checked out"));
            }
            catch (NotEnoughStockException)
            {
                _logger.LogDebug($"Cart {cartName} checked out");
                return BadRequest(new ResultMessageDto("Not enough stock"));
            }
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

        [HttpGet("{cartName}/Checkout")]
        [SwaggerResponse(200, typeof(ResultMessageDto), "Cart checked out")]
        [SwaggerResponse(400, typeof(ResultMessageDto), "Empty body / Invalid product quantity / Cart has beed checked out / Not enough stock")]
        [SwaggerResponse(404, typeof(ResultMessageDto), "Cart not found / Cart item product not found")]
        public async Task<ActionResult> CheckoutAsync(string cartName)
        {
            _logger.LogDebug($"Checkout called with parameter: {cartName}");
            try
            {
                await _cartsRepository.CheckoutAsync(cartName, _productsRepository.GetByIdAsync);
                return Ok(new ResultMessageDto("Cart checked out"));
            }
            catch (CartNotFoundException)
            {
                _logger.LogDebug($"Cart {cartName} not found");
                return NotFound(new ResultMessageDto("Cart not found"));
            }
            catch (ProdcutNotFoundException)
            {
                _logger.LogDebug($"Product not found on {cartName}");
                return NotFound(new ResultMessageDto($"Cart product not found"));
            }
            catch (CartCheckedOutException)
            {
                _logger.LogDebug($"Cart {cartName} checked out");
                return BadRequest(new ResultMessageDto("Cart is checked out"));
            }
            catch (NotEnoughStockException)
            {
                return BadRequest(new ResultMessageDto("Items out of stock"));
            }
        }
    }
}
