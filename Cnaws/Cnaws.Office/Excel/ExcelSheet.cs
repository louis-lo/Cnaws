using Cnaws.ExtensionMethods;
using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using E = Microsoft.Office.Interop.Excel;

namespace Cnaws.Office.Excel
{
    public sealed class BindColumnEventArgs : CancelEventArgs
    {
        private bool _isHeader;
        private string _name;
        private object _value;

        public BindColumnEventArgs(bool isHeader, string name, object value, bool cancel)
            : base(cancel)
        {
            _isHeader = isHeader;
            _name = name;
            _value = value;
        }

        public bool IsHeader
        {
            get { return _isHeader; }
        }
        public string Name
        {
            get { return _name; }
        }
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }

    public delegate void BindColumnEventHandler(object sender, BindColumnEventArgs e);

    public sealed class ExcelSheet
    {
        private int _row;
        private int _column;
        private E.Worksheet _sheet;

        internal ExcelSheet(E.Worksheet sheet)
        {
            _row = 1;
            _column = 1;
            _sheet = sheet;
        }

        public event BindColumnEventHandler BindColumn;

        public string Name
        {
            get { return _sheet.Name; }
            set { _sheet.Name = value; }
        }

        public void Fill<T>(IList<T> list, bool head = false)
        {
            _row = 1;
            _column = 1;

            if (TType<T>.Type.IsAnonymousType())
            {
                List<KeyValuePair<string, PropertyInfo>> plist = GetProperties<T>();

                if (head)
                    SetHeader(plist);

                for (int i = 0; i < list.Count; ++i)
                    SetRow(list[i], plist);
            }
            else
            {
                List<KeyValuePair<string, FieldInfo>> flist = GetFields<T>();

                if (head)
                    SetHeader(flist);

                for (int i = 0; i < list.Count; ++i)
                    SetRow(list[i], flist);
            }
        }

        private List<KeyValuePair<string, PropertyInfo>> GetProperties<T>()
        {
            Dictionary<string, PropertyInfo> ps = TProperties<T>.Properties;
            List<KeyValuePair<string, PropertyInfo>> list = new List<KeyValuePair<string, PropertyInfo>>(ps.Count);
            foreach (KeyValuePair<string, PropertyInfo> pair in ps)
                list.Add(pair);
            return list;
        }
        private List<KeyValuePair<string, FieldInfo>> GetFields<T>()
        {
            Dictionary<string, FieldInfo> fs = TFields<T>.Fields;
            List<KeyValuePair<string, FieldInfo>> list = new List<KeyValuePair<string, FieldInfo>>(fs.Count);
            foreach (KeyValuePair<string, FieldInfo> pair in fs)
                list.Add(pair);
            return list;
        }

        private void SetColumn(bool isHeader, string name, object value)
        {
            BindColumnEventArgs e = new BindColumnEventArgs(isHeader, name, value, false);
            BindColumn?.Invoke(this, e);
            if (!e.Cancel)
            {
                _sheet.Cells[_row, _column] = e.Value;
                ++_column;
            }
        }
        private void SetHeader(List<KeyValuePair<string, PropertyInfo>> list)
        {
            KeyValuePair<string, PropertyInfo> pair;
            for (int i = 0; i < list.Count; ++i)
            {
                pair = list[i];
                SetColumn(true, pair.Key, pair.Key);
            }
            ++_row;
            _column = 1;
        }
        private void SetHeader(List<KeyValuePair<string, FieldInfo>> list)
        {
            KeyValuePair<string, FieldInfo> pair;
            for (int i = 0; i < list.Count; ++i)
            {
                pair = list[i];
                SetColumn(true, pair.Key, pair.Key);
            }
            ++_row;
            _column = 1;
        }
        private void SetRow<T>(T value, List<KeyValuePair<string, PropertyInfo>> list)
        {
            KeyValuePair<string, PropertyInfo> pair;
            for (int i = 0; i < list.Count; ++i)
            {
                pair = list[i];
                SetColumn(false, pair.Key, pair.Value.GetValue(value));
            }
            ++_row;
            _column = 1;
        }
        private void SetRow<T>(T value, List<KeyValuePair<string, FieldInfo>> list)
        {
            KeyValuePair<string, FieldInfo> pair;
            for (int i = 0; i < list.Count; ++i)
            {
                pair = list[i];
                SetColumn(false, pair.Key, pair.Value.GetValue(value));
            }
            ++_row;
            _column = 1;
        }
    }
}
