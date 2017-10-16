using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.Controllers
{
    //TODO: conent-type
    [Route("api/[controller]")]
    public class ShoppingBasketController : Controller
    {
        private readonly IRepository<Cart> cartsRepository;
        private readonly IQueryableByIdRepository<Product> productsRepository;
        public ShoppingBasketController(IRepository<Cart> cartsRepository, IQueryableByIdRepository<Product> productsRepository)
        {
            this.cartsRepository = cartsRepository;
            this.productsRepository = productsRepository;
        }

        [HttpGet("{cartName}")]
        public ActionResult Get(string cartName)
        {
            var cart = cartsRepository.GetByName(cartName);
            if (cart == null)
            {
                return NotFound(new ResultMessage($"Cart {cartName} not found"));
            }
            return Ok(cart);
        }

        [HttpPut("{cartName}")]
        public ActionResult Put(string cartName, [FromBody] CartItem item)
        {
            if(item == null)
            {
                return BadRequest(new ResultMessage("Empty body"));
            }
            var cart = cartsRepository.GetByName(cartName);
            if(cart == null)
            {
                return NotFound(new ResultMessage($"Cart {cartName} not found"));
            }
            var product = productsRepository.GetById(item.ID);
            if (product == null)
            {
                return NotFound(new ResultMessage($"Product with id {item.ID} not found"));
            }
            return Ok(item);
        }
    }
}
