using System;

namespace Cnaws.Data.Query
{
    public sealed class DbCaseWhen : DbExpressionQueryElement
    {
        private DbExpression _when;

        internal DbCaseWhen(DbQueryElement parent, DbExpression when, DbExpression then)
            : base(parent, then)
        {
            if (when == null)
                throw new ArgumentNullException("when");
            _when = when;
        }

        public DbCaseWhen When(DbExpression when, DbExpression then)
        {
            return new DbCaseWhen(this, when, then);
        }
        public DbCaseElse Else(DbExpression then)
        {
            return new DbCaseElse(this, then);
        }
        public DbCaseEnd End()
        {
            return new DbCaseEnd(this);
        }

        internal override void OnBuild(DbQueryBuilder builder)
        {
            builder.Append(" WHEN ").Append(_when).Append(" THEN ").Append(Expression);
        }
    }
}
