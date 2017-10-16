using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared.Dto;

namespace ShoppingCart.UnitTests.Controllers.Validators
{
    class CartResultValidator : ResultObjectValidator<OkObjectResult, CartDto>
    {
        public CartResultValidator(string cartName)
        {
            RuleFor(x => extractValue(x).Name).Equal(cartName);
        }
    }
}
