using System;

namespace Cnaws.Data.Query
{
    public class DbGroupBy
    {
        private string _column;

        public DbGroupBy(string column)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            _column = column;
        }

        public string Column
        {
            get { return _column; }
        }

        protected virtual string FormatColumn(DataSource ds)
        {
            return ds.Provider.EscapeName(_column);
        }

        internal string Build(DataSource ds)
        {
            return FormatColumn(ds);
        }

        public static implicit operator DbGroupBy(string name)
        {
            return new DbGroupBy(name);
        }
    }

    public sealed class DbGroupBy<T> : DbGroupBy where T : IDbReader
    {
        public DbGroupBy(string column)
            : base(column)
        {
        }

        protected override string FormatColumn(DataSource ds)
        {
            return string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<T>()), '.', base.FormatColumn(ds));
        }
    }
}
