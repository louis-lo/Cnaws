using System;

namespace Cnaws.Data.Query
{
    public abstract class DbQueryElement
    {
        private DbQueryElement _parent;

        protected DbQueryElement(DbQueryElement parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            _parent = parent;
        }

        public DbQueryElement Parent
        {
            get { return _parent; }
        }

        internal void Build(DbQueryBuilder builder)
        {
            _parent.Build(builder);
            OnBuild(builder);
        }
        internal abstract void OnBuild(DbQueryBuilder builder);
    }

    public abstract class DbExpressionQueryElement : DbQueryElement
    {
        private DbExpression _expression;

        internal DbExpressionQueryElement(DbQueryElement parent, DbExpression expression)
            : base(parent)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            _expression = expression;
        }

        internal DbExpression Expression
        {
            get { return _expression; }
        }
    }
}
