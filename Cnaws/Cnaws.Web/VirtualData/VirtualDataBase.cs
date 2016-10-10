using System;
using System.Data;
using System.Data.Common;
using Cnaws.Data;
using Cnaws.Web.Modules;

namespace Cnaws.Web.VirtualData
{
    public abstract class VirtualDataBase : IDbReader
    {
        internal readonly DataSource ds_;
        internal readonly DataTable table_;
        internal readonly string[] pks_;

        public VirtualDataBase(DataSource ds, string name)
        {
            ds_ = ds;
            VirtualDataTable table = VirtualDataTable.FromName(ds_, name);
            table_ = new DataTable(table.Name);
            foreach (VirtualDataColumn column in table.Columns)
                table_.Columns.Add(column.Name, column.Type);
            pks_ = table.PrimaryKeys;
        }

        void IDbReader.ReadRow(DbDataReader reader)
        {
            DataRow row = table_.NewRow();
            foreach (System.Data.DataColumn column in table_.Columns)
                row[column.ColumnName] = reader[column.ColumnName];
        }
    }
}
