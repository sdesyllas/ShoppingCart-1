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
        private readonly IRepository<Cart> cartsReposiotry;
        public ShoppingBasketController(IRepository<Cart> repository)
        {
            cartsReposiotry = repository;
        }

        [HttpGet("{cartName}")]
        public ActionResult Get(string cartName)
        {
            var cart = cartsReposiotry.GetByName(cartName);
            if (cart == null)
            {
                return NotFound(new ResultMessage($"Cart {cartName} not found")); 
            }
            return Ok(cart);
        }
    }
}
