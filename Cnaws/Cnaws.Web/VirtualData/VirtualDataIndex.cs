using System;
using System.Collections.Generic;
using Cnaws.Data;

namespace Cnaws.Web.Modules
{
    public sealed class VirtualDataIndex : IdentityModule
    {
        [DataColumn(256)]
        public string Name = null;
        internal int TableId = 0;
        private string Column = null;
        public string[] Columns
        {
            get
            {
                if (!string.IsNullOrEmpty(Column))
                    return Column.Split(',');
                else
                    return null;
            }
            set
            {
                if (value != null)
                    Column = string.Join(",", value);
                else
                    Column = null;
            }
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "TableId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "TableId", "TableId");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            if (TableId == 0)
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Column) || Columns.Length == 0)
                return DataStatus.Failed;
            if (ExecuteCount<VirtualDataTable>(ds, P("Name", Name) & P("TableId", TableId)) > 0)
                return DataStatus.Exist;
            return DataStatus.Success;
        }

        public static IList<VirtualDataIndex> GetAllByTable(DataSource ds, int id)
        {
            return ExecuteReader<VirtualDataIndex>(ds, P("TableId", id));
        }
    }
}
