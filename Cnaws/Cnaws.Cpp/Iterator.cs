using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cnaws.Cpp
{
    public sealed class Iterator<L, I> where L : IList<I>
    {
        private L _list;
        private int _index;

        public Iterator(L list)
            : this(list, 0)
        {
        }
        public Iterator(L list, int index)
        {
            if (list == null)
                throw new ArgumentNullException("value");
            if (index < 0 || index > list.Count)
                throw new IndexOutOfRangeException();
            _list = list;
            _index = index;
        }

        public int Index
        {
            get { return _index; }
        }
        public I Current
        {
            get
            {
                if (_index < _list.Count)
                    return _list[_index];
                return default(I);
            }
        }

        private Iterator<L, I> Prev()
        {
            if (--_index < 0)
                _index = 0;
            return this;
        }
        private Iterator<L, I> Next()
        {
            if (++_index > _list.Count)
                _index = _list.Count;
            return this;
        }

        public override bool Equals(object obj)
        {
            Iterator<L, I> value = obj as Iterator<L, I>;
            if (value == null)
                return false;
            if (!ReferenceEquals(_list, value._list))
                return false;
            return _index == value._index;
        }
        public override int GetHashCode()
        {
            return _list.GetHashCode() ^ _index.GetHashCode();
        }

        public static implicit operator I(Iterator<L, I> iter)
        {
            return iter.Current;
        }

        public static Iterator<L, I> operator ++(Iterator<L, I> iter)
        {
            return iter.Next();
        }
        public static Iterator<L, I> operator --(Iterator<L, I> iter)
        {
            return iter.Prev();
        }

        public static int operator +(Iterator<L, I> left, Iterator<L, I> right)
        {
            return left.Index - right.Index;
        }
        public static int operator -(Iterator<L, I> left, Iterator<L, I> right)
        {
            return left.Index - right.Index;
        }

        public static bool operator ==(Iterator<L, I> left, Iterator<L, I> right)
        {
            if (left == null && right == null)
                return true;
            if (left == null || right == null)
                return false;
            return left.Equals(right);
        }
        public static bool operator !=(Iterator<L, I> left, Iterator<L, I> right)
        {
            return !(left == right);
        }
    }
}
