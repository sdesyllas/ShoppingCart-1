using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared.Dto;

namespace ShoppingCart.UnitTests.Controllers.Validators
{
    class ResultMessageValidator<T> : ResultObjectValidator<T, ResultMessageDto>
        where T : ObjectResult
    {
        public ResultMessageValidator(string expectedMessage)
        {
            RuleFor(x => extractValue(x).Message).Equal(expectedMessage);
        }
    }
}
