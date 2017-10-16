using FluentValidation;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;

namespace ShoppingCart.Shared.UnitTests.Mappers.Validators
{
    internal class CartDtoValidator : AbstractValidator<CartDto>
    {
        internal CartDtoValidator(IEnumerable<CartItem> sourceItems)
        {
            RuleFor(x => x.Items).SetCollectionValidator(new CollectionCartItemDtoValidator(sourceItems));
        }
    }
}
