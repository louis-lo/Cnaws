using System;
using System.Collections.Generic;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;

namespace Cnaws.Web
{
    public sealed class Arguments
    {
        private List<string> _array;
        private int _index;

        public Arguments(List<string> array, int index)
        {
            _array = array;
            _index = index;
        }
        internal void SetIndex(int step)
        {
            _index += step;
            if (_index < 0)
                _index = 0;
            if (_index > _array.Count)
                _index = _array.Count;
        }

        public int Count
        {
            get
            {
                return _array.Count - _index;
            }
        }

        public string this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                    return _array[_index + index];
                return null;
            }
        }

        public T Get<T>(int index)
        {
            if (index >= 0 && index < Count)
            {
                string s = _array[_index + index];
                if (TType<T>.Type == TType<string>.Type)
                    return (T)(object)s;

                if (string.IsNullOrEmpty(s))
                    return default(T);

                return (T)TType<T>.Type.GetObjectFromString(s);
            }
            return default(T);
        }
        public object Get(int index, Type type)
        {
            if (index >= 0 && index < Count)
            {
                string s = _array[_index + index];
                if (type == TType<string>.Type)
                    return s;

                if (string.IsNullOrEmpty(s))
                    return type.GetDefaultValue();

                return type.GetObjectFromString(s);
            }
            return type.GetDefaultValue();
        }
        public string[] ToArray()
        {
            int count = Count;
            string[] array = new string[count];
            if (count > 0)
                _array.CopyTo(_index, array, 0, count);
            return array;
        }
        public void CopyTo(string[] array, int index)
        {
            _array.CopyTo(_index, array, index, Count);
        }
    }
}
