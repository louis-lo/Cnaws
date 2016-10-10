using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cnaws.Data.Query
{
    public class DbSelect
    {
        private string _column;

        public DbSelect(string column = null)
        {
            _column = column;
        }

        public string Column
        {
            get { return _column; }
        }

        protected virtual string FormatColumn(DataSource ds)
        {
            if ("*".Equals(_column))
                return _column;
            return ds.Provider.EscapeName(_column);
        }
        protected virtual string GetAllColumn(DataSource ds)
        {
            return "*";
        }

        internal virtual string Build(DataSource ds)
        {
            if (_column != null)
                return FormatColumn(ds);
            return GetAllColumn(ds);
        }

        public static implicit operator DbSelect(string name)
        {
            return new DbSelect(name);
        }
    }

    public sealed class DbSelect<T> : DbSelect where T : IDbReader
    {
        public DbSelect(string column = null)
            : base(column)
        {
        }

        protected override string FormatColumn(DataSource ds)
        {
            if ("*".Equals(Column))
                return string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<T>()), '.', Column);
            return FormatColumn(ds, Column, DbTable.GetTableName<T>());
        }
        private static string FormatColumn(DataSource ds, string column, string table)
        {
            return string.Concat(ds.Provider.EscapeName(table), '.', ds.Provider.EscapeName(column), " AS ", ds.Provider.EscapeName(string.Concat(table, '_', column)));
        }
        protected override string GetAllColumn(DataSource ds)
        {
            string table = DbTable.GetTableName<T>();
            Dictionary<string, FieldInfo> fs = TAllNameSetFields<T, DataColumnAttribute>.Fields;
            List<string> list = new List<string>(fs.Count);
            foreach (string key in fs.Keys)
                list.Add(FormatColumn(ds, key, table));
            return string.Join(",", list.ToArray());
        }
    }

    public class DbSelectAs : DbSelect
    {
        private string _name;

        public DbSelectAs(string column, string name = null)
            : base(column)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            _name = name ?? column;
        }

        public string Name
        {
            get { return _name; }
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat(ds.Provider.EscapeName(Column), " AS ", ds.Provider.EscapeName(Column));
        }
    }
    public class DbSelectAs<T> : DbSelectAs where T : IDbReader
    {
        public DbSelectAs(string column, string name = null)
            : base(column, name)
        {
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<T>()), '.', ds.Provider.EscapeName(Column), " AS ", ds.Provider.EscapeName(Name));
        }
    }
    public sealed class DbSelectAs<A, B> : DbSelectAs<A> where A : IDbReader where B : IDbReader
    {
        public DbSelectAs(string column, string name = null)
            : base(column, name)
        {
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<A>()), '.', ds.Provider.EscapeName(Column), " AS ", ds.Provider.EscapeName(string.Concat(DbTable.GetTableName<B>(), '_', Name)));
        }
    }

    public sealed class DbSelectCount : DbSelect
    {
        public DbSelectCount(string column)
            : base(column)
        {
            if (column == null)
                throw new ArgumentNullException("column");
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat("COUNT(*) AS ", ds.Provider.EscapeName(Column));
        }
    }

    public class DbSelectMax : DbSelect
    {
        public DbSelectMax(string column)
            : base(column)
        {
            if (column == null)
                throw new ArgumentNullException("column");
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat("MAX(", ds.Provider.EscapeName(Column), ") AS ", ds.Provider.EscapeName(Column));
        }
    }
    public sealed class DbSelectMax<T> : DbSelectMax where T : IDbReader
    {
        public DbSelectMax(string column)
            : base(column)
        {
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat("MAX(", ds.Provider.EscapeName(DbTable.GetTableName<T>()), '.', ds.Provider.EscapeName(Column), ") AS ", ds.Provider.EscapeName(Column));
        }
    }

    public class DbSelectMin : DbSelect
    {
        public DbSelectMin(string column)
            : base(column)
        {
            if (column == null)
                throw new ArgumentNullException("column");
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat("MIN(", ds.Provider.EscapeName(Column), ") AS ", ds.Provider.EscapeName(Column));
        }
    }
    public sealed class DbSelectMin<T> : DbSelectMin where T : IDbReader
    {
        public DbSelectMin(string column)
            : base(column)
        {
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat("MIN(", ds.Provider.EscapeName(DbTable.GetTableName<T>()), '.', ds.Provider.EscapeName(Column), ") AS ", ds.Provider.EscapeName(Column));
        }
    }

    public class DbSelectSum : DbSelect
    {
        public DbSelectSum(string column)
            : base(column)
        {
            if (column == null)
                throw new ArgumentNullException("column");
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat("SUM(", ds.Provider.EscapeName(Column), ") AS ", ds.Provider.EscapeName(Column));
        }
    }
    public class DbSelectSum<T> : DbSelect where T : IDbReader
    {
        public DbSelectSum(string column)
            : base(column)
        {
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat("SUM(", ds.Provider.EscapeName(DbTable.GetTableName<T>()), '.', ds.Provider.EscapeName(Column), ") AS ", ds.Provider.EscapeName(Column));
        }
    }

    public enum DbSumExpressionType
    {
        /// <summary>
        /// 加
        /// </summary>
        Add,
        /// <summary>
        /// 减
        /// </summary>
        Subtract,
        /// <summary>
        /// 乘
        /// </summary>
        Multiply,
        /// <summary>
        /// 除
        /// </summary>
        Divide
    }
    public class DbSelectSumExpression : DbSelectSum
    {
        private string _other;
        private DbSumExpressionType _type;

        public DbSelectSumExpression(string column, string other, DbSumExpressionType type)
            : base(column)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            if (other == null)
                throw new ArgumentNullException("other");
            _other = other;
            _type = type;
        }

        public string Other
        {
            get { return _other; }
        }
        public DbSumExpressionType Type
        {
            get { return _type; }
        }

        protected string GetTypeString()
        {
            switch (_type)
            {
                case DbSumExpressionType.Add: return "+";
                case DbSumExpressionType.Subtract: return "-";
                case DbSumExpressionType.Multiply: return "*";
                case DbSumExpressionType.Divide: return "/";
            }
            throw new NotSupportedException();
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat("SUM(", ds.Provider.EscapeName(Column), GetTypeString(), ds.Provider.EscapeName(Other), ") AS ", ds.Provider.EscapeName(Column));
        }
    }
    public sealed class DbSelectSumExpression<T> : DbSelectSumExpression where T : IDbReader
    {
        public DbSelectSumExpression(string column, string other, DbSumExpressionType type)
            : base(column, other, type)
        {
        }

        internal override string Build(DataSource ds)
        {
            return string.Concat("SUM(", ds.Provider.EscapeName(DbTable.GetTableName<T>()), '.', ds.Provider.EscapeName(Column), GetTypeString(), ds.Provider.EscapeName(DbTable.GetTableName<T>()), '.', ds.Provider.EscapeName(Other), ") AS ", ds.Provider.EscapeName(Column));
        }
    }
}
