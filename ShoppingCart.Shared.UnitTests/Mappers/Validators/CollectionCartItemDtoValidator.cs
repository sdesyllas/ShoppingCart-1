using FluentValidation;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Shared.UnitTests.Mappers.Validators
{
    internal class CollectionCartItemDtoValidator : AbstractValidator<CartItemDto>
    {
        internal CollectionCartItemDtoValidator(IEnumerable<CartItem> sourceItems)
        {
            RuleFor(x => x.Product).Must(x => sourceItems.Any(y => y.ID == x.ID));
        }
    }
}
