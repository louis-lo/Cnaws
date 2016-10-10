using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using Cnaws.Data;
using Cnaws.Web.VirtualData;

namespace Cnaws.Web.Modules
{
    public sealed class VirtualDataTable : IdentityModule
    {
        [DataColumn(256)]
        public string Name = null;
        private string PrimaryKey = null;
        public string[] PrimaryKeys
        {
            get
            {
                if (!string.IsNullOrEmpty(PrimaryKey))
                    return PrimaryKey.Split(',');
                else
                    return new string[0];
            }
            set
            {
                if (value != null)
                    PrimaryKey = string.Join(",", value);
                else
                    PrimaryKey = null;
            }
        }
        [DataColumn(2000)]
        public string Description = null;

        [NonSerialized]
        private List<VirtualDataColumn> columns_ = new List<VirtualDataColumn>();
        public List<VirtualDataColumn> Columns
        {
            get { return columns_; }
        }
        [NonSerialized]
        private List<VirtualDataIndex> indexs_ = new List<VirtualDataIndex>();
        public List<VirtualDataIndex> Indexs
        {
            get { return indexs_; }
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Name");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Name", "Name");
        }

        internal Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>> GetFields(string[] pks)
        {
            Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>> fields = new Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>>(columns_.Count);
            foreach (VirtualDataColumn col in columns_)
                fields.Add(col.Name, new KeyValuePair<FieldInfo, DataColumnAttribute>(new ColumnInfo(col), new DataColumnAttribute(col.Name, IsPrimaryKey(pks, col.Name), col.AutoIncrement, col.AllowDBNull, col.MaxLength, col.Unique, col.DefaultValue)));
            return fields;
        }
        //private static int VirtualDataColumnComparison(VirtualDataColumn x, VirtualDataColumn y)
        //{
        //    return x.Number - y.Number;
        //}
        private static bool IsPrimaryKey(string[] array, string name)
        {
            foreach (string key in array)
            {
                if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref Data.DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            if (columns_.Count == 0)
                return DataStatus.Failed;
            if (ExecuteCount<VirtualDataTable>(ds, P("Name", Name)) > 0)
                return DataStatus.Exist;

            string[] pks = PrimaryKeys;
            CreateTable(ds, Name, GetFields(pks), pks);

            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            foreach (VirtualDataColumn column in columns_)
            {
                column.TableId = Id;
                column.Insert(ds);
            }
            foreach (VirtualDataIndex index in indexs_)
            {
                index.TableId = Id;
                index.Insert(ds);
                CreateIndex(ds, Name, index.Name, index.Columns);
            }
            return DataStatus.Success;
        }

        private void FillData(DataSource ds)
        {
            columns_ = new List<VirtualDataColumn>(VirtualDataColumn.GetAllByTable(ds, Id));
            indexs_ = new List<VirtualDataIndex>(VirtualDataIndex.GetAllByTable(ds, Id));
        }

        public static VirtualDataTable FromId(DataSource ds, int id)
        {
            VirtualDataTable table = ExecuteSingleRow<VirtualDataTable>(ds, P("Id", id));
            if (table != null)
                table.FillData(ds);
            return table;
        }
        public static VirtualDataTable FromName(DataSource ds, string name)
        {
            VirtualDataTable table = ExecuteSingleRow<VirtualDataTable>(ds, P("Name", name));
            if (table != null) 
                table.FillData(ds);
            return table;
        }
        public static IList<VirtualDataTable> GetAll(DataSource ds)
        {
            return ExecuteReader<VirtualDataTable>(ds, Cs("Id", "Name", "Description"));
        }
        public static SplitPageData<VirtualDataTable> GetPage(DataSource ds, long index, int size, int show = 11)
        {
            long count;
            IList<VirtualDataTable> list = ExecuteReader<VirtualDataTable>(ds, Cs("Id", "Name", "Description"), Os(Oa("Id")), index, size, out count);
            return new SplitPageData<VirtualDataTable>(index, size, list, count, show);
        }
    }
}
