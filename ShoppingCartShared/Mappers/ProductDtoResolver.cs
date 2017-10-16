using AutoMapper;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using System.Threading.Tasks;

namespace ShoppingCart.Shared.Mappers
{
    public class ProductDtoResolver : IValueResolver<CartItem, CartItemDto, CartProductDto>
    {
        private readonly IQueryableByIdRepository<Product> productsRepository;
        private readonly IMapper mapper;

        public ProductDtoResolver(IQueryableByIdRepository<Product> productsRepository, IMapperProvider<Product, CartProductDto> mapper)
        {
            this.productsRepository = productsRepository;
            this.mapper = mapper.Provide();
        }

        public CartProductDto Resolve(CartItem source, CartItemDto destination, CartProductDto destMember, ResolutionContext context)
        {
            var model = Task
                .Run(async () => await productsRepository.GetById(source.ID))
                .Result;

            return mapper.Map<CartProductDto>(model);
        }
    }
}
