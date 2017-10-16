using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared.Dto;

namespace ShoppingCartUnitTests.Controllers.Validators
{
    public class NotFoundResultValidator : ResultObjectValidator<NotFoundObjectResult, ResultMessage>
    {
        public NotFoundResultValidator(string expectedMessage)
        {
            RuleFor(x => extractValue(x).Message).Equal(expectedMessage);
        }
    }
}
