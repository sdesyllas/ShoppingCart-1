using FluentValidation;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.Shared.UnitTests.Mappers
{
    internal class CartItemDtoValidator : AbstractValidator<CartProductDto>
    {
        public CartItemDtoValidator(CartItem cartItem)
        {
            RuleFor(x => x.ID).Equal(cartItem.ID);
        }
    }
}