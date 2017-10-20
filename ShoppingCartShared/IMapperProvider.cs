using AutoMapper;

namespace ShoppingCart.Shared
{
    /// <summary>
    /// Provides mapper for mapping between 
    /// </summary>
    /// <typeparam name="TSource">Mapping from type</typeparam>
    /// <typeparam name="TDest">Mapping to type</typeparam>
    public interface IMapperProvider<TSource, TDest>
    {
        /// <summary>
        /// Creates a mapping.
        /// </summary>
        /// <returns></returns>
        IMapper Provide();
    }
}
