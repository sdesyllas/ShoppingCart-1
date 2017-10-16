using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using System.Threading.Tasks;
using CartDto = ShoppingCart.Shared.Dto.Cart;
using CartModel = ShoppingCart.Shared.Model.Cart;
using CartItemDto = ShoppingCart.Shared.Dto.CartItem;
using CartItemModel = ShoppingCart.Shared.Model.CartItem;
using ProductDto = ShoppingCart.Shared.Dto.Product;
using ProductModel = ShoppingCart.Shared.Model.Product;

namespace ShoppingCart.Controllers
{
    //TODO: conent-type
    [Route("api/[controller]")]
    public class ShoppingBasketController : Controller
    {
        private readonly IRepository<CartModel> cartsRepository;
        private readonly IQueryableByIdRepository<ProductModel> productsRepository;
        public ShoppingBasketController(IRepository<CartModel> cartsRepository,
            IQueryableByIdRepository<ProductModel> productsRepository)
        {
            this.cartsRepository = cartsRepository;
            this.productsRepository = productsRepository;
        }

        [HttpGet("{cartName}")]
        public async Task<ActionResult> Get(string cartName)
        {
            var cart = await cartsRepository.GetByName(cartName);
            if (cart == null)
            {
                return NotFound(new ResultMessage($"Cart {cartName} not found"));
            }
            return Ok(cart);
        }

        [HttpPut("{cartName}")]
        public async Task<ActionResult> Put(string cartName, [FromBody] AddCartItem item)
        {
            if(item == null)
            {
                return BadRequest(new ResultMessage("Empty body"));
            }

            if(item.Quantity <= 0)
            {
                return BadRequest(new ResultMessage("Invalid quantity"));
            }

            var cart = await cartsRepository.GetByName(cartName);
            if(cart == null)
            {
                return NotFound(new ResultMessage($"Cart {cartName} not found"));
            }

            var product = await productsRepository.GetById(item.ID);
            if (product == null)
            {
                return NotFound(new ResultMessage($"Product with id {item.ID} not found"));
            }

            if (product.Stock < item.Quantity)
            {
                return BadRequest(new ResultMessage("Not enough quantity"));
            }

            CartItemModel model = new CartItemModel()
            {
                ID = item.ID,
                Quantity = item.Quantity
            };
            cart.Items.Add(model);

            return Ok(new ResultMessage("Product added"));
        }
    }
}
