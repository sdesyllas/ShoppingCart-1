using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartUnitTests.Controllers.Validators
{
    class OkResultValidator : ResultMessageValidator<OkObjectResult>
    {
        public OkResultValidator(string expectedMessage) :
            base(expectedMessage)
        { }
    }
}
