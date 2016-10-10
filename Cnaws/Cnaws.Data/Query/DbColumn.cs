using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnaws.Data.Query
{
    public class DbColumn
    {
        private string _column;

        public DbColumn(string column)
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

        public static implicit operator DbColumn(string name)
        {
            return new DbColumn(name);
        }
    }

    public sealed class DbColumn<T> : DbColumn where T : IDbReader
    {
        public DbColumn(string column)
            : base(column)
        {
        }

        protected override string FormatColumn(DataSource ds)
        {
            return string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<T>()), '.', ds.Provider.EscapeName(Column));
        }
    }
}
