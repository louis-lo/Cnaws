using Cnaws.ExtensionMethods;
using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Collections;

namespace Cnaws.Data
{
    [Serializable]
    public sealed class DataJoin<TA, TB> : IDictionary, IDictionary<string, object>, IDbReader where TA : IDbReader, new() where TB : IDbReader, new()
    {
        private TA a;
        private TB b;
        private Dictionary<string, object> items;

        public DataJoin()
        {
            a = new TA();
            b = new TB();
            items = new Dictionary<string, object>();
        }
        public DataJoin(TA a, TB b)
        {
            this.a = a;
            this.b = b;
            items = new Dictionary<string, object>();
        }
        public DataJoin(TA a, TB b, Dictionary<string, object> items)
        {
            this.a = a;
            this.b = b;
            this.items = items;
        }

        public TA A
        {
            get { return a; }
        }
        public TB B
        {
            get { return b; }
        }

        public T Get<T>(string key)
        {
            object value = this[key];
            if (key == null || DBNull.Value.Equals(value))
                return default(T);
            return (T)value;
        }
        public bool GetBoolean(string key)
        {
            return Get<bool>(key);
        }
        public short GetInt16(string key)
        {
            return Get<short>(key);
        }
        public ushort GetUInt16(string key)
        {
            return Get<ushort>(key);
        }
        public int GetInt32(string key)
        {
            return Get<int>(key);
        }
        public uint GetUInt32(string key)
        {
            return Get<uint>(key);
        }
        public long GetInt64(string key)
        {
            return Get<long>(key);
        }
        public ulong GetUInt64(string key)
        {
            return Get<ulong>(key);
        }
        public string GetString(string key)
        {
            return Get<string>(key);
        }

