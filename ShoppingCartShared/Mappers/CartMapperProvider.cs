using AutoMapper;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.Shared.Mappers
{
    public class CartMapperProvider : IMapperProvider<Cart, CartDto>
    {
        private readonly IValueResolver<CartItem, CartItemDto, CartProductDto> _productValueResolver;

        public CartMapperProvider(IValueResolver<CartItem, CartItemDto, CartProductDto> productValueResolver)
        {
            this._productValueResolver = productValueResolver;
        }

        public IMapper Provide()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Cart, CartDto>();
                cfg.CreateMap<CartItem, CartItemDto>()
                .ForMember(x => x.Product, opt => opt.ResolveUsing(_productValueResolver));
            }).CreateMapper();
        }
    }
}
