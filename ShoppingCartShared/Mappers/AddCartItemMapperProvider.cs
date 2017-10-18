using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using AutoMapper;

namespace ShoppingCart.Shared.Mappers
{
    public class AddCartItemMapperProvider : IMapperProvider<AddCartItemDto, CartItem>
    {
        public IMapper Provide()
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<AddCartItemDto, CartItem>())
                .CreateMapper();
        }
    }
}
