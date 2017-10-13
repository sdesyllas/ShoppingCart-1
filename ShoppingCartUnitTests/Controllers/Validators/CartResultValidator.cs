using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared.Model;
using System;

namespace ShoppingCartUnitTests.Controllers.Validators
{
    public class CartResultValidator : AbstractValidator<ActionResult>
    {
        private static readonly Func<ActionResult, OkObjectResult> asOkResult = x => x as OkObjectResult;
        private static readonly Func<ActionResult, object> extractValue = x => asOkResult.Invoke(x).Value;
        public CartResultValidator(string cartName)
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.GetType()).Equal(typeof(OkObjectResult));
            RuleFor(x => asOkResult(x).Value.GetType()).Equal(typeof(Cart));
            RuleFor(x => (extractValue(x) as Cart).Name).Equal(cartName);
        }
    }
}