        private string[] GetKeys()
        {
            int index = 0;
            Dictionary<string, FieldInfo> fa = TAllNameSetFields<TA, DataColumnAttribute>.Fields;
            Dictionary<string, FieldInfo> fb = TAllNameSetFields<TB, DataColumnAttribute>.Fields;
            string[] array = new string[fa.Count + fb.Count + items.Count];
            if (fa.Keys.Count > 0)
            {
                fa.Keys.CopyTo(array, index);
                for (int i = index; i < (index + fa.Keys.Count); ++i)
                    array[i] = string.Concat("a_", array[i]);
                index += fa.Keys.Count;
            }
            if (fb.Keys.Count > 0)
            {
                fb.Keys.CopyTo(array, index);
                for (int i = index; i < (index + fb.Keys.Count); ++i)
                    array[i] = string.Concat("b_", array[i]);
                index += fb.Keys.Count;
            }
            if (items.Keys.Count > 0)
                items.Keys.CopyTo(array, index);
            return array;
        }
        private List<object> GetValues()
        {
            Dictionary<string, FieldInfo> fa = TAllNameSetFields<TA, DataColumnAttribute>.Fields;
            Dictionary<string, FieldInfo> fb = TAllNameSetFields<TB, DataColumnAttribute>.Fields;
            List<object> list = new List<object>(fa.Count + fb.Count + items.Count);
            foreach (FieldInfo fi in fa.Values)
                list.Add(fi.GetValue(a));
            foreach (FieldInfo fi in fb.Values)
                list.Add(fi.GetValue(b));
            list.AddRange(items.Values);
            return list;
        }
        private bool ContainsKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key.Length > 2)
            {
                if (key.StartsWith("a_"))
                {
                    Dictionary<string, FieldInfo> fa = TAllNameSetFields<TA, DataColumnAttribute>.Fields;
                    if (fa.ContainsKey(key.Substring(2)))
                        return true;
                }
                if (key.StartsWith("b_"))
                {
                    Dictionary<string, FieldInfo> fb = TAllNameSetFields<TB, DataColumnAttribute>.Fields;
                    if (fb.ContainsKey(key.Substring(2)))
                        return true;
                }
            }
            return items.ContainsKey(key);
        }
        private void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            Dictionary<string, FieldInfo> fa = TAllNameSetFields<TA, DataColumnAttribute>.Fields;
            Dictionary<string, FieldInfo> fb = TAllNameSetFields<TB, DataColumnAttribute>.Fields;
            foreach (KeyValuePair<string, FieldInfo> item in fa)
                array[arrayIndex++] = new KeyValuePair<string, object>(string.Concat("a_", item.Key), item.Value.GetValue(a));
            foreach (KeyValuePair<string, FieldInfo> item in fb)
                array[arrayIndex++] = new KeyValuePair<string, object>(string.Concat("b_", item.Key), item.Value.GetValue(b));
            foreach (KeyValuePair<string, object> item in items)
                array[arrayIndex++] = new KeyValuePair<string, object>(item.Key, item.Value);
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get
            {
                return GetKeys();
            }
        }
        private KeyValuePair<string, object> this[int index]
        {
            get
            {
                if (index >= 0)
                {
                    int i = 0;
                    Dictionary<string, FieldInfo> fa = TAllNameSetFields<TA, DataColumnAttribute>.Fields;
                    if (index < fa.Count)
                    {
                        foreach (KeyValuePair<string, FieldInfo> item in fa)
                        {
                            if (i++ == index)
                                return new KeyValuePair<string, object>(string.Concat("a_", item.Key), item.Value.GetValue(a));
                        }
                    }
                    Dictionary<string, FieldInfo> fb = TAllNameSetFields<TB, DataColumnAttribute>.Fields;
                    if (index < (fa.Count + fb.Count))
                    {
                        i = fa.Count;
                        foreach (KeyValuePair<string, FieldInfo> item in fb)
                        {
                            if (i++ == index)
                                return new KeyValuePair<string, object>(string.Concat("b_", item.Key), item.Value.GetValue(b));
                        }
                    }
                    if (index < (fa.Count + fb.Count + items.Count))
                    {
                        i = fa.Count + fb.Count;
                        foreach (KeyValuePair<string, object> item in items)
                        {
                            if (i++ == index)
                                return new KeyValuePair<string, object>(item.Key, item.Value);
                        }
                    }
                }
                throw new IndexOutOfRangeException();
            }
        }
        ICollection<object> IDictionary<string, object>.Values
        {
            get
            {
                return GetValues();
            }
        }
        public int Count
        {
            get
            {
                Dictionary<string, FieldInfo> fa = TAllNameSetFields<TA, DataColumnAttribute>.Fields;
                Dictionary<string, FieldInfo> fb = TAllNameSetFields<TB, DataColumnAttribute>.Fields;
                return fa.Count + fb.Count + items.Count;
            }
        }
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get
            {
                return true;
            }
        }
        ICollection IDictionary.Keys
        {
            get
            {
                return GetKeys();
            }
        }
        ICollection IDictionary.Values
        {
            get
            {
                return GetValues();
            }
        }
        bool IDictionary.IsReadOnly
        {
            get
            {
                return true;
            }
        }
        bool IDictionary.IsFixedSize
        {
            get
            {
                return true;
            }
        }
        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }
        bool ICollection.IsSynchronized
        {
            get
            {
                return true;
            }
        }
        object IDictionary.this[object key]
        {
            get
            {
                return this[key as string];
            }
            set
            {
            }
        }
        object IDictionary<string, object>.this[string key]
        {
            get
            {
                return this[key];
            }
            set
            {
            }
        }
        public object this[string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                if (key.Length > 2)
                {
                    if (key.StartsWith("a_"))
                    {
                        FieldInfo fi;
                        Dictionary<string, FieldInfo> fa = TAllNameSetFields<TA, DataColumnAttribute>.Fields;
                        if (fa.TryGetValue(key.Substring(2), out fi))
                            return fi.GetValue(a);
                    }
                    if (key.StartsWith("b_"))
                    {
                        FieldInfo fi;
                        Dictionary<string, FieldInfo> fb = TAllNameSetFields<TB, DataColumnAttribute>.Fields;
                        if (fb.TryGetValue(key.Substring(2), out fi))
                            return fi.GetValue(b);
                    }
                }
                return items[key];
            }
        }
        void IDbReader.ReadRow(DbDataReader reader)
        {
            object v;
            FieldInfo f;

            Dictionary<string, bool> keys = new Dictionary<string, bool>(reader.FieldCount);
            for (int i = 0; i < reader.FieldCount; i++)
                keys.Add(reader.GetName(i), false);

            string temp;
            bool unused;
            string table = DbTable.GetTableName<TA>();
            Dictionary<string, FieldInfo> fs = TAllNameSetFields<TA, DataColumnAttribute>.Fields;
            foreach (string key in fs.Keys)
            {
                try
                {
                    temp = string.Concat(table, '_', key);
                    if (!keys.TryGetValue(temp, out unused))
                        temp = key;
                    if (keys.TryGetValue(temp, out unused))
                    {
                        f = fs[key];
                        v = DataUtility.FromDataType(reader[temp], f.FieldType);
                        f.SetValue(a, v);
                        keys[temp] = true;
                    }
                }
                catch (IndexOutOfRangeException) { }
            }

            table = DbTable.GetTableName<TB>();
            fs = TAllNameSetFields<TB, DataColumnAttribute>.Fields;
            foreach (string key in fs.Keys)
            {
                try
                {
                    temp = string.Concat(table, '_', key);
                    if (!keys.TryGetValue(temp, out unused))
                        temp = key;
                    if (keys.TryGetValue(temp, out unused))
                    {
                        f = fs[key];
                        v = DataUtility.FromDataType(reader[temp], f.FieldType);
                        f.SetValue(b, v);
                        keys[temp] = true;
                    }
                }
                catch (IndexOutOfRangeException) { }
            }

            foreach(KeyValuePair<string,bool> pair in keys)
            {
                if (!pair.Value)
                    items.Add(pair.Key, reader[pair.Key]);
            }
        }
        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return ContainsKey(key);
        }
        void IDictionary<string, object>.Add(string key, object value)
        {
        }
        bool IDictionary<string, object>.Remove(string key)
        {
            return false;
        }
        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            if(key != null)
            {
                if (key.Length > 2)
                {
                    if (key.StartsWith("a_"))
                    {
                        FieldInfo fi;
                        Dictionary<string, FieldInfo> fa = TAllNameSetFields<TA, DataColumnAttribute>.Fields;
                        if (fa.TryGetValue(key.Substring(2), out fi))
                        {
                            value = fi.GetValue(a);
                            return true;
                        }
                    }
                    if (key.StartsWith("b_"))
                    {
                        FieldInfo fi;
                        Dictionary<string, FieldInfo> fb = TAllNameSetFields<TB, DataColumnAttribute>.Fields;
                        if (fb.TryGetValue(key.Substring(2), out fi))
                        {
                            value = fi.GetValue(b);
                            return true;
                        }
                    }
                }
                return items.TryGetValue(key, out value);
            }
            value = null;
            return false;
        }
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
        }
        void ICollection<KeyValuePair<string, object>>.Clear()
        {
        }
        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return false;
        }
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return false;
        }
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return new DataJoinEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DataJoinEnumerator(this);
        }
        bool IDictionary.Contains(object key)
        {
            return ContainsKey(key as string);
        }
        void IDictionary.Add(object key, object value)
        {
        }
        void IDictionary.Clear()
        {
        }
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DataJoinEnumerator(this);
        }
        void IDictionary.Remove(object key)
        {
        }
        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((KeyValuePair<string, object>[])array, index);
        }

        private sealed class DataJoinEnumerator : IDictionaryEnumerator, IEnumerator<KeyValuePair<string, object>>
        {
            private DataJoin<TA, TB> _data;
            private int _index;
            private KeyValuePair<string, object> _current;

            public DataJoinEnumerator(DataJoin<TA, TB> data)
            {
                _data = data;
                _index = 0;
                _current = new KeyValuePair<string, object>();
            }

            public KeyValuePair<string, object> Current
            {
                get
                {
                    return _current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _current;
                }
            }

            public object Key
            {
                get
                {
                    return _current.Key;
                }
            }

            public object Value
            {
                get
                {
                    return _current.Value;
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    return new DictionaryEntry(_current.Key, _current.Value);
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                while (_index < _data.Count)
                {
                    _current = _data[_index++];
                    return true;
                }
                _index = _data.Count + 1;
                _current = new KeyValuePair<string, object>();
                return false;
            }

            public void Reset()
            {
                _index = 0;
                _current = new KeyValuePair<string, object>();
            }
        }
    }
}
