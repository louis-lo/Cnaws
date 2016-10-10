using System;

namespace Cnaws.Data.Query
{
    public enum DbOrderByType
    {
        Asc,
        Desc
    }

    public class DbOrderBy
    {
        private string _column;
        private DbOrderByType _type;

        public DbOrderBy(string column, DbOrderByType type = DbOrderByType.Asc)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            _column = column;
            _type = type;
        }

        public string Column
        {
            get { return _column; }
        }
        public DbOrderByType Type
        {
            get { return _type; }
        }

        private string GetTypeString()
        {
            return GetTypeString(_type);
        }
        private string GetTypeStringReverse()
        {
            DbOrderByType type;
            if (_type == DbOrderByType.Asc)
                type = DbOrderByType.Desc;
            else
                type = DbOrderByType.Asc;
            return GetTypeString(type);
        }
        private static string GetTypeString(DbOrderByType type)
        {
            if (type == DbOrderByType.Desc)
                return " DESC";
            return " ASC";
        }
        protected virtual string FormatColumn(DataSource ds)
        {
            return ds.Provider.EscapeName(_column);
        }
        protected virtual string FormatColumnBase(DataSource ds)
        {
            return ds.Provider.EscapeName(_column);
        }

        internal string Build(DataSource ds)
        {
            return string.Concat(FormatColumn(ds), GetTypeString());
        }
        internal string BuildBase(DataSource ds)
        {
            return string.Concat(FormatColumnBase(ds), GetTypeString());
        }
        internal string BuildReverse(DataSource ds)
        {
            return string.Concat(FormatColumn(ds), GetTypeStringReverse());
        }
    }

    public sealed class DbOrderBy<T> : DbOrderBy where T : IDbReader
    {
        public DbOrderBy(string column, DbOrderByType type = DbOrderByType.Asc)
            : base(column, type)
        {
        }

        protected override string FormatColumn(DataSource ds)
        {
            return string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<T>()), '.', base.FormatColumn(ds));
        }
        protected override string FormatColumnBase(DataSource ds)
        {
            return ds.Provider.EscapeName(string.Concat(DbTable.GetTableName<T>(), '_', Column));
        }
    }
}
