using Logitar.Data;
using Logitar.Data.SqlServer;

namespace SkillCraft.EntityFrameworkCore.SqlServer;

internal class SqlServerHelper : SqlHelper
{
  public override IQueryBuilder QueryFrom(TableId table) => SqlServerQueryBuilder.From(table);
}
