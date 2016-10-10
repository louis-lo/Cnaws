using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Cnaws.Data
{
    public enum DataSourceType
    {
        Unknown,
        MSSQL,
#if (PostgreSQL)
        PostgreSQL,
#endif
#if (SQLite)
        SQLite,
#endif
#if (MySQL)
        MySQL,
#endif
    }

    public sealed class DataSource : IDisposable
    {
        private sealed class VersionLocker
        {
            private object locker;
            private int version;

            public VersionLocker()
            {
                locker = null;
                version = 0;
            }

            public object Locker
            {
                get
                {
                    if (locker == null)
                        Interlocked.CompareExchange(ref locker, new object(), null);
                    return locker;
                }
            }
            public int Version
            {
                get { return version; }
            }

            public void Increment()
            {
                ++version;
            }
            public void Decrement()
            {
                --version;
            }
        }
        private sealed class DataTransaction
        {
            private DataSource _ds;
            private DbTransaction trans;
            private VersionLocker locker;

            public DataTransaction(DataSource ds)
            {
                _ds = ds;
                trans = null;
                locker = new VersionLocker();
            }

            public DbTransaction Transaction
            {
                get { return trans; }
            }

            public void Begin(DbConnection conn)
            {
                lock (locker.Locker)
                {
                    if (locker.Version == 0)
                    {
                        Debug.Assert(trans == null);
                        trans = conn.BeginTransaction();
                    }
                    locker.Increment();
                }
            }
            public void Commit()
            {
                lock (locker.Locker)
                {
                    locker.Decrement();
                    if (locker.Version == 0)
                    {
                        Debug.Assert(trans != null);
                        trans.Commit();
                        trans.Dispose();
                        trans = null;
                    }
                }
            }
            public void Rollback()
            {
                lock (locker.Locker)
                {
                    locker.Decrement();
                    if (locker.Version == 0)
                    {
                        Debug.Assert(trans != null);
                        trans.Rollback();
                        trans.Dispose();
                        trans = null;
                    }
                }
            }


            public void Dispose()
            {
                if (locker.Version != 0)
                {
                    if (_ds.Controller != null)
                    {
                        object app = _ds.Controller.GetType().GetProperty("Application", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)?.GetValue(_ds.Controller);
                        StringBuilder sb = new StringBuilder();
                        sb.Append(_ds.Controller?.GetType().FullName)
                            .Append(" - ").Append(app?.GetType().GetProperty("Action", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)?.GetValue(app) as string)
                            .AppendLine();
                        File.AppendAllText("D:\\wwwroot\\cnaws_error.txt", sb.ToString());
                    }
                }
                while (locker.Version > 0)
                    Rollback();
            }
        }


        public object Controller = null;

        private bool disposed;
        private string _name;
        private DbConnection _conn;
        private DataSourceType _type;
        private DataProvider _provider;
        private DataTransaction _trans;
        private VersionLocker _locker;
        private int _pscount;

        public DataSource(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            disposed = false;

            _name = name;
            _type = DataSourceType.Unknown;
            _provider = null;
            _trans = new DataTransaction(this);
            _locker = new VersionLocker();
            _pscount = 0;

            ConnectionStringSettings settings = System.Configuration.ConfigurationManager.ConnectionStrings[_name];
            if (settings == null)
                throw new SettingsPropertyNotFoundException(string.Concat("配置文件中不存在名为“", _name, "”的数据库连接字符串"));
            DbProviderFactory factory = DbProviderFactories.GetFactory(settings.ProviderName);
            _conn = factory.CreateConnection();
            _conn.ConnectionString = settings.ConnectionString;
            _conn.Open();

            lock (_locker.Locker)
                _locker.Increment();
        }
        public DataSource(DataSource ds)
        {
            if (ds == null)
                throw new ArgumentNullException("ds");

            disposed = false;

            _name = ds._name;
            _type = ds._type;
            _provider = ds._provider;
            _trans = ds._trans;
            _locker = ds._locker;
            _pscount = ds.PsCount;

            _conn = ds._conn;

            lock (_locker.Locker)
                _locker.Increment();
        }

        internal int PsCount
        {
            get
            {
                return Interlocked.Increment(ref _pscount);
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public DataSourceType Type
        {
            get
            {
                if (_type == DataSourceType.Unknown)
                {
                    if (_conn is System.Data.SqlClient.SqlConnection)
                        _type = DataSourceType.MSSQL;
#if (PostgreSQL)
                    else if (_conn is Npgsql.NpgsqlConnection)
                        _type = DataSourceType.PostgreSQL;
#endif
#if (SQLite)
                    else if (_conn is System.Data.SQLite.SQLiteConnection)
                        _type = DataSourceType.SQLite;
#endif
#if (MySQL)
                    else if (_conn is MySql.Data.MySqlClient.MySqlConnection)
                        _type = DataSourceType.MySQL;
#endif
                }
                return _type;
            }
        }

        internal DataProvider Provider
        {
            get
            {
                if (_provider == null)
                    _provider = DataProvider.GetProvider(Type);
                return _provider;
            }
        }
        internal DbConnection Connection
        {
            get { return _conn; }
        }

        private DbCommand CreateCommand()
        {
            DbCommand cmd = _conn.CreateCommand();
            if (_trans.Transaction != null)
                cmd.Transaction = _trans.Transaction;
            return cmd;
        }
        private int ExecuteNonQuery(DbCommand cmd)
        {
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                if (_conn.State == ConnectionState.Closed)
                {
                    try { _conn.Open(); }
                    catch (Exception) { }
                    return cmd.ExecuteNonQuery();
                }
                throw;
            }
        }
        private DbDataReader ExecuteReader(DbCommand cmd, CommandBehavior behavior)
        {
            try
            {
                return cmd.ExecuteReader(behavior);
            }
            catch (Exception)
            {
                if (_conn.State == ConnectionState.Closed)
                {
                    try { _conn.Open(); }
                    catch (Exception) { }
                    return cmd.ExecuteReader(behavior);
                }
                throw;
            }
        }
        private object ExecuteScalar(DbCommand cmd)
        {
            try
            {
                return cmd.ExecuteScalar();
            }
            catch (Exception)
            {
                if (_conn.State == ConnectionState.Closed)
                {
                    try { _conn.Open(); }
                    catch (Exception) { }
                    return cmd.ExecuteScalar();
                }
                throw;
            }
        }

        public void Begin()
        {
            _trans.Begin(_conn);
        }
        public void Commit()
        {
            _trans.Commit();
        }
        public void Rollback()
        {
            _trans.Rollback();
        }

        private void SetParameter(DbCommand cmd, DataParameter pair)
        {
            if (pair is DataFormatWhere && (pair.Name == null || pair.Value == null))
                return;

            DbParameter p = cmd.CreateParameter();
            p.ParameterName = pair.GetParameterName();
            //p.Value = pair.Value ?? DBNull.Value;
            if (pair.Value == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                if (pair.Value is Money)
                    p.Value = (decimal)(Money)pair.Value;
                else
                    p.Value = pair.Value;
            }
            cmd.Parameters.Add(p);
        }
        private void SetParameters(DbCommand cmd, IList<DataParameter> ps)
        {
            if (ps != null)
            {
                foreach (DataParameter pair in ps)
                    SetParameter(cmd, pair);
            }
        }

        public int ExecuteNonQuery(string sql, params DataParameter[] ps)
        {
            return ExecuteNonQuery(sql, ps, CommandType.Text);
        }
        public int ExecuteNonQuery(string sql, DataParameter[] ps, CommandType type)
        {
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);
                    return ExecuteNonQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
        }

        public dynamic ExecuteSingleRow(string sql, params DataParameter[] ps)
        {
            return ExecuteSingleRow(sql, ps, CommandType.Text);
        }
        public dynamic ExecuteSingleRow(string sql, DataParameter[] ps, CommandType type)
        {
            if (sql == null)
                throw new ArgumentNullException("sql");
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);
                    IDbReader row = null;
                    DbDataReader reader = ExecuteReader(cmd, CommandBehavior.SingleRow);
                    if (reader != null)
                    {
                        try
                        {
                            if (reader.HasRows)
                            {
                                row = new DbRow();
                                if (reader.Read())
                                    row.ReadRow(reader);
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    return row;
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
        }
        public T ExecuteSingleRow<T>(string sql, params DataParameter[] ps) where T : IDbReader, new()
        {
            return ExecuteSingleRow<T>(sql, ps, CommandType.Text);
        }
        public T ExecuteSingleRow<T>(string sql, DataParameter[] ps, CommandType type) where T : IDbReader, new()
        {
            if (sql == null)
                throw new ArgumentNullException("sql");
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);
                    T row = default(T);
                    DbDataReader reader = ExecuteReader(cmd, CommandBehavior.SingleRow);
                    if (reader != null)
                    {
                        try
                        {
                            if (reader.HasRows)
                            {
                                row = new T();
                                if (reader.Read())
                                    row.ReadRow(reader);
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    return row;
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
        }
        public bool ExecuteSingleRow<T>(T target, string sql, params DataParameter[] ps) where T : IDbReader
        {
            return ExecuteSingleRow<T>(target, sql, ps, CommandType.Text);
        }
        public bool ExecuteSingleRow<T>(T target, string sql, DataParameter[] ps, CommandType type) where T : IDbReader
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (sql == null)
                throw new ArgumentNullException("sql");
            bool result = false;
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);
                    DbDataReader reader = ExecuteReader(cmd, CommandBehavior.SingleRow);
                    if (reader != null)
                    {
                        try
                        {
                            if (reader.HasRows)
                            {
                                if (reader.Read())
                                    target.ReadRow(reader);
                                result = true;
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
            return result;
        }

        public IList<dynamic> ExecuteReader(string sql, params DataParameter[] ps)
        {
            return ExecuteReader(sql, ps, CommandType.Text);
        }
        public IList<dynamic> ExecuteReader(string sql, DataParameter[] ps, CommandType type, CommandBehavior behavior = CommandBehavior.Default)
        {
            if (sql == null)
                throw new ArgumentNullException("sql");
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);
                    List<dynamic> list = new List<dynamic>();
                    DbDataReader reader = ExecuteReader(cmd, behavior);
                    if (reader != null)
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                IDbReader row = new DbRow();
                                row.ReadRow(reader);
                                list.Add(row);
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
        }
        public IList<T> ExecuteReader<T>(string sql, params DataParameter[] ps) where T : IDbReader, new()
        {
            return ExecuteReader<T>(sql, ps, CommandType.Text);
        }
        public IList<T> ExecuteReader<T>(string sql, DataParameter[] ps, CommandType type, CommandBehavior behavior = CommandBehavior.Default) where T : IDbReader, new()
        {
            if (sql == null)
                throw new ArgumentNullException("sql");
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);
                    List<T> list = new List<T>();
                    DbDataReader reader = ExecuteReader(cmd, behavior);
                    if (reader != null)
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                T row = new T();
                                row.ReadRow(reader);
                                list.Add(row);
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
        }
        public bool ExecuteReader<T>(T target, string sql, params DataParameter[] ps) where T : IDbReader
        {
            return ExecuteReader<T>(target, sql, ps, CommandType.Text);
        }
        public bool ExecuteReader<T>(T target, string sql, DataParameter[] ps, CommandType type, CommandBehavior behavior = CommandBehavior.Default) where T : IDbReader
        {
            if (sql == null)
                throw new ArgumentNullException("sql");
            bool result = false;
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);
                    DbDataReader reader = ExecuteReader(cmd, behavior);
                    if (reader != null)
                    {
                        try
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                    target.ReadRow(reader);
                                result = true;
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
            return result;
        }

        public IList<dynamic> ExecuteReader(string sql, ref int rv, params DataParameter[] ps)
        {
            return ExecuteReader(sql, ref rv, ps, CommandType.Text);
        }
        public IList<dynamic> ExecuteReader(string sql, ref int rv, DataParameter[] ps, CommandType type, CommandBehavior behavior = CommandBehavior.Default)
        {
            if (sql == null)
                throw new ArgumentNullException("sql");
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);

                    DbParameter rvp = cmd.CreateParameter();
                    rvp.ParameterName = "@ReturnValue";
                    rvp.Direction = ParameterDirection.ReturnValue;
                    rvp.Value = 0;
                    cmd.Parameters.Add(rvp);

                    List<dynamic> list = new List<dynamic>();
                    DbDataReader reader = ExecuteReader(cmd, behavior);
                    if (reader != null)
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                IDbReader row = new DbRow();
                                row.ReadRow(reader);
                                list.Add(row);
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    foreach (DbParameter p in cmd.Parameters)
                    {
                        if (p.Direction == ParameterDirection.ReturnValue)
                        {
                            rv = (int)p.Value;
                            break;
                        }
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
        }
        public IList<T> ExecuteReader<T>(string sql, ref int rv, params DataParameter[] ps) where T : IDbReader, new()
        {
            return ExecuteReader<T>(sql, ref rv, ps, CommandType.Text);
        }
        public IList<T> ExecuteReader<T>(string sql, ref int rv, DataParameter[] ps, CommandType type, CommandBehavior behavior = CommandBehavior.Default) where T : IDbReader, new()
        {
            if (sql == null)
                throw new ArgumentNullException("sql");
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);

                    DbParameter rvp = cmd.CreateParameter();
                    rvp.ParameterName = "@ReturnValue";
                    rvp.Direction = ParameterDirection.ReturnValue;
                    rvp.Value = 0;
                    cmd.Parameters.Add(rvp);

                    List<T> list = new List<T>();
                    DbDataReader reader = ExecuteReader(cmd, behavior);
                    if (reader != null)
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                T row = new T();
                                row.ReadRow(reader);
                                list.Add(row);
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    foreach (DbParameter p in cmd.Parameters)
                    {
                        if (p.Direction == ParameterDirection.ReturnValue)
                        {
                            rv = (int)p.Value;
                            break;
                        }
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
        }
        public bool ExecuteReader<T>(T target, string sql, ref int rv, params DataParameter[] ps) where T : IDbReader
        {
            return ExecuteReader<T>(target, sql, ref rv, ps, CommandType.Text);
        }
        public bool ExecuteReader<T>(T target, string sql, ref int rv, DataParameter[] ps, CommandType type, CommandBehavior behavior = CommandBehavior.Default) where T : IDbReader
        {
            if (sql == null)
                throw new ArgumentNullException("sql");
            bool result = false;
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);

                    DbParameter rvp = cmd.CreateParameter();
                    rvp.ParameterName = "@ReturnValue";
                    rvp.Direction = ParameterDirection.ReturnValue;
                    rvp.Value = 0;
                    cmd.Parameters.Add(rvp);

                    DbDataReader reader = ExecuteReader(cmd, behavior);
                    if (reader != null)
                    {
                        try
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                    target.ReadRow(reader);
                                result = true;
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    foreach (DbParameter p in cmd.Parameters)
                    {
                        if (p.Direction == ParameterDirection.ReturnValue)
                        {
                            rv = (int)p.Value;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
            return result;
        }

        public object ExecuteScalar(string sql, params DataParameter[] ps)
        {
            return ExecuteScalar(sql, ps, CommandType.Text);
        }
        public object ExecuteScalar(string sql, DataParameter[] ps, CommandType type)
        {
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);
                    object value = ExecuteScalar(cmd);
                    if (value != null && !DBNull.Value.Equals(value))
                        return value;
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
        }
        public T ExecuteScalar<T>(string sql, params DataParameter[] ps)
        {
            return ExecuteScalar<T>(sql, ps, CommandType.Text);
        }
        public T ExecuteScalar<T>(string sql, DataParameter[] ps, CommandType type)
        {
            object value = ExecuteScalar(sql, ps, type);
            if (value != null && !DBNull.Value.Equals(value))
            {
                if (TType<T>.Type == TType<Money>.Type)
                    return (T)(object)(new Money((decimal)Convert.ChangeType(value, TType<decimal>.Type)));
                return (T)Convert.ChangeType(value, TType<T>.Type);
            }
            return default(T);
        }

        public object[] ExecuteArray(string sql, params DataParameter[] ps)
        {
            return ExecuteArray(sql, ps, CommandType.Text);
        }
        public object[] ExecuteArray(string sql, DataParameter[] ps, CommandType type)
        {
            if (sql == null)
                throw new ArgumentNullException("sql");
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);

                    List<object> list = new List<object>();
                    DbDataReader reader = ExecuteReader(cmd, CommandBehavior.Default);
                    if (reader != null && reader.FieldCount > 0)
                    {
                        try
                        {
                            object value;
                            while (reader.Read())
                            {
                                value = reader[0];
                                if (value != null && !DBNull.Value.Equals(value))
                                    list.Add(value);
                                else
                                    list.Add(null);
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    return list.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
        }
        public T[] ExecuteArray<T>(string sql, params DataParameter[] ps)
        {
            return ExecuteArray<T>(sql, ps, CommandType.Text);
        }
        public T[] ExecuteArray<T>(string sql, DataParameter[] ps, CommandType type)
        {
            if (sql == null)
                throw new ArgumentNullException("sql");
            try
            {
                using (DbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = type;
                    SetParameters(cmd, ps);

                    List<T> list = new List<T>();
                    DbDataReader reader = ExecuteReader(cmd, CommandBehavior.Default);
                    if (reader != null && reader.FieldCount > 0)
                    {
                        try
                        {
                            object value;
                            while (reader.Read())
                            {
                                value = reader[0];
                                if (value != null && !DBNull.Value.Equals(value))
                                {
                                    if (TType<T>.Type == TType<Money>.Type)
                                        list.Add((T)(object)(new Money((decimal)Convert.ChangeType(value, TType<decimal>.Type))));
                                    else
                                        list.Add((T)Convert.ChangeType(value, TType<T>.Type));
                                }
                                else
                                {
                                    list.Add(default(T));
                                }
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    return list.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new DataException(string.Concat(ex.Message, sql));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    lock (_locker.Locker)
                    {
                        _locker.Decrement();
                        if (_locker.Version == 0)
                        {
                            _trans.Dispose();
                            _conn.Dispose();
                            _conn = null;
                        }
                    }
                }
                disposed = true;
            }
        }
        ~DataSource()
        {
            Dispose(false);
        }
    }
}
