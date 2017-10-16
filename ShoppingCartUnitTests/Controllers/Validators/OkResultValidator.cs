using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.UnitTests.Controllers.Validators
{
    class OkResultValidator : ResultMessageValidator<OkObjectResult>
    {
        public OkResultValidator(string expectedMessage) :
            base(expectedMessage)
        { }
    }
}
