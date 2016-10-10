using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using Cnaws.ExtensionMethods;

namespace Cnaws.Data
{
    [Serializable]
    public class DbRow : DynamicObject, IDictionary, IDictionary<string, object>, IDbReader
    {
        [Serializable]
        private sealed class DbRowEx : DbRow
        {
            private string _table;

            public DbRowEx(string table, DbRow row)
            {
                _table = string.Concat(table, '_');
                _dict = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> item in row._dict)
                {
                    if (item.Key.StartsWith(_table))
                        _dict.Add(item.Key, item.Value);
                }
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                return _dict.TryGetValue(string.Concat(_table, binder.Name), out result);
            }
        }
        
        private Dictionary<string, object> _dict;

        internal DbRow()
        {
            _dict = new Dictionary<string, object>();
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dict.Keys;
        }
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            return base.TryBinaryOperation(binder, arg, out result);
        }
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            try
            {
                string name;
                object value;
                result = Activator.CreateInstance(binder.Type, false);
                string table = DbTable.GetTableName(binder.Type);
                Dictionary<string, FieldInfo> fields = binder.Type.GetStaticAllNameSetFields<DataColumnAttribute>();
                foreach (KeyValuePair<string, FieldInfo> pair in fields)
                {
                    if (_dict.TryGetValue(pair.Key, out value))
                    {
                        pair.Value.SetValue(result, DataUtility.FromDataType(value, pair.Value.FieldType));
                    }
                    else
                    {
                        name = string.Concat(table, '_', pair.Key);
                        if (_dict.TryGetValue(name, out value))
                            pair.Value.SetValue(result, DataUtility.FromDataType(value, pair.Value.FieldType));
                    }
                }
                return true;
            }
            catch (Exception) { }
            result = null;
            return false;
        }
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length > 0)
            {
                object index = indexes[0];
                if (index is string)
                    return _dict.TryGetValue((string)index, out result);
            }
            result = null;
            return false;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_dict.TryGetValue(binder.Name, out result))
                return true;

            string key = string.Concat(binder.Name, '_');
            foreach (KeyValuePair<string, object> item in _dict)
            {
                if (item.Key.StartsWith(key) && item.Key.Length > key.Length)
                {
                    result = new DbRowEx(binder.Name, this);
                    _dict[binder.Name] = result;
                    return true;
                }
            }

            return false;
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            return base.TryInvokeMember(binder, args, out result);
        }
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length > 0)
            {
                object index = indexes[0];
                if (index is string)
                {
                    _dict[(string)index] = value;
                    return true;
                }
            }
            return false;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dict[binder.Name] = value;
            return true;
        }
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            return base.TryUnaryOperation(binder, out result);
        }
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }

        #region interface
        object IDictionary<string, object>.this[string key]
        {
            get
            {
                return ((IDictionary<string, object>)_dict)[key];
            }
            set
            {
                ((IDictionary<string, object>)_dict)[key] = value;
            }
        }
        object IDictionary.this[object key]
        {
            get
            {
                return ((IDictionary)_dict)[key];
            }
            set
            {
                ((IDictionary)_dict)[key] = value;
            }
        }
        int ICollection<KeyValuePair<string, object>>.Count
        {
            get
            {
                return ((ICollection<KeyValuePair<string, object>>)_dict).Count;
            }
        }
        int ICollection.Count
        {
            get
            {
                return ((ICollection)_dict).Count;
            }
        }
        bool IDictionary.IsFixedSize
        {
            get
            {
                return ((IDictionary)_dict).IsFixedSize;
            }
        }
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get
            {
                return ((ICollection<KeyValuePair<string, object>>)_dict).IsReadOnly;
            }
        }
        bool IDictionary.IsReadOnly
        {
            get
            {
                return ((IDictionary)_dict).IsReadOnly;
            }
        }
        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)_dict).IsSynchronized;
            }
        }
        ICollection<string> IDictionary<string, object>.Keys
        {
            get
            {
                return ((IDictionary<string, object>)_dict).Keys;
            }
        }
        ICollection IDictionary.Keys
        {
            get
            {
                return ((IDictionary)_dict).Keys;
            }
        }
        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)_dict).SyncRoot;
            }
        }
        ICollection<object> IDictionary<string, object>.Values
        {
            get
            {
                return ((IDictionary<string, object>)_dict).Values;
            }
        }
        ICollection IDictionary.Values
        {
            get
            {
                return ((IDictionary)_dict).Values;
            }
        }
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            ((ICollection<KeyValuePair<string, object>>)_dict).Add(item);
        }
        void IDictionary<string, object>.Add(string key, object value)
        {
            ((IDictionary<string, object>)_dict).Add(key, value);
        }
        void IDictionary.Add(object key, object value)
        {
            ((IDictionary)_dict).Add(key, value);
        }
        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            ((ICollection<KeyValuePair<string, object>>)_dict).Clear();
        }
        void IDictionary.Clear()
        {
            ((IDictionary)_dict).Clear();
        }
        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_dict).Contains(item);
        }
        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)_dict).Contains(key);
        }
        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return ((IDictionary<string, object>)_dict).ContainsKey(key);
        }
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)_dict).CopyTo(array, arrayIndex);
        }
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_dict).CopyTo(array, index);
        }
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object>>)_dict).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dict).GetEnumerator();
        }
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_dict).GetEnumerator();
        }
        void IDbReader.ReadRow(DbDataReader reader)
        {
            string key;
            for (int i = 0; i < reader.FieldCount; ++i)
            {
                key = reader.GetName(i);
                _dict.Add(key, reader[i]);
            }
        }
        bool IDictionary<string, object>.Remove(string key)
        {
            return ((IDictionary<string, object>)_dict).Remove(key);
        }
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_dict).Remove(item);
        }
        void IDictionary.Remove(object key)
        {
            ((IDictionary)_dict).Remove(key);
        }
        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return ((IDictionary<string, object>)_dict).TryGetValue(key, out value);
        }
        #endregion
    }
}
