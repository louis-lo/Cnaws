using Cnaws.Data;
using System;

namespace Cnaws.Web.Modules
{
    [Serializable]
    public sealed class SqlCache : NoIdentityModule
    {
        [DataColumn(true, 128)]
        public string Key = null;
        public byte[] Value = null;

        public static SqlCache Get(DataSource ds, string key)
        {
            return ExecuteSingleRow<SqlCache>(ds, P("Key", key));
        }
        public static void Set(DataSource ds, string key, byte[] value)
        {
            try
            {
                if ((new SqlCache() { Key = key, Value = value }).Insert(ds) != DataStatus.Success)
                    throw new Exception();
            }
            catch (Exception)
            {
                (new SqlCache() { Key = key, Value = value }).Update(ds);
            }
        }
        public static void Delete(DataSource ds, string key)
        {
            (new SqlCache() { Key = key }).Delete(ds);
        }
    }
}
