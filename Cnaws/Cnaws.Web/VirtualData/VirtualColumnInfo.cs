using Cnaws.Web.Modules;
using System;
using System.Reflection;

namespace Cnaws.Web.VirtualData
{
    internal sealed class ColumnInfo : FieldInfo
    {
        private VirtualDataColumn _column;

        public ColumnInfo(VirtualDataColumn column)
        {
            _column = column;
        }

        public override FieldAttributes Attributes
        {
            get { throw new NotImplementedException(); }
        }
        public override RuntimeFieldHandle FieldHandle
        {
            get { throw new NotImplementedException(); }
        }
        public override Type FieldType
        {
            get { return _column.Type; }
        }
        public override object GetValue(object obj)
        {
            throw new NotImplementedException();
        }
        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public override Type DeclaringType
        {
            get { throw new NotImplementedException(); }
        }
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
        public override string Name
        {
            get { throw new NotImplementedException(); }
        }
        public override Type ReflectedType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
