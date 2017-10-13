using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartUnitTests.Controllers.Validators
{
    public class NotFoundResultValidator : AbstractValidator<ActionResult>
    {
        public NotFoundResultValidator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.GetType()).Equal(typeof(NotFoundResult));
        }
    }
}
