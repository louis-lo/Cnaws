using System;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;

namespace Cnaws.Data.Providers
{
#if(SQLite)
    internal sealed class SQLiteProvider : DataProvider
    {
        public override string IdentityString
        {
            get { return " AUTOINCREMENT"; }
        }
        public override string InsertOrReplaceSql
        {
            get { return "INSERT OR REPLACE INTO"; }
        }
        public override string GetIndexName(string table, string name)
        {
            return string.Concat(table, "_", name);
        }
        public override string GetDropIndexSqlEnd(string table)
        {
            return string.Concat(" ON ", EscapeName(table));
        }
        public override string GetTruncateSql(string table)
        {
            return string.Concat("DELETE FROM ", EscapeName(table), ";VACUUM;");
        }
        public override bool SupperReplace
        {
            get { return true; }
        }
        public override bool SupperLimit
        {
            get { return true; }
        }
        public override string GetVacuumSql()
        {
            return "VACUUM";
        }
        public override string GetType(Type type, int size, bool id)
        {
            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean: return "BOOLEAN";
                case TypeCode.Char:
                case TypeCode.String: return "TEXT";
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Decimal: return "INTEGER";
                case TypeCode.Single:
                case TypeCode.Double: return "REAL";
                case TypeCode.DateTime: return "DATETIME";
                case TypeCode.Object:
                    if (TType<Money>.Type.Equals(type)) return "MONEY";
                    if (TType<Guid>.Type.Equals(type)) return "GUID";
                    if (TType<byte[]>.Type.Equals(type)) return string.Concat("VARBINARY(", (size > 0 ? size.ToString() : "8000"), ")");
                    break;
            }
            throw new Cnaws.Data.DataException();
        }
    }
#endif
}
