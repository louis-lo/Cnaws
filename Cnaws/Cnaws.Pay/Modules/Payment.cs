using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Reflection;
using Cnaws.Templates;
using System.Collections.Generic;

namespace Cnaws.Pay.Modules
{
    [Serializable]
    public sealed class Payment : NoIdentityModule
    {
        [DataColumn(true, 32)]
        public string Id = null;
        [DataColumn(32)]
        public string Name = null;
        [DataColumn(32)]
        public string Version = null;
        [DataColumn(64)]
        public string Partner = null;
        [DataColumn(64)]
        public string PartnerId = null;
        [DataColumn(1024)]
        public string PartnerKey = null;
        [DataColumn(1024)]
        public string PartnerSecret = null;
        public bool Enabled = false;

        private static string[] GetCacheName(string id = null)
        {
            if (string.IsNullOrEmpty(id))
                return new string[] { "Payment", "Module" };
            return new string[] { "Payment", "Module", id.ToLower() };
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Enabled");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Enabled", "Enabled");

            PayProvider provider;
            string ns = string.Concat("Cnaws.Pay.Providers");
            Assembly asm = Assembly.GetAssembly(TType<Payment>.Type);
            foreach (TypeInfo type in asm.DefinedTypes)
            {
                if (type.IsClass && 
                    !type.IsAbstract && 
                    string.Equals(type.Namespace, ns, StringComparison.OrdinalIgnoreCase) &&
                    TType<PayProvider>.Type.IsAssignableFrom(type.UnderlyingSystemType))
                {
                    provider = (PayProvider)Activator.CreateInstance(type.UnderlyingSystemType);
                    (new Payment() { Id = provider.Key, Name = provider.Name, Version = provider.Version.ToString() }).Insert(ds);
                }
            }
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Version))
                return DataStatus.Failed;
            Id = Id.ToLower();
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            CacheProvider.Current.Set(GetCacheName(Id), null);
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "Id", "Name", "Version");
            if (string.IsNullOrEmpty(Id))
                return DataStatus.Failed;
            Id = Id.ToLower();
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            CacheProvider.Current.Set(GetCacheName(Id), null);
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Id))
                return DataStatus.Failed;
            Id = Id.ToLower();
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            CacheProvider.Current.Set(GetCacheName(Id), null);
            return DataStatus.Success;
        }

        public static Payment GetById(DataSource ds, string id)
        {
            if (id == null)
                return null;
            id = id.ToLower();
            string[] key = GetCacheName(id);
            Payment result = CacheProvider.Current.Get<Payment>(key);
            if (result == null)
            {
                result = ExecuteSingleRow<Payment>(ds, P("Id", id));
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }

        public static IList<Payment> GetAll(DataSource ds)
        {
            string[] key = GetCacheName();
            IList<Payment> result = CacheProvider.Current.Get<IList<Payment>>(key);
            if (result == null)
            {
                result = ExecuteReader<Payment>(ds);
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
        public static SplitPageData<Payment> GetPage(DataSource ds, int index, int size, int show = 8)
        {
            int count;
            IList<Payment> list = ExecuteReader<Payment>(ds, Os(Oa("Id")), index, size, out count);
            return new SplitPageData<Payment>(index, size, list, count, show);
        }
    }
}
