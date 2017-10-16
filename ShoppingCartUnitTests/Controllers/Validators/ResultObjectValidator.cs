using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCartUnitTests.Controllers.Validators
{
    public class ResultObjectValidator<T, Q> : AbstractValidator<ActionResult>
        where T : ObjectResult
        where Q : class
    {
        protected static readonly Func<ActionResult, Q> extractValue = x => castResult(x).Value as Q;
        private static readonly Func<ActionResult, T> castResult = x => x as T;

        public ResultObjectValidator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.GetType()).Equal(typeof(T));
            RuleFor(x => castResult(x).Value).NotNull();
            RuleFor(x => extractValue).NotNull();
        }
    }
}
