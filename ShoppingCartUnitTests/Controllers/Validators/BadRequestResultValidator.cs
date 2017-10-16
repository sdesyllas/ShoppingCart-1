using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartUnitTests.Controllers.Validators
{
    class BadRequestResultValidator : ResultMessageValidator<BadRequestObjectResult>
    {
        public BadRequestResultValidator(string expectedMessage)
            : base(expectedMessage)
        { }
    }
}
