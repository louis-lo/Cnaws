using System;

namespace Cnaws.Data.Query
{
    public sealed class DbCaseElse : DbExpressionQueryElement
    {
        internal DbCaseElse(DbQueryElement parent, DbExpression then)
            : base(parent, then)
        {
        }

        public DbCaseEnd End()
        {
            return new DbCaseEnd(this);
        }

        internal override void OnBuild(DbQueryBuilder builder)
        {
            builder.Append(" ELSE ").Append(Expression);
        }
    }
}
