using MyBlog.Abstraction;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Extensions
{
  public static class IEnumerableExtensions
  {
    public static IEnumerable<T> ApplayPaging<T>(this IEnumerable<T> query, IPaggingQuery pagingParameter)
    {
      return query
        .Skip(pagingParameter.PageSize * (pagingParameter.Page - 1))
        .Take(pagingParameter.PageSize)
        .AsEnumerable();
    }
  }
}
