using System;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;
using System.Text.RegularExpressions;

namespace Cnaws.Data.Providers
{
    internal sealed class MSSQLProvider : DataProvider
    {
        public static readonly Regex ORDER_REGEX = new Regex(@"(A|DE)SC", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public override string IdentityString
        {
            get { return " IDENTITY(1,1)"; }
        }
        public override string GetMutiPrimaryKeyBegin(string table)
        {
            return string.Concat("CONSTRAINT ", EscapeName(string.Concat("PK_", table)), " ");
        }
        public override string GetInsertSqlEnd(string id)
        {
            if (!string.IsNullOrEmpty(id))
                return ";SELECT @@IDENTITY";
            return string.Empty;
        }
        public override string GetTruncateSql(string table)
        {
            return string.Concat("TRUNCATE TABLE ", EscapeName(table), ";");
        }
        public override bool SupperTop
        {
            get { return true; }
        }
        public override bool SupperRowNumber
        {
            get { return true; }
        }
        public override bool IsInsertReturnId(string sql)
        {
            return sql.IndexOf("@@IDENTITY", StringComparison.OrdinalIgnoreCase) > -1;
        }
        public override string GetType(Type type, int size, bool id)
        {
            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean: return "bit";
                case TypeCode.Char: return "nchar(1)";
                case TypeCode.String: return string.Concat("nvarchar(", (size > 0 ? size.ToString() : "max"), ")");
                case TypeCode.SByte:
                case TypeCode.Byte: return "tinyint";
                case TypeCode.Int16:
                case TypeCode.UInt16: return "smallint";
                case TypeCode.Int32:
                case TypeCode.UInt32: return "int";
                case TypeCode.Int64:
                case TypeCode.UInt64: return "bigint";
                case TypeCode.Decimal: return "decimal";
                case TypeCode.Single: return "float(24)";
                case TypeCode.Double: return "float(53)";
                case TypeCode.DateTime: return "datetime";
                case TypeCode.Object:
                    if (TType<Money>.Type.Equals(type)) return "money";
                    if (TType<Guid>.Type.Equals(type)) return "uniqueidentifier";
                    if (TType<byte[]>.Type.Equals(type)) return string.Concat("varbinary(", (size > 0 ? size.ToString() : "max"), ")");
                    break;
            }
            throw new Cnaws.Data.DataException();
        }

        public override string GetTable(string table)
        {
            return string.Concat("SELECT COUNT(*) FROM sysobjects WHERE [type]=N'U' AND [name]=", EscapeName(table), ';');
        }
        public override string GetColumn(string table, string column)
        {
            return string.Concat("SELECT COUNT(*) FROM syscolumns WHERE [name]=", EscapeName(column), " AND [id]=object_id(", EscapeName(table), ");");
        }
        public override string GetIndex(string table, string name, string[] columns)
        {
            return string.Concat("SELECT COUNT(*) FROM sysindexes a JOIN sysindexkeys b ON a.id=b.id AND a.indid=b.indid JOIN sysobjects c ON b.id=c.id JOIN syscolumns d ON b.id=d.id AND b.colid=d.colid WHERE a.name=", EscapeName(GetIndexNameImpl(table, name)), " AND c.name=", EscapeName(table), ';');
        }
    }
}
