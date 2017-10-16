using FluentValidation;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.Shared.UnitTests.Mappers
{
    internal class ProductDtoMappedValidator : AbstractValidator<CartProductDto>
    {
        public ProductDtoMappedValidator(Product product)
        {
            RuleFor(x => x.Description).Equal(product.Description);
            RuleFor(x => x.ID).Equal(product.ID);
            RuleFor(x => x.Name).Equal(product.Name);
            RuleFor(x => x.Price).Equal(product.Price);
        }
    }
}