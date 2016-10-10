using System;
using System.Collections.Generic;

namespace Cnaws.Data.Query
{
    internal sealed class DbToListQuery<T> where T : IDbSelectQuery
    {
        private T _query;

        internal DbToListQuery(T query)
        {
            _query = query;
        }

        private IList<dynamic> ExecuteImpl(int size, long page)
        {
            if (_query.Query.Provider.SupperLimit)
            {
                DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
                builder.Append(" LIMIT ").Append(size).Append(" OFFSET ").Append((page - 1) * size);
                builder.Append(';');
                return _query.Query.DataSource.ExecuteReader(builder.Sql, builder.Parameters);
            }

            if (_query.Query.Provider.SupperTop)
            {
                DbQueryBuilder builder = _query.Build(_query.Query.DataSource, page * size, false);
                builder.Append(';');
                IList<dynamic> list = _query.Query.DataSource.ExecuteReader(builder.Sql, builder.Parameters);
                if (page == 1)
                    return list;
                List<dynamic> array = new List<dynamic>(size);
                for (long i = ((page - 1) * size); i < list.Count; ++i)
                    array.Add(list[(int)i]);
                return array;
            }

            throw new NotSupportedException();
        }
        private IList<R> ExecuteImpl<R>(int size, long page) where R : IDbReader, new()
        {
            if (_query.Query.Provider.SupperLimit)
            {
                DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
                builder.Append(" LIMIT ").Append(size).Append(" OFFSET ").Append((page - 1) * size);
                builder.Append(';');
                return _query.Query.DataSource.ExecuteReader<R>(builder.Sql, builder.Parameters);
            }

            if (_query.Query.Provider.SupperTop)
            {
                DbQueryBuilder builder = _query.Build(_query.Query.DataSource, page * size, false);
                builder.Append(';');
                IList<R> list = _query.Query.DataSource.ExecuteReader<R>(builder.Sql, builder.Parameters);
                if (page == 1)
                    return list;
                List<R> array = new List<R>(size);
                for (long i = ((page - 1) * size); i < list.Count; ++i)
                    array.Add(list[(int)i]);
                return array;
            }

            throw new NotSupportedException();
        }

        private IList<dynamic> ExecuteImpl(int size, long page, out long count)
        {
            bool group = false;
            DbQueryBuilder qb = _query.BuildCount(_query.Query.DataSource, 0, false, ref group);
            if (group)
                qb.Append(") AS T").Append(_query.Query.DataSource.PsCount);
            count = Convert.ToInt64(_query.Query.DataSource.ExecuteScalar(qb.Sql, qb.Parameters));

            if (_query.Query.Provider.SupperRowNumber)
            {
                long half = count / 2;
                long lower = (page - 1) * size;
                long upper = page * size;
                bool reverse = lower > half;
                DbQueryRowNumberBuilder builder = _query.BuildRowNumber(_query.Query.DataSource, reverse ? (count - lower) : upper, false, null, reverse);
                builder.Append(")SELECT * FROM CTE WHERE _RowNumber>");
                if (reverse)
                    builder.Append(count - upper);
                else
                    builder.Append(lower);
                if (builder.OrderBy != null)
                    builder.Append(' ').Append(builder.OrderBy);
                builder.Append(';');
                return _query.Query.DataSource.ExecuteReader(builder.Sql, builder.Parameters);
            }

            if (_query.Query.Provider.SupperLimit)
            {
                DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
                builder.Append(" LIMIT ").Append(size).Append(" OFFSET ").Append((page - 1) * size);
                builder.Append(';');
                return _query.Query.DataSource.ExecuteReader(builder.Sql, builder.Parameters);
            }

            if (_query.Query.Provider.SupperTop)
            {
                DbQueryBuilder builder = _query.Build(_query.Query.DataSource, page * size, false);
                builder.Append(';');
                IList<dynamic> list = _query.Query.DataSource.ExecuteReader(builder.Sql, builder.Parameters);
                List<dynamic> array = new List<dynamic>(size);
                for (long i = ((page - 1) * size); i < list.Count; ++i)
                    array.Add(list[(int)i]);
                return array;
            }

            throw new NotSupportedException();
        }
        private IList<R> ExecuteImpl<R>(int size, long page, out long count) where R : IDbReader, new()
        {
            bool group = false;
            DbQueryBuilder qb = _query.BuildCount(_query.Query.DataSource, 0, false, ref group);
            if (group)
                qb.Append(") AS T").Append(_query.Query.DataSource.PsCount);
            count = Convert.ToInt64(_query.Query.DataSource.ExecuteScalar(qb.Sql, qb.Parameters));

            if (_query.Query.Provider.SupperRowNumber)
            {
                long half = count / 2;
                long lower = (page - 1) * size;
                long upper = page * size;
                bool reverse = lower > half;
                DbQueryRowNumberBuilder builder = _query.BuildRowNumber(_query.Query.DataSource, reverse ? (count - lower) : upper, false, null, reverse);
                builder.Append(")SELECT * FROM CTE WHERE _RowNumber>");
                if (reverse)
                    builder.Append(count - upper);
                else
                    builder.Append(lower);
                if (builder.OrderBy != null)
                    builder.Append(' ').Append(builder.OrderBy);
                builder.Append(';');
                return _query.Query.DataSource.ExecuteReader<R>(builder.Sql, builder.Parameters);
            }

            if (_query.Query.Provider.SupperLimit)
            {
                DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
                builder.Append(" LIMIT ").Append(size).Append(" OFFSET ").Append((page - 1) * size);
                builder.Append(';');
                return _query.Query.DataSource.ExecuteReader<R>(builder.Sql, builder.Parameters);
            }

            if (_query.Query.Provider.SupperTop)
            {
                DbQueryBuilder builder = _query.Build(_query.Query.DataSource, page * size, false);
                builder.Append(';');
                IList<R> list = _query.Query.DataSource.ExecuteReader<R>(builder.Sql, builder.Parameters);
                List<R> array = new List<R>(size);
                for (long i = ((page - 1) * size); i < list.Count; ++i)
                    array.Add(list[(int)i]);
                return array;
            }

            throw new NotSupportedException();
        }

        public IList<dynamic> Execute()
        {
            DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
            builder.Append(';');
            return _query.Query.DataSource.ExecuteReader(builder.Sql, builder.Parameters);
        }
        public IList<dynamic> Execute(int size, long page = 1)
        {
            return ExecuteImpl(size, page);
        }
        public IList<dynamic> Execute(int size, long page, out long count)
        {
            return ExecuteImpl(size, page, out count);
        }
        public IList<R> Execute<R>() where R : IDbReader, new()
        {
            DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
            builder.Append(';');
            return _query.Query.DataSource.ExecuteReader<R>(builder.Sql, builder.Parameters);
        }
        public IList<R> Execute<R>(int size, long page = 1) where R : IDbReader, new()
        {
            return ExecuteImpl<R>(size, page);
        }
        public IList<R> Execute<R>(int size, long page, out long count) where R : IDbReader, new()
        {
            return ExecuteImpl<R>(size, page, out count);
        }
    }
}
