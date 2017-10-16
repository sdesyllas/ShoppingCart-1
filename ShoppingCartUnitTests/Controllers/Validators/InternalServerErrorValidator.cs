using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.UnitTests.Controllers.Validators
{
    class InternalServerErrorValidator : ResultMessageValidator<ObjectResult>
    {
        public InternalServerErrorValidator(string expectedMessage)
            : base(expectedMessage)
        {
            RuleFor(x => (x as ObjectResult).StatusCode).Equal(500);
        }
    }
}
