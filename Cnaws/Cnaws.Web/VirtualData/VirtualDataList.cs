using System;
using System.Collections;
using System.Collections.Generic;
using Cnaws.Data;

namespace Cnaws.Web.VirtualData
{
    public sealed class VirtualDataList : VirtualDataBase, IEnumerable<IVirtualDataObject>
    {
        public VirtualDataList(DataSource ds, string name)
            : base(ds, name)
        {
        }

        public int Count
        {
            get { return table_.Rows.Count; }
        }

        public IVirtualDataObject this[int index]
        {
            get { return new VirtualDataListRow(this, index); }
        }

        public void Init()
        {
            //table_.Rows.Clear();
            //ds_.ExecuteReader<VirtualDataList>(this, string.Format("SELECT * FROM [{0}]", table_.TableName));
        }
        public void Init(string whereAndOrder, params DataParameter[] ps)
        {
            //if (string.IsNullOrEmpty(whereAndOrder))
            //    throw new ArgumentNullException("whereAndOrder");
            //table_.Rows.Clear();
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("SELECT * FROM [{0}] WHERE ", table_.TableName);
            //sb.Append(whereAndOrder);
            //ds_.ExecuteReader<VirtualDataList>(this, sb.ToString(), ps);
        }
        public void Init(string whereAndOrder, int index, int count, ref int total, params DataParameter[] ps)
        {
            //if (string.IsNullOrEmpty(whereAndOrder))
            //    throw new ArgumentNullException("whereAndOrder");
            //table_.Rows.Clear();
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("SELECT COUNT(*) FROM [{0}] WHERE ", table_.TableName);
            //sb.Append(whereAndOrder);
            //total = Convert.ToInt32(ds_.ExecuteScalar(sb.ToString(), ps));
            //sb = new StringBuilder();
            //sb.AppendFormat("SELECT * FROM [{0}] WHERE ", table_.TableName);
            //sb.Append(whereAndOrder);
            //sb.Append(" LIMIT @index,@count;");
            //List<DataParameter> args = new List<DataParameter>();
            //args.Add(P("index", index * count));
            //args.Add(P("count", count));
            //args.AddRange(ps);
            //ds_.ExecuteReader<VirtualDataList>(this, sb.ToString(), args.ToArray());
        }

        public IEnumerator<IVirtualDataObject> GetEnumerator()
        {
            return new VirtualDataListEnumerator(this);
        }
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class VirtualDataListRow : IVirtualDataObject
        {
            private readonly VirtualDataList list_;
            private readonly int index_;

            public VirtualDataListRow(VirtualDataList list, int index)
            {
                list_ = list;
                index_ = index;
            }

            public object this[int index]
            {
                get { return list_.table_.Rows[index_][index]; }
            }
            public object this[string name]
            {
                get { return list_.table_.Rows[index_][name]; }
            }
        }
        private sealed class VirtualDataListEnumerator : IEnumerator<IVirtualDataObject>
        {
            private int index_;
            private readonly VirtualDataList list_;

            public VirtualDataListEnumerator(VirtualDataList list)
            {
                index_ = -1;
                list_ = list;
            }

            public IVirtualDataObject Current
            {
                get { return new VirtualDataListRow(list_, index_); }
            }
            public void Dispose()
            {
            }
            object IEnumerator.Current
            {
                get { return Current; }
            }
            public bool MoveNext()
            {
                return (++index_ < list_.table_.Rows.Count);
            }
            public void Reset()
            {
                index_ = -1;
            }
        }
    }
}
