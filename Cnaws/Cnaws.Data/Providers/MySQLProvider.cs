using System;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;

namespace Cnaws.Data.Providers
{
#if(MySQL)
    internal sealed class MySQLProvider : DataProvider
    {
        public override string EscapeString(string name)
        {
            return string.Concat("`", name, "`");
        }
        public override string IdentityString
        {
            get { return " auto_increment"; }
        }
        public override string CreateSqlEnd
        {
            get { return "ENGINE=MyISAM  DEFAULT CHARSET=utf8"; }
        }
        public override string InsertOrReplaceSql
        {
            get { return "REPLACE INTO"; }
        }
        public override string GetTruncateSql(string table)
        {
            return string.Concat("TRUNCATE TABLE ", EscapeName(table), ";");
        }
        public override bool SupperReplace
        {
            get { return true; }
        }
        public override bool SupperLimit
        {
            get { return true; }
        }
        public override string GetType(Type type, int size, bool id)
        {
            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean: return "tinyint unsigned";
                case TypeCode.Char: return "char(1)";
                case TypeCode.String: return (size > 0) ? string.Concat("varchar(", size, ")") : "text";
                case TypeCode.SByte: return "tinyint";
                case TypeCode.Byte: return "tinyint unsigned";
                case TypeCode.Int16: return "smallint";
                case TypeCode.UInt16: return "smallint unsigned";
                case TypeCode.Int32: return "int";
                case TypeCode.UInt32: return "int unsigned";
                case TypeCode.Int64: return "bigint";
                case TypeCode.UInt64: return "bigint unsigned";
                case TypeCode.Decimal: return "decimal";
                case TypeCode.Single: return "float";
                case TypeCode.Double: return "double";
                case TypeCode.DateTime: return "datetime";
                case TypeCode.Object:
                    if (TType<Money>.Type.Equals(type)) return "decimal(10,4)";
                    if (TType<Guid>.Type.Equals(type)) return "char(36)";//binary(16)
                    if (TType<byte[]>.Type.Equals(type)) return string.Concat("varbinary(", (size > 0 ? size.ToString() : "8000"), ")");
                    break;
            }
            throw new Cnaws.Data.DataException();
        }
    }
#endif
}
