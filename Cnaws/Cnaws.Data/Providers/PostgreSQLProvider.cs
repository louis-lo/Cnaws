using System;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;

namespace Cnaws.Data.Providers
{
#if (PostgreSQL)
    internal sealed class PostgreSQLProvider : DataProvider
    {
        public override string EscapeString(string name)
        {
            return string.Concat("\"", name, "\"");
        }
        public override string GetMutiPrimaryKeyBegin(string table)
        {
            return string.Concat("CONSTRAINT ", EscapeName(string.Concat("PK_", table)), " ");
        }
        public override string GetInsertSqlEnd(string id)
        {
            if (!string.IsNullOrEmpty(id))
                return string.Concat(" RETURNING (", EscapeName(id), ")");
            return string.Empty;
        }
        public override string GetTruncateSql(string table)
        {
            return string.Concat("TRUNCATE TABLE ", EscapeName(table), ";");
        }
        public override bool SupperLimit
        {
            get { return true; }
        }
        public override bool IsInsertReturnId(string sql)
        {
            return sql.IndexOf("RETURNING", StringComparison.OrdinalIgnoreCase) > -1;
        }
        public override string GetVacuumSql()
        {
            return "VACUUM";
        }
        public override string GetType(Type type, int size, bool id)
        {
            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean: return "boolean";
                case TypeCode.Char: return "character(1)";
                case TypeCode.String: return (size > 0) ? string.Concat("character varying(", size, ")") : "text";
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16: return "smallint";
                case TypeCode.Int32:
                case TypeCode.UInt32: return id ? "serial" : "integer";
                case TypeCode.Int64:
                case TypeCode.UInt64: return id ? "bigserial" : "bigint";
                case TypeCode.Decimal: return "numeric";
                case TypeCode.Single: return "real";
                case TypeCode.Double: return "double precision";
                case TypeCode.DateTime: return "timestamp without time zone";
                case TypeCode.Object:
                    if (TType<Money>.Type.Equals(type)) return "money";
                    if (TType<Guid>.Type.Equals(type)) return "uuid";//binary(16)
                    if (TType<byte[]>.Type.Equals(type)) return string.Concat("bit varying(", (size > 0 ? size.ToString() : "8000"), ")");
                    break;
            }
            throw new Cnaws.Data.DataException();
        }
    }
#endif
}
