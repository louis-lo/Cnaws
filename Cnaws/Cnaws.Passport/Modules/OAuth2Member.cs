using System;
using System.Web;
using System.Text;
using System.Globalization;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Passport.OAuth2;
using System.Collections.Generic;
using Cnaws.ExtensionMethods;
using Cnaws.Data.Query;

namespace Cnaws.Passport.Modules
{
    [Serializable]
    public class OAuth2Member : NoIdentityModule
    {
        [DataColumn(true, 32)]
        public string Type = null;
        [DataColumn(true, 64)]
        public string UserId = null;
        public long MemberId = 0L;
        [DataColumn(32)]
        public string ScreenName = null;
        [DataColumn(32)]
        public string UserName = null;
        [DataColumn(128)]
        public string Location = null;
        [DataColumn(2000)]
        public string Description = null;
        [DataColumn(256)]
        public string Image = null;
        [DataColumn(64)]
        public string AccessToken = null;
        public int ExpireAt = 0;
        [DataColumn(64)]
        public string RefreshToken = null;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "UserId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "UserId", "UserId");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(UserId))
                return DataStatus.Failed;
            Type = Type.ToLower();
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "Type", "UserId");
            if (string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(UserId))
                return DataStatus.Failed;
            Type = Type.ToLower();
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        //public Member GetMember(DataSource ds)
        //{
        //    MemberId = ExecuteScalar<OAuth2Member, int>(ds, "MemberId", P("Type", Type) & P("UserId", UserId));
        //    if (MemberId > 0)
        //        return Member.GetById(ds, MemberId);
        //    return null;
        //}
        //public DataStatus BindMember(DataSource ds, long memberId)
        //{
        //    MemberId = memberId;
        //    return Update(ds, ColumnMode.Include, "MemberId");
        //}
        //public Member BindMember(DataSource ds, string memberName)
        //{
        //    Member member = Member.GetByName(ds, memberName);
        //    if (member != null)
        //    {
        //        if (BindMember(ds, member.Id) == DataStatus.Success)
        //            return member;
        //    }
        //    return null;
        //}
        public static DataStatus Register(DataSource ds, string type, string userId, Member member, RegisterType rt)
        {
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(userId))
            {
                ds.Begin();
                try
                {
                    Member temp = Member.GetByType(ds, member, rt);
                    if (temp == null)
                    {
                        DataStatus status = member.Insert(ds);
                        if (status != DataStatus.Success)
                        {
                            ds.Commit();
                            return status;
                        }
                        else
                        {
                            temp = member;
                        }
                    }
                    else
                    {
                        if (rt != RegisterType.Mobile && !string.Equals(temp.Password, member.Password.MD5()))
                        {
                            ds.Commit();
                            return DataStatus.Exist;
                        }
                    }
                    OAuth2Member user = new OAuth2Member()
                    {
                        Type = type,
                        UserId = userId,
                        MemberId = temp.Id
                    };
                    if (user.Update(ds, ColumnMode.Include, "MemberId") != DataStatus.Success)
                    {
                        if (user.Insert(ds) != DataStatus.Success)
                            throw new Exception();
                    }
                    ds.Commit();
                    return DataStatus.Success;
                }
                catch (Exception)
                {
                    ds.Rollback();
                    return DataStatus.Failed;
                }
            }
            else
            {
                return DataStatus.ExistOther;
            }
        }
        public static LoginStatus Login(DataSource ds, string type, string userId, string ip, out Member member)
        {
            Guid token;
            return Login(ds, type, userId, ip, null, out member, out token);
        }
        public static LoginStatus Login(DataSource ds, string type, string userId, string ip, string mark, out Guid token)
        {
            Member member;
            return Login(ds, type, userId, ip, mark, out member, out token);
        }
        private static LoginStatus Login(DataSource ds, string type, string userId, string ip, string mark, out Member member, out Guid token)
        {
            member = null;
            token = Guid.Empty;
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(userId))
            {
                ds.Begin();
                try
                {
                    LoginStatus result;
                    long memberId = ExecuteScalar<OAuth2Member, long>(ds, "MemberId", P("Type", type) & P("UserId", userId));
                    if (memberId > 0)
                    {
                        member = Member.GetById(ds, memberId);
                        if (member != null)
                        {
                            if (member.Approved)
                            {
                                if (member.Locked)
                                {
                                    DateTime now = DateTime.Now;
                                    if (member.LockTime.AddMinutes(PassportAuthentication.PasswordAnswerAttemptLockoutDuration) < now)
                                    {
                                        bool hasMark = !string.IsNullOrEmpty(mark);
                                        List<DataColumn> columns = new List<DataColumn>(6);
                                        columns.AddRange(new DataColumn[] { "Locked", "LockNum", "LastIp", "LastTime" });
                                        if (hasMark)
                                        {
                                            member.Mark = mark;
                                            member.Token = Guid.NewGuid();
                                            columns.AddRange(new DataColumn[] { "Mark", "Token" });
                                        }
                                        member.Locked = false;
                                        member.LockNum = 0;
                                        member.LastIp = ip;
                                        member.LastTime = DateTime.Now;
                                        if (member.Update(ds, ColumnMode.Include, columns.ToArray()) == DataStatus.Success)
                                        {
                                            if (hasMark)
                                                token = member.Token;
                                            result = LoginStatus.Success;
                                        }
                                        else
                                        {
                                            result = LoginStatus.DataError;
                                        }
                                    }
                                    else
                                    {
                                        result = LoginStatus.Locked;
                                    }
                                }
                                else
                                {
                                    bool hasMark = !string.IsNullOrEmpty(mark);
                                    List<DataColumn> columns = new List<DataColumn>(6);
                                    columns.AddRange(new DataColumn[] { "LockNum", "LastIp", "LastTime" });
                                    if (hasMark)
                                    {
                                        member.Mark = mark;
                                        member.Token = Guid.NewGuid();
                                        columns.AddRange(new DataColumn[] { "Mark", "Token" });
                                    }
                                    member.LockNum = 0;
                                    member.LastIp = ip;
                                    member.LastTime = DateTime.Now;
                                    if (member.Update(ds, ColumnMode.Include, columns.ToArray()) == DataStatus.Success)
                                    {
                                        if (hasMark)
                                            token = member.Token;
                                        result = LoginStatus.Success;
                                    }
                                    else
                                    {
                                        result = LoginStatus.DataError;
                                    }
                                }
                            }
                            else
                            {
                                result = LoginStatus.NotApproved;
                            }
                        }
                        else
                        {
                            result = LoginStatus.NotFound;
                        }
                    }
                    else
                    {
                        result = LoginStatus.NeedBind;
                    }
                    ds.Commit();
                    return result;
                }
                catch (Exception)
                {
                    ds.Rollback();
                    return LoginStatus.DataError;
                }
            }
            else
            {
                return LoginStatus.NotFound;
            }
        }

        public static OAuth2Member GetByUser(DataSource ds, string type, long userId)
        {
            return Db<OAuth2Member>.Query(ds)
                .Select()
                .Where(W("Type", type) & W("MemberId", userId))
                .First<OAuth2Member>();
        }
        public static OAuth2Member GetByUserPay(DataSource ds, string pay, long userId)
        {
            if ("wxpay".Equals(pay, StringComparison.OrdinalIgnoreCase))
                return GetByUser(ds, "weixin", userId);
            return null;
        }

        public static SplitPageData<OAuth2Member> GetPage(DataSource ds, int index,int size,int show = 8)
        {
            long count;
            IList<OAuth2Member> list = Db<OAuth2Member>.Query(ds)
                .Select(S<OAuth2Member>("*"))
                .ToList<OAuth2Member>(size, index, out count);
            return new SplitPageData<OAuth2Member>(index, size, list, count, show);
        }
    }
}
