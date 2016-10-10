using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Passport.Modules
{
    /// <summary>
    /// 会员银行卡信息表
    /// </summary>
    [Serializable]
    public sealed class MemberBank : LongIdentityModule
    {
        /// <summary>
        ///用户id
        /// </summary>
        public long UserId = 0L;
        /// <summary>
        /// 银行Id
        /// </summary>
        public int BankId = 0;
        /// <summary>
        /// 开户名
        /// </summary>
        [DataColumn(64)]
        public string AccountName = null;
        /// <summary>
        /// 银行卡号
        /// </summary>
        [DataColumn(64)]
        public string BankCard = null;
        public int Province = 0;
        public int City = 0;
        public int Region = 0;
        /// <summary>
        /// 开户网点
        /// </summary>
        [DataColumn(64)]
        public string BankZone = null;
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault = false;
        public bool IsVerify = true;

        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "UserId", "UserId");
            CreateIndex(ds, "BankId", "BankId");
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "UserId");
            DropIndex(ds, "BankId");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(AccountName) || string.IsNullOrEmpty(BankCard))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(AccountName) || string.IsNullOrEmpty(BankCard))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            return DataStatus.Success;
        }

        public static IList<DataJoin<MemberBank, MemberBankInfo>> GetBanksByUserId(DataSource ds, long userid)
        {
            return ExecuteReader<MemberBank, MemberBankInfo>(ds, 
                Cs(C<MemberBank>("*"), 
                C<MemberBankInfo>("Icon"), 
                C<MemberBankInfo>("BankName")), 
                Os(Od<MemberBank>("IsDefault")), "BankId", "Id", DataJoinType.Inner, P<MemberBank>("UserId", userid));
        }

        public static IList<MemberBank> GetAllBanksByUserId(DataSource ds, long userid)
        {
            return Db<MemberBank>.Query(ds)
                 .Select().Where(W("UserId", userid)).OrderBy(D("IsDefault")).ToList<MemberBank>();

        }

        public static MemberBank GetById(DataSource ds, int id)
        {
            return Db<MemberBank>.Query(ds)
                .Select().Where(W("Id", id))
                .First<MemberBank>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="bankId">银行卡Id，不是主键id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public static MemberBank GetById(DataSource ds, int bankId, long userId)
        {
            return Db<MemberBank>
                   .Query(ds)
                   .Select()
                   .Where(new DbWhereQueue("UserId", userId) & new DbWhereQueue("Id", bankId))
                   .First<MemberBank>();
        }

        public static MemberBank GetUserDefaultBank(DataSource ds, long userId)
        {
            return Db<MemberBank>
                    .Query(ds)
                    .Select()
                    .Where(new DbWhereQueue("IsDefault", true) & new DbWhereQueue("UserId", userId))
                    .First<MemberBank>();
        }
    }
}
