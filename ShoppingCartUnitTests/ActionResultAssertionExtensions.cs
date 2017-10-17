using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Shared.Dto;

namespace ShoppingCart.UnitTests
{
    public static class ActionResultAssertionExtensions
    {
        public static AndWhichConstraint<ObjectAssertions, TExpectedResult> AssertResponseType<TExpectedResult>(
            this ActionResult result,
            int statusCode)
            where TExpectedResult : ObjectResult
        {
            result.Should().NotBeNull();
            var responseAssertion = result.Should().BeAssignableTo<TExpectedResult>();
            responseAssertion.Which.StatusCode.Should().Be(statusCode);
            return responseAssertion;
        }

        public static AndWhichConstraint<ObjectAssertions, TExpectedResult> AssertMessage<TExpectedResult>(
            this AndWhichConstraint<ObjectAssertions, TExpectedResult> result,
            string resultMessageBody)
            where TExpectedResult : ObjectResult
        {
            result.Subject.Value.Should().BeAssignableTo<ResultMessageDto>().Subject.Message.Should().Be(resultMessageBody);
            return result;
        }
    }
}
