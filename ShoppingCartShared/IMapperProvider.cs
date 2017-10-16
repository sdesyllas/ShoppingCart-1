using AutoMapper;

namespace ShoppingCart.Shared
{
    public interface IMapperProvider<TSource, TDest>
    {
        IMapper Provide();
    }
}
