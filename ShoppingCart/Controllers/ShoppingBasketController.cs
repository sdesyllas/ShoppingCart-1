using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ShoppingCart.Controllers
{
    //TODO: conent-type
    [Route("api/[controller]")]
    public class ShoppingBasketController : Controller
    {
        private readonly IRepository<Cart> cartsRepository;
        private readonly IQueryableByIdRepository<Product> productsRepository;
        private readonly IMapper cartMapper;

        public ShoppingBasketController(IRepository<Cart> cartsRepository,
            IQueryableByIdRepository<Product> productsRepository,
            IMapperProvider<Cart, CartDto> cartMapperProvider)
        {
            this.cartsRepository = cartsRepository;
            this.productsRepository = productsRepository;
            this.cartMapper = cartMapperProvider.Provide();
        }

        [HttpGet("{cartName}")]
        public async Task<ActionResult> Get(string cartName)
        {
            var cart = await cartsRepository.GetByName(cartName);
            if (cart == null)
            {
                return NotFound(new ResultMessageDto($"Cart {cartName} not found"));
            }        

            var cartDto = cartMapper.Map<CartDto>(cart);

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

            var cart = await cartsRepository.GetByName(cartName);
            if(cart == null)
            {
                return NotFound(new ResultMessageDto($"Cart {cartName} not found"));
            }

            var product = await productsRepository.GetById(item.ID);
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
    }
}
