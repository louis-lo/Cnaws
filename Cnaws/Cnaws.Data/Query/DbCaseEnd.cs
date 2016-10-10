using System;

namespace Cnaws.Data.Query
{
    public sealed class DbCaseEnd : DbQueryElement
    {
        internal DbCaseEnd(DbQueryElement parent)
            : base(parent)
        {
        }

        internal override void OnBuild(DbQueryBuilder builder)
        {
            builder.Append(" END");
        }
    }
}
