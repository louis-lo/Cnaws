using System;

namespace Cnaws.Data.Query
{
    public sealed class DbInsert
    {
        private string _column;

        internal DbInsert(string column)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            _column = column;
        }

        public string Column
        {
            get { return _column; }
        }

        internal string Build(DataSource ds)
        {
            return ds.Provider.EscapeName(_column);
        }

        public static implicit operator DbInsert(string name)
        {
            return new DbInsert(name);
        }
    }
}
