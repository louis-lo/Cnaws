using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Cnaws.Data
{
    //internal enum DataQueryType
    //{
    //    Insert = 0,
    //    Update = 1,
    //    Select = 2,
    //    Delete = 3
    //}

    //internal abstract class DataQueryBuilder
    //{
    //    private DataSource _ds;
    //    private List<DataParameter> _ps;

    //    protected DataQueryBuilder(DataSource ds)
    //    {
    //        _ds = ds;
    //        _ps = new List<DataParameter>();
    //    }

    //    public DataSource DataSource
    //    {
    //        get { return _ds; }
    //    }
    //    public DataProvider Provider
    //    {
    //        get { return _ds.Provider; }
    //    }
    //    public List<DataParameter> Parameters
    //    {
    //        get { return _ps; }
    //    }

    //    public abstract string GetSqlString(bool append = false);
    //}
    //internal sealed class DataSelectQueryBuilder : DataQueryBuilder
    //{
    //    private DataColumn[] _columns;
    //    private string _table;
    //    private List<string> _join;
    //    private DataWhereQueue _where;
    //    private DataColumn[] _group;
    //    private List<string> _union;
    //    private DataOrder[] _order;
    //    private int _size;
    //    private long _page;
    //    private bool _prefix;

    //    public DataSelectQueryBuilder(DataSource ds)
    //        : base(ds)
    //    {
    //        _columns = null;
    //        _table = null;
    //        _join = new List<string>();
    //        _where = null;
    //        _group = null;
    //        _union = new List<string>();
    //        _order = null;
    //        _size = 0;
    //        _page = 1L;
    //        _prefix = false;
    //    }

    //    public void AppendSelect<T>(DataColumn[] columns) where T : IDbReader, new()
    //    {
    //        _columns = columns;
    //        _table = Provider.EscapeName(DbTable.GetTableName<T>());
    //    }
    //    public void AppendJoinOn<A, B>(string aid, string bid, DataJoinType type) where A : IDbReader, new() where B : IDbReader, new()
    //    {
    //        _prefix = true;
    //        string ta = Provider.EscapeName(DbTable.GetTableName<A>());
    //        string tb = Provider.EscapeName(DbTable.GetTableName<B>());
    //        _join.Add(string.Concat(type.ToString().ToUpper(), " JOIN ", tb, " ON ", ta, '.', Provider.EscapeName(aid), '=', tb, '.', Provider.EscapeName(bid)));
    //    }
    //    public void AppendJoinOn<A, B>(DataSelectQueryBuilder builder, string aid, string bid, DataJoinType type)
    //    {
    //        _prefix = true;
    //        string ta = Provider.EscapeName(DbTable.GetTableName<A>());
    //        string tb = Provider.EscapeName(DbTable.GetTableName<B>());
    //        _join.Add(string.Concat(type.ToString().ToUpper(), " JOIN (", builder.GetSqlString(true), ") AS ", tb, " ON ", ta, '.', Provider.EscapeName(aid), '=', tb, '.', Provider.EscapeName(bid)));
    //        Parameters.AddRange(builder.Parameters);
    //    }
    //    public void AppendWhere(DataWhereQueue where)
    //    {
    //        _where = where;
    //        Parameters.AddRange(where.Parameters);
    //    }
    //    public void AppendGroupBy(DataColumn[] columns)
    //    {
    //        _group = columns;
    //    }
    //    public void AppendUnion(DataSelectQueryBuilder builder, bool all)
    //    {
    //        if (all)
    //            _union.Add(string.Concat("UNION ALL ", builder.GetSqlString(true)));
    //        else
    //            _union.Add(string.Concat("UNION ", builder.GetSqlString(true)));
    //        Parameters.AddRange(builder.Parameters);
    //    }
    //    public void AppendOrderBy(DataOrder[] order)
    //    {
    //        _order = order;
    //    }
    //    public void AppendLimit(int size, long page)
    //    {
    //        _size = size;
    //        _page = page;
    //    }

    //    public long Count()
    //    {
    //        //if (string.IsNullOrEmpty(group))
    //        //    return BuildSelectSqlImpl(null, "COUNT(*)", table_and_join, where, group, null, null, false);
    //        //return string.Concat("SELECT COUNT(*) FROM (", BuildSelectSqlImpl(null, "COUNT(*) AS C", table_and_join, where, group, null, null, true), ") AS T;");

    //        //KeyValuePair<string, DataParameter[]> where = GetWhere(_ds.Provider);
    //        //return CountImpl(GetTableName(), GetWhere(_ds.Provider), GetGroupBy(_ds.Provider));
    //    }
    //    internal long CountImpl(string table, KeyValuePair<string, DataParameter[]> where, string group)
    //    {
    //        //return Convert.ToInt64(_ds.ExecuteScalar(_ds.Provider.BuildSelectCountSqlImpl(table, where.Key, group), where.Value));
    //    }
    //    public object Single()
    //    {
    //        if (_select == null || _select.Length == 0)
    //            throw new ArgumentException();
    //        KeyValuePair<string, DataParameter[]> where = GetWhere(_ds.Provider);
    //        return _ds.ExecuteScalar(_ds.Provider.BuildSelectSqlImpl(null, _select[0].GetSqlString(_ds.Provider, Prefix, true), GetTableName(), where.Key, GetGroupBy(_ds.Provider), GetOrderBy(_ds.Provider), null, false), where.Value);
    //    }
    //    public T Single<T>()
    //    {
    //        if (_select == null || _select.Length == 0)
    //            throw new ArgumentException();
    //        KeyValuePair<string, DataParameter[]> where = GetWhere(_ds.Provider);
    //        return _ds.ExecuteScalar<T>(_ds.Provider.BuildSelectSqlImpl(null, _select[0].GetSqlString(_ds.Provider, Prefix, true), GetTableName(), where.Key, GetGroupBy(_ds.Provider), GetOrderBy(_ds.Provider), null, false), where.Value);
    //    }
    //    public dynamic First()
    //    {
    //        KeyValuePair<string, DataParameter[]> where = GetWhere(_ds.Provider);
    //        return _ds.ExecuteSingleRow(_ds.Provider.BuildSelectSqlImpl(null, GetSelect(_ds.Provider), GetTableName(), where.Key, GetGroupBy(_ds.Provider), GetOrderBy(_ds.Provider), null, false), where.Value);
    //    }
    //    public T First<T>() where T : IDbReader, new()
    //    {
    //        KeyValuePair<string, DataParameter[]> where = GetWhere(_ds.Provider);
    //        return _ds.ExecuteSingleRow<T>(_ds.Provider.BuildSelectSqlImpl(null, GetSelect(_ds.Provider), GetTableName(), where.Key, GetGroupBy(_ds.Provider), GetOrderBy(_ds.Provider), null, false), where.Value);
    //    }
    //    public IList<dynamic> ToList()
    //    {
    //        KeyValuePair<string, DataParameter[]> where = GetWhere(_ds.Provider);
    //        return _ds.ExecuteReader(_ds.Provider.BuildSelectSqlImpl(null, GetSelect(_ds.Provider), GetTableName(), where.Key, GetGroupBy(_ds.Provider), GetOrderBy(_ds.Provider), null, false), where.Value);
    //    }
    //    public IList<T> ToList<T>() where T : IDbReader, new()
    //    {
    //        KeyValuePair<string, DataParameter[]> where = GetWhere(_ds.Provider);
    //        return _ds.ExecuteReader<T>(_ds.Provider.BuildSelectSqlImpl(null, GetSelect(_ds.Provider), GetTableName(), where.Key, GetGroupBy(_ds.Provider), GetOrderBy(_ds.Provider), null, false), where.Value);
    //    }

    //    private string GetCountSqlString(bool append = false)
    //    {
    //        StringBuilder sb = new StringBuilder(512);
    //        sb.Append(Provider.SelectSql).Append(" COUNT(*) AS C FROM ");
    //        if (_union.Count > 0)
    //        {
    //            sb.Append('(');
    //            sb.Append(GetSqlString(true));
    //            foreach (string union in _union)
    //                sb.Append(' ').Append(union);
    //            sb.Append(") AS T");
    //        }
    //        else
    //        {
    //            if (_group != null)
    //            {
    //                sb.Append('(');
    //                sb.Append(GetSqlString(true));
    //                sb.Append(')');
    //            }
    //            else
    //            {
    //                sb.Append(_table).Append(" AS T").Append(index);
    //                foreach (string join in _join)
    //                    sb.Append(' ').Append(join);
    //                if (_where != null)
    //                    sb.Append(" WHERE ").Append(_where.GetSqlString(Provider, _prefix, false));
    //            }
    //        }
    //        if (!append)
    //            sb.Append(';');
    //        return sb.ToString();
    //    }
    //    public override string GetSqlString(bool append = false)
    //    {
    //        KeyValuePair<string, string> limit = Provider.GetTopOrLimit(_size, _page);
    //        StringBuilder sb = new StringBuilder(512);
    //        sb.Append(Provider.SelectSql);
    //        if (!string.IsNullOrEmpty(limit.Key))
    //            sb.Append(' ').Append(limit.Key);
    //        if (_columns != null && _columns.Length > 0)
    //            sb.Append(' ').Append(Provider.GetSqlString(_columns, _prefix, true));
    //        else
    //            sb.Append(' ').Append(_table).Append(".*");
    //        sb.Append(" FROM ").Append(_table);
    //        foreach (string join in _join)
    //            sb.Append(' ').Append(join);
    //        if (_where != null)
    //            sb.Append(" WHERE ").Append(_where.GetSqlString(Provider, _prefix, false));
    //        foreach (string union in _union)
    //            sb.Append(' ').Append(union);
    //        if (_order != null && _order.Length > 0)
    //            sb.Append(" ORDER BY ").Append(Provider.GetSqlString(_order, _prefix, false));
    //        if (!string.IsNullOrEmpty(limit.Value))
    //            sb.Append(' ').Append(limit.Value);
    //        if (!append)
    //            sb.Append(';');
    //        return sb.ToString();
    //    }
    //    private string GetRowNumberSqlString()
    //    {
    //        StringBuilder sb = new StringBuilder();

    //        string table = _query.GetTableName();
    //        KeyValuePair<string, DataParameter[]> pair = _query.GetWhere(ds.Provider);
    //        string group = _query.GetGroupBy(ds.Provider);

    //        count = _query.CountImpl(table, pair, group);
    //        long half = count / 2;
    //        long lower = (_page - 1) * _size;
    //        long upper = _page * _size;

    //        string select = _query.GetSelect(ds.Provider);
    //        string where = pair.Key;
    //        if (where == null)
    //            where = string.Empty;
    //        else
    //            where = string.Concat("WHERE ", where);
    //        string order = _query.GetOrderByImpl(ds.Provider, true);
    //        string torder = _query.GetOrderByImpl(ds.Provider, false);
    //        if (order == null)
    //            order = string.Empty;
    //        if (torder == null)
    //            torder = string.Empty;

    //        if (lower > half)
    //        {
    //            if (torder.Length > 0)
    //            {
    //                torder = Providers.MSSQLProvider.ORDER_REGEX.Replace(torder, new MatchEvaluator((m) =>
    //                {
    //                    return "A".Equals(m.Groups[1].Value, StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
    //                }));
    //            }
    //        }
    //        if (order.Length > 0)
    //            order = string.Concat("ORDER BY ", order);
    //        if (torder.Length > 0)
    //            torder = string.Concat("ORDER BY ", torder);

    //        long top = count - lower;
    //        if (top <= 0)
    //            return new List<dynamic>();

    //        sb.Append("WITH CTE AS(SELECT TOP ");
    //        if (lower > half)
    //            sb.Append(top);
    //        else
    //            sb.Append(upper);
    //        sb.Append(" ROW_NUMBER() OVER(");
    //        sb.Append(torder);
    //        sb.Append(")AS _RowNumber,");
    //        sb.Append(select);
    //        sb.Append(" FROM ");
    //        sb.Append(table);
    //        if (where.Length > 0)
    //            sb.Append(' ').Append(where);
    //        if (torder.Length > 0)
    //            sb.Append(' ').Append(torder);
    //        sb.Append(")SELECT * FROM CTE WHERE _RowNumber>");
    //        if (lower > half)
    //            sb.Append(count - upper);
    //        else
    //            sb.Append(lower);
    //        if (order.Length > 0)
    //            sb.Append(' ').Append(order);
    //        sb.Append(";");
    //        return ds.ExecuteReader(sb.ToString(), pair.Value);
    //    }
    //}

    public static class DataQuery
    {
        public static SelectQuery Select<T>(DataSource ds, params DataColumn[] columns) where T : IDbReader, new()
        {
            return new SelectQuery(ds, ds.Provider.EscapeName(DbTable.GetTableName<T>()), columns);
        }
    }
    public sealed class SelectQuery
    {
        internal DataSource _ds;
        private string _table;
        private DataColumn[] _select;
        private WhereQuery _where;
        private GroupByQuery _groupBy;
        private OrderByQuery _orderBy;
        private bool _prefix;
        private string _default;
        private List<DataParameter> _ps;

        internal SelectQuery(DataSource ds, string table, params DataColumn[] columns)
        {
            _ds = ds;
            _table = table;
            _select = columns;
            _where = null;
            _groupBy = null;
            _orderBy = null;
            _prefix = false;
            _default = string.Concat(table, ".*");
            _ps = new List<DataParameter>();
        }

        internal SelectQuery JoinOn(string ta, string aid, DataJoinType type, string tb, string bid)
        {
            _table = string.Concat(_table, ' ', type.ToString().ToUpper(), " JOIN ", tb, " ON ", ta, '.', aid, '=', tb, '.', bid);
            _prefix = true;
            return this;
        }
        internal SelectQuery JoinSelect(string ta, string aid, DataJoinType type, string tb, string bid, string sql, DataParameter[] ps)
        {
            if (ps != null && ps.Length > 0)
                _ps.AddRange(ps);
            _table = string.Concat(_table, ' ', type.ToString().ToUpper(), " JOIN (", sql, ") AS ", tb, " ON ", ta, '.', aid, '=', tb, '.', bid);
            _prefix = true;
            return this;
        }
        private bool Prefix
        {
            get { return _prefix; }
        }
        internal string GetSelect(DataSource ds)
        {
            if (_select == null || _select.Length == 0)
                return _default;
            return DataProvider.GetSqlString(_select, _ds, Prefix, true);
        }
        internal string GetTableName()
        {
            return _table;
        }
        internal KeyValuePair<string, DataParameter[]> GetWhere(DataSource ds)
        {
            string sql = null;
            if (_where != null)
            {
                sql = _where.GetSqlString(ds, Prefix, false);
                _ps.AddRange(_where.GetParameters());
            }
            return new KeyValuePair<string, DataParameter[]>(sql, _ps.ToArray());
        }
        internal string GetGroupBy(DataSource ds)
        {
            if (_groupBy != null)
                return _groupBy.GetSqlString(ds, Prefix, false);
            return null;
        }
        internal string GetOrderBy(DataSource ds)
        {
            return GetOrderByImpl(ds, false);
        }
        internal string GetOrderByImpl(DataSource ds, bool select)
        {
            if (_orderBy != null)
                return _orderBy.GetSqlString(ds, Prefix, select);
            return null;
        }

        public long Count()
        {
            KeyValuePair<string, DataParameter[]> where = GetWhere(_ds);
            return CountImpl(GetTableName(), where, GetGroupBy(_ds));
        }
        internal long CountImpl(string table, KeyValuePair<string, DataParameter[]> where, string group)
        {
            return Convert.ToInt64(_ds.ExecuteScalar(_ds.Provider.BuildSelectCountSqlImpl(table, where.Key, group), where.Value));
        }
        public object Single()
        {
            if (_select == null || _select.Length == 0)
                throw new ArgumentException();
            KeyValuePair<string, DataParameter[]> where = GetWhere(_ds);
            return _ds.ExecuteScalar(_ds.Provider.BuildSelectSqlImpl(null, _select[0].GetSqlString(_ds, Prefix, true), GetTableName(), where.Key, GetGroupBy(_ds), GetOrderBy(_ds), null, false), where.Value);
        }
        public T Single<T>()
        {
            if (_select == null || _select.Length == 0)
                throw new ArgumentException();
            KeyValuePair<string, DataParameter[]> where = GetWhere(_ds);
            return _ds.ExecuteScalar<T>(_ds.Provider.BuildSelectSqlImpl(null, _select[0].GetSqlString(_ds, Prefix, true), GetTableName(), where.Key, GetGroupBy(_ds), GetOrderBy(_ds), null, false), where.Value);
        }
        public dynamic First()
        {
            KeyValuePair<string, DataParameter[]> where = GetWhere(_ds);
            return _ds.ExecuteSingleRow(_ds.Provider.BuildSelectSqlImpl(null, GetSelect(_ds), GetTableName(), where.Key, GetGroupBy(_ds), GetOrderBy(_ds), null, false), where.Value);
        }
        public T First<T>() where T : IDbReader, new()
        {
            KeyValuePair<string, DataParameter[]> where = GetWhere(_ds);
            return _ds.ExecuteSingleRow<T>(_ds.Provider.BuildSelectSqlImpl(null, GetSelect(_ds), GetTableName(), where.Key, GetGroupBy(_ds), GetOrderBy(_ds), null, false), where.Value);
        }
        public IList<dynamic> ToList()
        {
            KeyValuePair<string, DataParameter[]> where = GetWhere(_ds);
            return _ds.ExecuteReader(_ds.Provider.BuildSelectSqlImpl(null, GetSelect(_ds), GetTableName(), where.Key, GetGroupBy(_ds), GetOrderBy(_ds), null, false), where.Value);
        }
        public IList<T> ToList<T>() where T : IDbReader, new()
        {
            KeyValuePair<string, DataParameter[]> where = GetWhere(_ds);
            return _ds.ExecuteReader<T>(_ds.Provider.BuildSelectSqlImpl(null, GetSelect(_ds), GetTableName(), where.Key, GetGroupBy(_ds), GetOrderBy(_ds), null, false), where.Value);
        }

        public JoinQuery Join<T>(string id, DataJoinType type = DataJoinType.Inner) where T : IDbReader, new()
        {
            return new JoinQuery(this, _ds.Provider.EscapeName(DbTable.GetTableName<T>()), _ds.Provider.EscapeName(id), type);
        }

        //public UnionQuery Union(bool all = true)
        //{
        //    return new UnionQuery(this, all);
        //}
        public WhereQuery Where(DataWhereQueue where)
        {
            _where = new WhereQuery(this, where);
            return _where;
        }
        public GroupByQuery GroupBy(params DataColumn[] group)
        {
            _groupBy = new GroupByQuery(this, group);
            return _groupBy;
        }
        public OrderByQuery OrderBy(params DataOrder[] order)
        {
            _orderBy = new OrderByQuery(this, order);
            return _orderBy;
        }
        public LimitQuery Limit(int size, long page = 1L)
        {
            return new LimitQuery(this, size, page);
        }
    }
    public sealed class JoinQuery
    {
        internal SelectQuery _query;
        internal string _table;
        internal string _id;
        internal DataJoinType _type;

        internal JoinQuery(SelectQuery query, string table, string id, DataJoinType type = DataJoinType.Inner)
        {
            _query = query;
            _table = table;
            _id = id;
            _type = type;
        }

        public SelectQuery On<T>(string id) where T : IDbReader, new()
        {
            return _query.JoinOn(_table, _id, _type, _query._ds.Provider.EscapeName(DbTable.GetTableName<T>()), _query._ds.Provider.EscapeName(id));
        }
        public JoinSelectQuery Select<T>(params DataColumn[] columns) where T : IDbReader, new()
        {
            return new JoinSelectQuery(this, _query._ds.Provider.EscapeName(DbTable.GetTableName<T>()), columns);
        }
    }
    public sealed class JoinSelectQuery
    {
        internal JoinQuery _query;
        private string _table;
        private DataColumn[] _select;
        private JoinWhereQuery _where;
        private JoinGroupByQuery _groupBy;
        private bool _prefix;
        private string _default;

        internal JoinSelectQuery(JoinQuery query, string table, params DataColumn[] columns)
        {
            _query = query;
            _table = table;
            _select = columns;
            _where = null;
            _groupBy = null;
            _prefix = false;
            _default = string.Concat(table, ".*");
        }

        private bool Prefix
        {
            get { return _prefix; }
        }
        internal string GetSelect(DataSource ds)
        {
            if (_select == null || _select.Length == 0)
                return _default;
            return DataProvider.GetSqlString(_select, _query._query._ds, Prefix, true);
        }
        internal string GetTableName()
        {
            return _table;
        }
        internal KeyValuePair<string, DataParameter[]> GetWhere(DataSource ds)
        {
            if (_where != null)
                return new KeyValuePair<string, DataParameter[]>(_where.GetSqlString(ds, Prefix, false), _where.GetParameters());
            return new KeyValuePair<string, DataParameter[]>(null, null);
        }
        internal string GetGroupBy(DataSource ds)
        {
            if (_groupBy != null)
                return _groupBy.GetSqlString(ds, Prefix, false);
            return null;
        }

        public SelectQuery On<T>(string id) where T : IDbReader, new()
        {
            KeyValuePair<string, DataParameter[]> where = GetWhere(_query._query._ds);
            string sql = _query._query._ds.Provider.BuildSelectSqlImpl(null, GetSelect(_query._query._ds), GetTableName(), where.Key, GetGroupBy(_query._query._ds), null, null, true);
            _query._query.JoinSelect(_query._table, _query._id, _query._type, _query._query._ds.Provider.EscapeName(DbTable.GetTableName<T>()), id, sql, where.Value);
            return _query._query;
        }
        public JoinWhereQuery Where(DataWhereQueue where)
        {
            _where = new JoinWhereQuery(this, where);
            return _where;
        }
        public JoinGroupByQuery GroupBy(params DataColumn[] group)
        {
            _groupBy = new JoinGroupByQuery(this, group);
            return _groupBy;
        }
    }
    public sealed class JoinWhereQuery
    {
        private JoinSelectQuery _query;
        private DataWhereQueue _where;

        internal JoinWhereQuery(JoinSelectQuery query, DataWhereQueue where)
        {
            _query = query;
            _where = where;
        }

        internal string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (_where != null)
                return DataProvider.GetSqlString(_where, _query._query._query._ds, prefix, select);
            return null;
        }
        internal DataParameter[] GetParameters()
        {
            if (_where != null)
                return DataWhereQueue.GetParameters(_where);
            return null;
        }

        public SelectQuery On<T>(string id) where T : IDbReader, new()
        {
            return _query.On<T>(id);
        }
        public JoinGroupByQuery GroupBy(params DataColumn[] group)
        {
            return _query.GroupBy(group);
        }
    }
    public sealed class JoinGroupByQuery
    {
        private JoinSelectQuery _query;
        private DataColumn[] _group;

        internal JoinGroupByQuery(JoinSelectQuery query, params DataColumn[] group)
        {
            _query = query;
            _group = group;
        }

        internal string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (_group != null)
                return DataProvider.GetSqlString(_group, _query._query._query._ds, prefix, select);
            return null;
        }

        public SelectQuery On<T>(string id) where T : IDbReader, new()
        {
            return _query.On<T>(id);
        }
    }
    //public sealed class UnionQuery
    //{
    //    internal SelectQuery _query;
    //    internal bool _all;

    //    internal UnionQuery(SelectQuery query, bool all)
    //    {

    //    }
    //}
    //public sealed class UnionSelectQuery
    //{
    //    internal UnionQuery _query;
    //    private string _table;
    //    private DataColumn[] _select;
    //    private JoinWhereQuery _where;
    //    private JoinGroupByQuery _groupBy;
    //    private bool _prefix;
    //    private string _default;

    //    internal UnionSelectQuery(UnionQuery query, string table, params DataColumn[] columns)
    //    {
    //        _query = query;
    //        _table = table;
    //        _select = columns;
    //        _where = null;
    //        _groupBy = null;
    //        _prefix = false;
    //        _default = string.Concat(table, ".*");
    //    }

    //    private bool Prefix
    //    {
    //        get { return _prefix; }
    //    }
    //    internal string GetSelect(DataSource ds)
    //    {
    //        if (_select == null || _select.Length == 0)
    //            return _default;
    //        return provider.GetSqlString(_select, Prefix, true);
    //    }
    //    internal string GetTableName()
    //    {
    //        return _table;
    //    }
    //    internal KeyValuePair<string, DataParameter[]> GetWhere(DataSource ds)
    //    {
    //        if (_where != null)
    //            return new KeyValuePair<string, DataParameter[]>(_where.GetSqlString(provider, Prefix, false), _where.GetParameters());
    //        return new KeyValuePair<string, DataParameter[]>(null, null);
    //    }
    //    internal string GetGroupBy(DataSource ds)
    //    {
    //        if (_groupBy != null)
    //            return _groupBy.GetSqlString(provider, Prefix, false);
    //        return null;
    //    }

    //    public SelectQuery On<T>(string id) where T : IDbReader, new()
    //    {
    //        KeyValuePair<string, DataParameter[]> where = GetWhere(_query._query._ds.Provider);
    //        string sql = _query._query._ds.Provider.BuildSelectSqlImpl(null, GetSelect(_query._query._ds.Provider), GetTableName(), where.Key, GetGroupBy(_query._query._ds.Provider), null, null, true);
    //        _query._query.JoinSelect(_query._table, _query._id, _query._type, _query._query._ds.Provider.EscapeName(DbTable.GetTableName<T>()), id, sql, where.Value);
    //        return _query._query;
    //    }
    //    public JoinWhereQuery Where(DataWhereQueue where)
    //    {
    //        _where = new JoinWhereQuery(this, where);
    //        return _where;
    //    }
    //    public JoinGroupByQuery GroupBy(params DataColumn[] group)
    //    {
    //        _groupBy = new JoinGroupByQuery(this, group);
    //        return _groupBy;
    //    }
    //}
    //public sealed class UnionWhereQuery
    //{
    //    private UnionSelectQuery _query;
    //    private DataWhereQueue _where;

    //    internal UnionWhereQuery(UnionSelectQuery query, DataWhereQueue where)
    //    {
    //        _query = query;
    //        _where = where;
    //    }

    //    internal string GetSqlString(DataSource ds, bool prefix, bool select)
    //    {
    //        if (_where != null)
    //            return provider.GetSqlString(_where, prefix, select);
    //        return null;
    //    }
    //    internal DataParameter[] GetParameters()
    //    {
    //        if (_where != null)
    //            return DataWhereQueue.GetParameters(_where);
    //        return null;
    //    }

    //    public SelectQuery On<T>(string id) where T : IDbReader, new()
    //    {
    //        return _query.On<T>(id);
    //    }
    //    public JoinGroupByQuery GroupBy(params DataColumn[] group)
    //    {
    //        return _query.GroupBy(group);
    //    }
    //}
    //public sealed class UnionGroupByQuery
    //{
    //    private UnionSelectQuery _query;
    //    private DataColumn[] _group;

    //    internal UnionGroupByQuery(UnionSelectQuery query, params DataColumn[] group)
    //    {
    //        _query = query;
    //        _group = group;
    //    }

    //    internal string GetSqlString(DataSource ds, bool prefix, bool select)
    //    {
    //        if (_group != null)
    //            return provider.GetSqlString(_group, prefix, select);
    //        return null;
    //    }

    //    public SelectQuery On<T>(string id) where T : IDbReader, new()
    //    {
    //        return _query.On<T>(id);
    //    }
    //}

    public sealed class WhereQuery
    {
        private SelectQuery _query;
        private DataWhereQueue _where;

        internal WhereQuery(SelectQuery query, DataWhereQueue where)
        {
            _query = query;
            _where = where;
        }

        internal string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (_where != null)
                return DataProvider.GetSqlString(_where, _query._ds, prefix, select);
            return null;
        }
        internal DataParameter[] GetParameters()
        {
            if (_where != null)
                return DataWhereQueue.GetParameters(_where);
            return null;
        }

        public long Count()
        {
            return _query.Count();
        }
        public object Single()
        {
            return _query.Single();
        }
        public T Single<T>()
        {
            return _query.Single<T>();
        }
        public dynamic First()
        {
            return _query.First();
        }
        public T First<T>() where T : IDbReader, new()
        {
            return _query.First<T>();
        }
        public IList<dynamic> ToList()
        {
            return _query.ToList();
        }
        public IList<T> ToList<T>() where T : IDbReader, new()
        {
            return _query.ToList<T>();
        }

        //public UnionQuery Union(bool all = true)
        //{
        //    return _query.Union(all);
        //}
        public GroupByQuery GroupBy(params DataColumn[] group)
        {
            return _query.GroupBy(group);
        }
        public OrderByQuery OrderBy(params DataOrder[] order)
        {
            return _query.OrderBy(order);
        }
        public LimitQuery Limit(int size, long page = 1L)
        {
            return _query.Limit(size, page);
        }
    }
    public sealed class GroupByQuery
    {
        private SelectQuery _query;
        private DataColumn[] _group;

        internal GroupByQuery(SelectQuery query, params DataColumn[] group)
        {
            _query = query;
            _group = group;
        }

        internal string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (_group != null)
                return DataProvider.GetSqlString(_group, _query._ds, prefix, select);
            return null;
        }

        public long Count()
        {
            return _query.Count();
        }
        public object Single()
        {
            return _query.Single();
        }
        public T Single<T>()
        {
            return _query.Single<T>();
        }
        public dynamic First()
        {
            return _query.First();
        }
        public T First<T>() where T : IDbReader, new()
        {
            return _query.First<T>();
        }
        public IList<dynamic> ToList()
        {
            return _query.ToList();
        }
        public IList<T> ToList<T>() where T : IDbReader, new()
        {
            return _query.ToList<T>();
        }

        //public UnionQuery Union(bool all = true)
        //{
        //    return _query.Union(all);
        //}
        public OrderByQuery OrderBy(params DataOrder[] order)
        {
            return _query.OrderBy(order);
        }
        public LimitQuery Limit(int size, long page = 1L)
        {
            return _query.Limit(size, page);
        }
    }
    public sealed class OrderByQuery
    {
        private SelectQuery _query;
        private DataOrder[] _order;

        internal OrderByQuery(SelectQuery query, params DataOrder[] order)
        {
            _query = query;
            _order = order;
        }

        internal string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (_order != null)
                return DataProvider.GetSqlString(_order, _query._ds, prefix, select);
            return null;
        }

        public object Single()
        {
            return _query.Single();
        }
        public T Single<T>()
        {
            return _query.Single<T>();
        }
        public dynamic First()
        {
            return _query.First();
        }
        public T First<T>() where T : IDbReader, new()
        {
            return _query.First<T>();
        }
        public IList<dynamic> ToList()
        {
            return _query.ToList();
        }
        public IList<T> ToList<T>() where T : IDbReader, new()
        {
            return _query.ToList<T>();
        }

        public LimitQuery Limit(int size, long page = 1L)
        {
            return _query.Limit(size, page);
        }
    }
    public sealed class LimitQuery
    {
        private SelectQuery _query;
        private int _size;
        private long _page;

        internal LimitQuery(SelectQuery query, int size, long page = 1L)
        {
            _query = query;
            _size = Math.Max(size, 0);
            _page = Math.Max(page, 1);
        }

        public IList<dynamic> ToList()
        {
            if (_size > 0)
            {
                if (_page > 1)
                {
                    long count;
                    return ToList(out count);
                }
                KeyValuePair<string, string> limit = _query._ds.Provider.GetTopOrLimit(_size, _page);
                KeyValuePair<string, DataParameter[]> where = _query.GetWhere(_query._ds);
                return _query._ds.ExecuteReader(_query._ds.Provider.BuildSelectSqlImpl(limit.Key, _query.GetSelect(_query._ds), _query.GetTableName(), where.Key, _query.GetGroupBy(_query._ds), _query.GetOrderBy(_query._ds), limit.Value, false), where.Value);
            }
            return _query.ToList();
        }
        public IList<T> ToList<T>() where T : IDbReader, new()
        {
            if (_size > 0)
            {
                if (_page > 1)
                {
                    long count;
                    return ToList<T>(out count);
                }
                KeyValuePair<string, string> limit = _query._ds.Provider.GetTopOrLimit(_size, _page);
                KeyValuePair<string, DataParameter[]> where = _query.GetWhere(_query._ds);
                return _query._ds.ExecuteReader<T>(_query._ds.Provider.BuildSelectSqlImpl(limit.Key, _query.GetSelect(_query._ds), _query.GetTableName(), where.Key, _query.GetGroupBy(_query._ds), _query.GetOrderBy(_query._ds), limit.Value, false), where.Value);
            }
            return _query.ToList<T>();
        }
        public IList<dynamic> ToList(out long count)
        {
            if (_query._ds.Provider.SupperRowNumber)
                return ToListWithRowNumber(_query._ds, out count);
            IList<dynamic> list = ToListWithLimitOrTop(_query._ds, out count);
            if (_query._ds.Provider.SupperTop)
                list = SplitList(list, _page, _size);
            return list;
        }
        public IList<T> ToList<T>(out long count) where T : IDbReader, new()
        {
            if (_query._ds.Provider.SupperRowNumber)
                return ToListWithRowNumber<T>(_query._ds, out count);
            IList<T> list = ToListWithLimitOrTop<T>(_query._ds, out count);
            if (_query._ds.Provider.SupperTop)
                list = SplitList<T>(list, _page, _size);
            return list;
        }

        private IList<dynamic> ToListWithLimitOrTop(DataSource ds, out long count)
        {
            string table = _query.GetTableName();
            KeyValuePair<string, DataParameter[]> where = _query.GetWhere(ds);
            string group = _query.GetGroupBy(ds);
            count = _query.CountImpl(table, where, group);
            KeyValuePair<string, string> limit = ds.Provider.GetTopOrLimit(_size, _page);
            return ds.ExecuteReader(ds.Provider.BuildSelectSqlImpl(limit.Key, _query.GetSelect(ds), table, where.Key, group, _query.GetOrderBy(ds), limit.Value, false), where.Value);
        }
        private IList<T> ToListWithLimitOrTop<T>(DataSource ds, out long count) where T : IDbReader, new()
        {
            string table = _query.GetTableName();
            KeyValuePair<string, DataParameter[]> where = _query.GetWhere(ds);
            string group = _query.GetGroupBy(ds);
            count = _query.CountImpl(table, where, group);
            KeyValuePair<string, string> limit = ds.Provider.GetTopOrLimit(_size, _page);
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSqlImpl(limit.Key, _query.GetSelect(ds), table, where.Key, group, _query.GetOrderBy(ds), limit.Value, false), where.Value);
        }
        private IList<dynamic> SplitList(IList<dynamic> list, long index, int size)
        {
            List<dynamic> array = new List<dynamic>();
            for (long i = ((index - 1) * size); i < list.Count; ++i)
                array.Add(list[(int)i]);
            return array;
        }
        private IList<T> SplitList<T>(IList<T> list, long index, int size) where T : IDbReader, new()
        {
            List<T> array = new List<T>();
            for (long i = ((index - 1) * size); i < list.Count; ++i)
                array.Add(list[(int)i]);
            return array;
        }
        private IList<dynamic> ToListWithRowNumber(DataSource ds, out long count)
        {
            StringBuilder sb = new StringBuilder();

            string table = _query.GetTableName();
            KeyValuePair<string, DataParameter[]> pair = _query.GetWhere(ds);
            string group = _query.GetGroupBy(ds);

            count = _query.CountImpl(table, pair, group);
            long half = count / 2;
            long lower = (_page - 1) * _size;
            long upper = _page * _size;

            string select = _query.GetSelect(ds);
            string where = pair.Key;
            if (where == null)
                where = string.Empty;
            else
                where = string.Concat("WHERE ", where);
            string order = _query.GetOrderByImpl(ds, true);
            string torder = _query.GetOrderByImpl(ds, false);
            if (order == null)
                order = string.Empty;
            if (torder == null)
                torder = string.Empty;

            if (lower > half)
            {
                if (torder.Length > 0)
                {
                    torder = Providers.MSSQLProvider.ORDER_REGEX.Replace(torder, new MatchEvaluator((m) =>
                    {
                        return "A".Equals(m.Groups[1].Value, StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
                    }));
                }
            }
            if (order.Length > 0)
                order = string.Concat("ORDER BY ", order);
            if (torder.Length > 0)
                torder = string.Concat("ORDER BY ", torder);

            long top = count - lower;
            if (top <= 0)
                return new List<dynamic>();

            sb.Append("WITH CTE AS(SELECT TOP ");
            if (lower > half)
                sb.Append(top);
            else
                sb.Append(upper);
            sb.Append(" ROW_NUMBER() OVER(");
            sb.Append(torder);
            sb.Append(")AS _RowNumber,");
            sb.Append(select);
            sb.Append(" FROM ");
            sb.Append(table);
            if (where.Length > 0)
                sb.Append(' ').Append(where);
            if (torder.Length > 0)
                sb.Append(' ').Append(torder);
            sb.Append(")SELECT * FROM CTE WHERE _RowNumber>");
            if (lower > half)
                sb.Append(count - upper);
            else
                sb.Append(lower);
            if (order.Length > 0)
                sb.Append(' ').Append(order);
            sb.Append(";");
            return ds.ExecuteReader(sb.ToString(), pair.Value);
        }
        private IList<T> ToListWithRowNumber<T>(DataSource ds, out long count) where T : IDbReader, new()
        {
            StringBuilder sb = new StringBuilder();

            string table = _query.GetTableName();
            KeyValuePair<string, DataParameter[]> pair = _query.GetWhere(ds);
            string group = _query.GetGroupBy(ds);

            count = _query.CountImpl(table, pair, group);
            long half = count / 2;
            long lower = (_page - 1) * _size;
            long upper = _page * _size;

            string select = _query.GetSelect(ds);
            string where = pair.Key;
            if (where == null)
                where = string.Empty;
            else
                where = string.Concat("WHERE ", where);
            string order = _query.GetOrderByImpl(ds, true);
            string torder = _query.GetOrderByImpl(ds, false);
            if (order == null)
                order = string.Empty;
            if (torder == null)
                torder = string.Empty;

            if (lower > half)
            {
                if (torder.Length > 0)
                {
                    torder = Providers.MSSQLProvider.ORDER_REGEX.Replace(torder, new MatchEvaluator((m) =>
                    {
                        return "A".Equals(m.Groups[1].Value, StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
                    }));
                }
            }
            if (order.Length > 0)
                order = string.Concat("ORDER BY ", order);
            if (torder.Length > 0)
                torder = string.Concat("ORDER BY ", torder);

            long top = count - lower;
            if (top <= 0)
                return new List<T>();

            sb.Append("WITH CTE AS(SELECT TOP ");
            if (lower > half)
                sb.Append(top);
            else
                sb.Append(upper);
            sb.Append(" ROW_NUMBER() OVER(");
            sb.Append(torder);
            sb.Append(")AS _RowNumber,");
            sb.Append(select);
            sb.Append(" FROM ");
            sb.Append(table);
            if (where.Length > 0)
                sb.Append(' ').Append(where);
            if (torder.Length > 0)
                sb.Append(' ').Append(torder);
            sb.Append(")SELECT * FROM CTE WHERE _RowNumber>");
            if (lower > half)
                sb.Append(count - upper);
            else
                sb.Append(lower);
            if (order.Length > 0)
                sb.Append(' ').Append(order);
            sb.Append(";");
            return ds.ExecuteReader<T>(sb.ToString(), pair.Value);
        }
    }
}
