using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Reflection;
using Cnaws.Templates;
using System.Collections.Generic;

namespace Cnaws.Sms.Modules
{
    [Serializable]
    public sealed class Sms : NoIdentityModule
    {
        [DataColumn(true, 32)]
        public string Id = null;
        [DataColumn(32)]
        public string Name = null;
        [DataColumn(32)]
        public string Version = null;
        [DataColumn(64)]
        public string Account = null;
        [DataColumn(64)]
        public string Token = null;
        [DataColumn(64)]
        public string AppId = null;
        public bool Enabled = false;

        private static string[] GetCacheName(string id = null)
        {
            if (string.IsNullOrEmpty(id))
                return new string[] { "Sms", "Module" };
            return new string[] { "Sms", "Module", id.ToLower() };
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Enabled");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Enabled", "Enabled");

            SmsProvider provider;
            string ns = string.Concat("Cnaws.Sms.Providers");
            Assembly asm = Assembly.GetAssembly(TType<Sms>.Type);
            foreach (TypeInfo type in asm.DefinedTypes)
            {
                if (type.IsClass &&
                    !type.IsAbstract &&
                    string.Equals(type.Namespace, ns, StringComparison.OrdinalIgnoreCase) &&
                    TType<SmsProvider>.Type.IsAssignableFrom(type.UnderlyingSystemType))
                {
                    provider = (SmsProvider)Activator.CreateInstance(type.UnderlyingSystemType);
                    (new Sms() { Id = provider.Key, Name = provider.Name, Version = provider.Version.ToString() }).Insert(ds);
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

        public DataStatus UpdateEnable(DataSource ds)
        {
            if (!Enabled)
                return Update(ds);
            ds.Begin();
            try
            {
                if ((new Sms() { Id = "All", Enabled = false }).Update(ds, ColumnMode.Include, Cs("Enabled"), WD) != DataStatus.Success)
                    throw new Exception();
                if (Update(ds) != DataStatus.Success)
                    throw new Exception();
                ds.Commit();
                return DataStatus.Success;
            }
            catch (Exception)
            {
                ds.Rollback();
                return DataStatus.Rollback;
            }
        }

        public static Sms GetById(DataSource ds, string id)
        {
            if (id == null)
                return null;
            id = id.ToLower();
            string[] key = GetCacheName(id);
            Sms result = CacheProvider.Current.Get<Sms>(key);
            if (result == null)
            {
                result = ExecuteSingleRow<Sms>(ds, P("Id", id));
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
        public static IList<Sms> GetAll(DataSource ds)
        {
            string[] key = GetCacheName();
            IList<Sms> result = CacheProvider.Current.Get<IList<Sms>>(key);
            if (result == null)
            {
                result = ExecuteReader<Sms>(ds);
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
        public static SplitPageData<Sms> GetPage(DataSource ds, int index, int size, int show = 8)
        {
            int count;
            IList<Sms> list = ExecuteReader<Sms>(ds, Os(Oa("Id")), index, size, out count);
            return new SplitPageData<Sms>(index, size, list, count, show);
        }
    }
}
