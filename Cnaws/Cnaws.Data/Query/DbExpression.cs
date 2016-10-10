using System;
using System.Collections.Generic;
using System.Text;

namespace Cnaws.Data.Query
{
    internal sealed class DbExpressionBuilder
    {
        private DataSource _ds;
        private StringBuilder _builder;
        private List<DataParameter> _parameters;

        public DbExpressionBuilder(DataSource ds)
        {
            if (ds == null)
                throw new ArgumentNullException("ds");
            _ds = ds;
            _builder = new StringBuilder();
            _parameters = new List<DataParameter>();
        }

        public DataSource DataSource
        {
            get { return _ds; }
        }

        public DbExpressionBuilder Append(char value)
        {
            _builder.Append(value);
            return this;
        }
        public DbExpressionBuilder Append(int value)
        {
            _builder.Append(value);
            return this;
        }
        public DbExpressionBuilder Append(long value)
        {
            _builder.Append(value);
            return this;
        }
        public DbExpressionBuilder Append(string value)
        {
            if (value != null && value.Length > 0)
                _builder.Append(value);
            return this;
        }
        public DbExpressionBuilder Append(params DataParameter[] value)
        {
            if (value != null && value.Length > 0)
                _parameters.AddRange(value);
            return this;
        }
    }

    public abstract class DbExpression
    {
        internal abstract void Build(DbExpressionBuilder builder);
    }
    internal sealed class DbCharExpression : DbExpression
    {
        private char _expression;

        public DbCharExpression(char expression)
        {
            _expression = expression;
        }

        public char Expression
        {
            get { return _expression; }
        }

        internal override void Build(DbExpressionBuilder builder)
        {
            builder.Append(_expression);
        }
    }
    internal sealed class DbInt32Expression : DbExpression
    {
        private int _expression;

        public DbInt32Expression(int expression)
        {
            _expression = expression;
        }

        public int Expression
        {
            get { return _expression; }
        }

        internal override void Build(DbExpressionBuilder builder)
        {
            builder.Append(_expression);
        }
    }
    internal sealed class DbInt64Expression : DbExpression
    {
        private long _expression;

        public DbInt64Expression(long expression)
        {
            _expression = expression;
        }

        public long Expression
        {
            get { return _expression; }
        }

        internal override void Build(DbExpressionBuilder builder)
        {
            builder.Append(_expression);
        }
    }
    internal class DbStringExpression : DbExpression
    {
        private string _expression;

        public DbStringExpression(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            _expression = expression;
        }

        public string Expression
        {
            get { return _expression; }
        }

        internal override void Build(DbExpressionBuilder builder)
        {
            builder.Append(_expression);
        }
    }
    internal sealed class DbParameterExpression : DbStringExpression
    {
        private DataParameter[] _parameters;

        public DbParameterExpression(string expression, params DataParameter[] parameters)
            : base(expression)
        {
            _parameters = parameters;
        }

        public DataParameter[] Parameters
        {
            get { return _parameters; }
        }

        internal override void Build(DbExpressionBuilder builder)
        {
            base.Build(builder);
            builder.Append(_parameters);
        }
    }
}
