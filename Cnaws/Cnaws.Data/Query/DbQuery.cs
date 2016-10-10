using System;

namespace Cnaws.Data.Query
{
    public abstract class DbQuery
    {
        private DataSource _ds;

        internal DbQuery(DataSource ds)
        {
            _ds = ds;
        }

        internal DataSource DataSource
        {
            get { return _ds; }
        }
        internal DataProvider Provider
        {
            get { return _ds.Provider; }
        }

        internal DataParameter BuildParameter(object value)
        {
            return BuildParameter(DataSource, value);
        }
        internal static DataParameter BuildParameter(DataSource ds, object value)
        {
            return new DataParameter(string.Concat('V', ds.PsCount), value);
        }
    }

    public sealed class DbQuery<T> : DbQuery where T : IDbReader
    {
        internal DbQuery(DataSource ds)
            : base(ds)
        {
        }

        public DbSelectQuery<T> Select(params DbSelect[] columns)
        {
            return new DbSelectQuery<T>(this, columns);
        }

        public bool Insert(T instance)
        {
            return (new DbInsertInstanceQuery<T>(this, instance)).Execute();
        }
        public DbInsertQuery<T> Insert(params DbColumn[] columns)
        {
            return new DbInsertQuery<T>(this, columns);
        }

        public int Update(T instance)
        {
            return (new DbUpdateInstanceQuery<T>(this, instance)).Execute();
        }
        public int Update(T instance, params DbColumn[] columns)
        {
            return (new DbUpdateInstanceQuery<T>(this, instance, columns)).Execute();
        }
        public int Update(T instance, ColumnMode mode, params DbColumn[] columns)
        {
            return (new DbUpdateInstanceQuery<T>(this, instance, mode, columns)).Execute();
        }
        public DbUpdateQuery<T> Update()
        {
            return new DbUpdateQuery<T>(this);
        }

        public int Delete(T instance)
        {
            return (new DbDeleteInstanceQuery<T>(this, instance)).Execute();
        }
        public DbDeleteQuery<T> Delete()
        {
            return new DbDeleteQuery<T>(this);
        }
    }
}
