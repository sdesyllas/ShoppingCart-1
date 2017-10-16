﻿using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartUnitTests.Controllers.Validators
{
    class NotFoundResultValidator : ResultMessageValidator<NotFoundObjectResult>
    {
        public NotFoundResultValidator(string expectedMessage)
            : base(expectedMessage)
        { }
    }
}
