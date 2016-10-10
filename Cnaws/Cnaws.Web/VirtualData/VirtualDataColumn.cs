using System;
using System.Data;
using System.Collections.Generic;
using Cnaws.Data;
using Cnaws.Templates;

namespace Cnaws.Web.Modules
{
    public sealed class VirtualDataColumn : IdentityModule
    {
        [DataColumn(256)]
        public string Name = null;
        [DataColumn(2000)]
        public string Caption = null;
        internal int TableId = 0;
        internal DbType DataType = DbType.String;
        public Type Type
        {
            get
            {
                switch (DataType)
                {
                    case DbType.Byte: return TType<byte>.Type;
                    case DbType.Boolean: return TType<bool>.Type;
                    case DbType.DateTime: return TType<DateTime>.Type;
                    case DbType.Decimal: return TType<decimal>.Type;
                    case DbType.Double: return TType<double>.Type;
                    case DbType.Guid: return TType<Guid>.Type;
                    case DbType.Int16: return TType<short>.Type;
                    case DbType.Int32: return TType<int>.Type;
                    case DbType.Int64: return TType<long>.Type;
                    case DbType.SByte: return TType<sbyte>.Type;
                    case DbType.Single: return TType<float>.Type;
                    case DbType.String: return TType<string>.Type;
                    case DbType.UInt16: return TType<ushort>.Type;
                    case DbType.UInt32: return TType<uint>.Type;
                    case DbType.UInt64: return TType<ulong>.Type;
                    default: throw new Cnaws.Data.DataException();
                }
            }
            set
            {
                if (TType<byte>.Type.Equals(value)) DataType = DbType.Byte;
                else if (TType<bool>.Type.Equals(value)) DataType = DbType.Boolean;
                else if (TType<DateTime>.Type.Equals(value)) DataType = DbType.DateTime;
                else if (TType<decimal>.Type.Equals(value)) DataType = DbType.Decimal;
                else if (TType<double>.Type.Equals(value)) DataType = DbType.Double;
                else if (TType<Guid>.Type.Equals(value)) DataType = DbType.Guid;
                else if (TType<short>.Type.Equals(value)) DataType = DbType.Int16;
                else if (TType<int>.Type.Equals(value)) DataType = DbType.Int32;
                else if (TType<long>.Type.Equals(value)) DataType = DbType.Int64;
                else if (TType<sbyte>.Type.Equals(value)) DataType = DbType.SByte;
                else if (TType<float>.Type.Equals(value)) DataType = DbType.Single;
                else if (TType<string>.Type.Equals(value)) DataType = DbType.String;
                else if (TType<ushort>.Type.Equals(value)) DataType = DbType.UInt16;
                else if (TType<uint>.Type.Equals(value)) DataType = DbType.UInt32;
                else if (TType<ulong>.Type.Equals(value)) DataType = DbType.UInt64;
                else throw new Cnaws.Data.DataException();
                DefaultValue = null;
            }
        }
        public int MaxLength = 0;
        public bool AutoIncrement = false;
        public bool AllowDBNull = true;
        public bool Unique = false;
        internal string DefaultValue = null;
        public object Value
        {
            get
            {
                switch (DataType)
                {
                    case DbType.Boolean: return DefaultValue == "1";
                    case DbType.Guid: return Guid.Parse(DefaultValue);
                    default: return Convert.ChangeType(DefaultValue, Type);
                }
            }
            set
            {
                if (value != null)
                {
                    switch (DataType)
                    {
                        case DbType.Boolean: DefaultValue = ((bool)value) ? "1" : "0"; break;
                        case DbType.Guid: DefaultValue = ((Guid)value).ToString(); break;
                        default: DefaultValue = (string)Convert.ChangeType(value, TType<string>.Type); break;
                    }
                }
                else
                {
                    DefaultValue = null;
                }
            }
        }
        public int Number = 0;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "TableId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "TableId", "TableId");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref Data.DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            if (TableId == 0)
                return DataStatus.Failed;
            if (ExecuteCount<VirtualDataTable>(ds, P("Name", Name) & P("TableId", TableId)) > 0)
                return DataStatus.Exist;
            return DataStatus.Success;
        }

        public static IList<VirtualDataColumn> GetAllByTable(DataSource ds, int id)
        {
            return ExecuteReader<VirtualDataColumn>(ds, Os(Oa("Number")), P("TableId", id));
        }
    }
}
