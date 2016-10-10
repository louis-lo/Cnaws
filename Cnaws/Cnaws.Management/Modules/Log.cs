using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;

namespace Cnaws.Management.Modules
{
    [Serializable]
    public sealed class LogOperation : IdentityModule
    {
        [DataColumn(32)]
        public string Author = null;
        [DataColumn(32)]
        public string Action = null;
        public string Content = null;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        public static DataStatus Insert(DataSource ds, string author, string action, string content)
        {
            return (new LogOperation() { Author = author, Action = action, Content = content, CreationDate = DateTime.Now }).Insert(ds);
        }
        public static SplitPageData<LogOperation> GetPage(DataSource ds, int index, int size, int show = 8)
        {
            int count;
            IList<LogOperation> list = ExecuteReader<LogOperation>(ds, Os(Od("Id")), index, size, out count);
            return new SplitPageData<LogOperation>(index, size, list, count, show);
        }
        public static void Clear(DataSource ds)
        {
            TruncateTable<LogOperation>(ds);
        }
    }
}
