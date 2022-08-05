using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.Infrastructure.Queries;

using Microsoft.EntityFrameworkCore;

namespace MEI.Core.Infrastructure.Data.Helpers
{
    public static class PagingExtensions
    {
        public static Paged<T> Page<T>(this IEnumerable<T> collection, PageInfo paging)
        {
            paging = paging ?? new PageInfo();

            return new Paged<T> {Items = collection.Skip(paging.PageIndex * paging.PageSize).Take(paging.PageSize).ToArray(), Paging = paging};
        }

        /// <summary>
        /// </summary>
        /// <remarks>Ensure that you call .OrderBy prior to this.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public static async Task<Paged<T>> Page<T>(this IQueryable<T> collection, PageInfo paging)
        {
            paging = paging ?? new PageInfo();

            var pagedItems = collection.Skip(paging.PageIndex * paging.PageSize).Take(paging.PageSize);

            return new Paged<T> {Items = await pagedItems.ToArrayAsync(), Paging = paging};
        }
    }
}