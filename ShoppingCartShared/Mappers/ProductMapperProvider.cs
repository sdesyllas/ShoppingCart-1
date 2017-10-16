using AutoMapper;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.Shared.Mappers
{
    public class ProductMapperProvider : IMapperProvider<Product, CartProductDto>
    {
        public IMapper Provide()
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<Product, CartProductDto>())
                .CreateMapper();
        }
    }
}
