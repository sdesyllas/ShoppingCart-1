using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.UnitTests.Controllers.Validators
{
    class BadRequestResultValidator : ResultMessageValidator<BadRequestObjectResult>
    {
        public BadRequestResultValidator(string expectedMessage)
            : base(expectedMessage)
        { }
    }
}
