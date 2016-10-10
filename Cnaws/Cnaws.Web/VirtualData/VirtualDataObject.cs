using System;
using System.Text;
using System.Collections.Generic;
using Cnaws.Data;
using Cnaws.Web.Modules;

namespace Cnaws.Web.VirtualData
{
    public sealed class VirtualDataObject : VirtualDataBase, IVirtualDataObject
    {
        public VirtualDataObject(DataSource ds, string name)
            : base(ds, name)
        {
        }

        public object this[int index]
        {
            get { return table_.Rows[0][index]; }
        }
        public object this[string name]
        {
            get { return table_.Rows[0][name]; }
        }

        public bool IsEmpty
        {
            get { return (table_.Rows.Count == 0); }
        }

        public void InitById(params int[] ids)
        {
            if (ids == null)
                throw new ArgumentNullException("ids");
            if (pks_.Length != ids.Length)
                throw new ArgumentException("主键个数不匹配");
            table_.Rows.Clear();
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("SELECT * FROM [{0}] WHERE", table_.TableName);
            //List<DataParameter> args = new List<DataParameter>();
            //for (int i = 0; i < pks_.Length; ++i)
            //{
            //    if (i > 0) sb.Append(" AND");
            //    sb.AppendFormat(" [{0}]=@{0}", pks_[i]);
            //    args.Add(P(pks_[i], ids[i]));
            //}
            //sb.Append(" LIMIT 1;");
            //ds_.ExecuteSingleRow<VirtualDataObject>(this, sb.ToString(), args.ToArray());
        }
        public void Init(string whereAndOrder, params DataParameter[] ps)
        {
            //if (string.IsNullOrEmpty(whereAndOrder))
            //    throw new ArgumentNullException("whereAndOrder");
            //table_.Rows.Clear();
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("SELECT * FROM [{0}] WHERE ", table_.TableName);
            //sb.Append(whereAndOrder);
            //sb.Append(" LIMIT 1;");
            //ds_.ExecuteSingleRow<VirtualDataObject>(this, sb.ToString(), ps);
        }
    }
}
