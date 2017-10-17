using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    public class ShoppingBasketController : Controller
    {
        private readonly IRepository<Cart> _cartsRepository;
        private readonly IQueryableByIdRepository<Product> _productsRepository;
        private readonly IMapper _cartMapper;

        public ShoppingBasketController(IRepository<Cart> cartsRepository,
            IQueryableByIdRepository<Product> productsRepository,
            IMapperProvider<Cart, CartDto> cartMapperProvider)
        {
            this._cartsRepository = cartsRepository;
            this._productsRepository = productsRepository;
            this._cartMapper = cartMapperProvider.Provide();
        }

        [HttpGet("{cartName}")]
        public async Task<ActionResult> Get(string cartName)
        {
            var cart = await _cartsRepository.GetByNameAsync(cartName);
            if (cart == null)
            {
                return NotFound(new ResultMessageDto($"Cart {cartName} not found"));
            }        

            var cartDto = _cartMapper.Map<CartDto>(cart);

            if (cartDto.Items != null && cartDto.Items.Any(x => x.Product == null))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResultMessageDto("Inconsistent database state"));
            }

            return Ok(cartDto);
        }

        [HttpPut("{cartName}")]
        public async Task<ActionResult> Put(string cartName, [FromBody] AddCartItemDto item)
        {
            if(item == null)
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

            var product = await _productsRepository.GetByIdAsync(item.ID);
            if (product == null)
            {
                return NotFound(new ResultMessageDto($"Product with id {item.ID} not found"));
            }

            if (product.Stock < item.Quantity)
            {
                return BadRequest(new ResultMessageDto("Not enough quantity"));
            }

            CartItem model = new CartItem()
            {
                ID = item.ID,
                Quantity = item.Quantity
            };
            cart.Items.Add(model);

            return Ok(new ResultMessageDto("Product added"));
        }

        [HttpGet("{cartName}/Checkout")]
        public async Task<ActionResult> Checkout(string cartName)
        {
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
                .GroupBy(x => x.ID)
                .Select(x => new
                {
                    Id = x.Key,
                    CartSum = x.Sum(y => y.Quantity),
                    Product = products.First(prod => prod.ID == x.Key)
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
                .Select(x => x.ID)
                .Distinct()
                .Select(_productsRepository.GetByIdAsync);
            return await Task.WhenAll(productTasks);
        }
    }
}
