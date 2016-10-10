using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.Data.Query;

namespace Cnaws.Verification.Modules
{
    [Serializable]
    public sealed class StringHash : IdentityModule
    {
        public const int SmsHash = 0;

        [DataColumn(true)]
        public int Type = 0;
        [DataColumn(true, 36)]
        public string Hash = null;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static StringHash Create(DataSource ds, string s, int type, int timespan)
        {
            StringHash hash = Db<StringHash>.Query(ds)
                .Select()
                .Where(W("Type", type) & W("Hash", s))
                .First<StringHash>();
            if (hash != null)
            {
                if (hash.CreationDate.AddSeconds(timespan) > DateTime.Now)
                    return null;
                hash.CreationDate = DateTime.Now;
                if (hash.Update(ds) == DataStatus.Success)
                    return hash;
            }
            else
            {
                hash = new StringHash() { Type = type, Hash = s, CreationDate = DateTime.Now };
                if (hash.Insert(ds) == DataStatus.Success)
                    return hash;
            }
            return null;
        }
    }
}
