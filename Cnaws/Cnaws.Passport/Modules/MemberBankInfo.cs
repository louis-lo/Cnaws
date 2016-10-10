using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Passport.Modules
{
    /// <summary>
    /// 银行信息表
    /// </summary>
    [Serializable]
    public sealed class MemberBankInfo : IdentityModule
    {
        /// <summary>
        /// 银行名称
        /// </summary>
        [DataColumn(64)]
        public string BankName = null;
        /// <summary>
        /// 到账天数
        /// </summary>
        public int Days = 0;
        /// <summary>
        /// 银行图标
        /// </summary>
        [DataColumn(128)]
        public string Icon = null;
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort = 0;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable = false;

        private static string[] GetCacheName()
        {
            return new string[] { "MemberBankInfo", "Module" };
        }
        private static void RemoveCache()
        {
            CacheProvider.Current.Set(GetCacheName(), null);
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(BankName))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(BankName))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }

        public static IList<MemberBankInfo> GetAll(DataSource ds)
        {
            string[] key = GetCacheName();
            IList<MemberBankInfo> result = CacheProvider.Current.Get<IList<MemberBankInfo>>(key);
            if (result == null)
            {
                result = Db<MemberBankInfo>.Query(ds)
                    .Select()
                    .Where(W("IsEnable",true))
                    .OrderBy(D("Sort"))
                    .ToList<MemberBankInfo>();
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
        public static MemberBankInfo GetById(DataSource ds, int id)
        {
            return Db<MemberBankInfo>.Query(ds)
                .Select().Where(W("Id", id))
                .First<MemberBankInfo>();
        }

    }
}
