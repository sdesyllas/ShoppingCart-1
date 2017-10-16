using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.UnitTests.Controllers.Validators
{
    class CartResultValidator : ResultObjectValidator<OkObjectResult, Cart>
    {
        public CartResultValidator(string cartName)
        {
            RuleFor(x => extractValue(x).Name).Equal(cartName);
        }
    }
}
