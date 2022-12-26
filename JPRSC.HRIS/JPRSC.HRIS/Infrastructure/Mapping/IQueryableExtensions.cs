using System.Linq;

namespace AutoMapper.QueryableExtensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source, IMapper mapper)
        {
            return Extensions.ProjectTo<TDestination>(source, mapper.ConfigurationProvider);
        }
    }
}