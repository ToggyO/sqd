using System.Linq;
using Squadio.Common.Models.Pages;

namespace Squadio.DAL.Extensions
{
    public static class QueryablePageExtension
    {
        public static IQueryable<T> GetPage<T>(this IQueryable<T> query, PageModel model) where T : class
        {
            query = query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize);

            return query;
        }
    }
}