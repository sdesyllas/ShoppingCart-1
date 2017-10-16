using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.UnitTests.Controllers.Validators
{
    class NotFoundResultValidator : ResultMessageValidator<NotFoundObjectResult>
    {
        public NotFoundResultValidator(string expectedMessage)
            : base(expectedMessage)
        { }
    }
}
