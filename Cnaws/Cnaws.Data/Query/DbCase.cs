using System;

namespace Cnaws.Data.Query
{
    public sealed class DbCase : DbExpressionQueryElement
    {
        internal DbCase(DbQueryElement parent, DbExpression expression)
            : base(parent, expression)
        {
        }

        public DbCaseWhen When(DbExpression when, DbExpression then)
        {
            return new DbCaseWhen(this, when, then);
        }

        internal override void OnBuild(DbQueryBuilder builder)
        {
            builder.Append(" CASE ").Append(Expression);
        }
    }
}
