using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared;
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
                return NotFound();
            }
            return Ok(cart);
        }
    }
}
