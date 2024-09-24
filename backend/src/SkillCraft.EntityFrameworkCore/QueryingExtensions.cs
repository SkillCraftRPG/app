using Logitar.Data;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace SkillCraft.EntityFrameworkCore;

internal static class QueryingExtensions
{
  public static IQueryBuilder ApplyIdFilter(this IQueryBuilder query, SearchPayload payload, ColumnId column)
  {
    if (payload.Ids.Count > 0)
    {
      string[] ids = payload.Ids.Select(id => id.ToString()).Distinct().ToArray();
      query.Where(column, Operators.IsIn(ids));
    }

    return query;
  }

  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, SearchPayload payload)
  {
    return query.ApplyPaging(payload.Skip, payload.Limit);
  }
  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int skip, int limit)
  {
    if (skip > 0)
    {
      query = query.Skip(skip);
    }
    if (limit > 0)
    {
      query = query.Take(limit);
    }

    return query;
  }

  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQueryBuilder builder) where T : class
  {
    return entities.FromQuery(builder.Build());
  }
  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQuery query) where T : class
  {
    return entities.FromSqlRaw(query.Text, query.Parameters.ToArray());
  }
}
