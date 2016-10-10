using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Passport.Modules
{
    public enum MemberSex
    {
        Unkown = 0,
        Boy = 1,
        Girl = 2
    }

    public enum RegisterType
    {
        Name = 0,
        Email = 1,
        Mobile = 2
    }

    public enum LoginStatus
    {
        NotFound = -1000,
        NotApproved = -1001,
        PasswordError = -1002,
        Locked = -1003,
        CaptchaError = -1004,
        SmsCaptchaError = -1005,
        NeedBind = -1006,
        DataError = -500,
        Success = -200
    }

    [Serializable, DataTable("Member")]
    public class Member : LongIdentityModule, IPassportUserInfo
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [DataColumn(null, false, false, false, 32, true)]
        public string Name = null;
        /// <summary>
        /// 电子邮件
        /// </summary>
        [DataColumn(256)]
        public string Email = null;
        /// <summary>
        /// 是否已验证电子邮件
        /// </summary>
        public bool VerMail = false;
        /// <summary>
        /// 手机号码
        /// </summary>
        public long Mobile = 0L;
        /// <summary>
        /// 是否已验证手机号码
        /// </summary>
        public bool VerMob = false;
        /// <summary>
        /// 推荐人
        /// </summary>
        public long ParentId = 0L;
        /// <summary>
        /// 密码
        /// </summary>
        [DataColumn(36)]
        public string Password = null;
        /// <summary>
        /// 角色
        /// </summary>
        public int RoleId = 0;
        /// <summary>
        /// 注册日期
        /// </summary>
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 移动端标识
        /// </summary>
        [DataColumn(36)]
        public string Mark = null;
        /// <summary>
        /// 移动端登录凭据
        /// </summary>
        public Guid Token = Guid.Empty;
        /// <summary>
        /// 登录次数
        /// </summary>
        public int LoginNum = 0;
        /// <summary>
        /// 最后登录IP
        /// </summary>
        [DataColumn(64)]
        public string LastIp = null;
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool Approved = false;
        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool Locked = false;
        /// <summary>
        /// 登录次数
        /// </summary>
        public int LockNum = 0;
        /// <summary>
        /// 锁定时间
        /// </summary>
        public DateTime LockTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        public virtual string GetName()
        {
            Guid guid;
            if (Guid.TryParse(Name, out guid))
            {
                if (Mobile > 0)
                    return Mobile.ToString();
                if (!string.IsNullOrEmpty(Email))
                    return Email;
            }
            return Name;
        }
        public virtual int GetSecurityValue()
        {
            int value = 10;
            Guid guid;
            if (!Guid.TryParse(Name, out guid))
                value += 30;
            if (VerMob)
                value += 30;
            if (VerMail)
                value += 30;
            return value;
        }

        #region IPassportUserInfo
        long IPassportUserInfo.Id
        {
            get { return Id; }
        }
        long IPassportUserInfo.AdminId
        {
            get { return 0L; }
        }
        string IPassportUserInfo.Name
        {
            get { return GetName(); }
        }
        long IPassportUserInfo.RoleId
        {
            get { return RoleId; }
        }
        long IPassportUserInfo.AdminRoleId
        {
            get { return 0L; }
        }
        DateTime IPassportUserInfo.CreationDate
        {
            get { return CreationDate; }
        }
        string IPassportUserInfo.LastIp
        {
            get { return LastIp; }
        }
        DateTime IPassportUserInfo.LastTime
        {
            get { return LastTime; }
        }
        long IPassportUserInfo.LoginCount
        {
            get { return LoginNum; }
        }
        string IPassportUserInfo.UserData
        {
            get { return string.Empty; }
        }
        #endregion

        public override bool CanInstall
        {
            get { return false; }
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Name");
            DropIndex(ds, "Email");
            DropIndex(ds, "Mobile");
            DropIndex(ds, "Token");
            DropIndex(ds, "Approved");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Name", "Name");
            CreateIndex(ds, "Email", "Email");
            CreateIndex(ds, "Mobile", "Mobile");
            CreateIndex(ds, "Token", "Token");
            CreateIndex(ds, "Approved", "Approved");
        }

        public static bool HasId(DataSource ds, long id)
        {
            return ExecuteCount<Member>(ds, P("Id", id)) > 0;
        }
        public static bool CheckName(DataSource ds, string name)
        {
            return ExecuteCount<Member>(ds, P("Name", name)) == 0;
        }
        public static bool CheckMobile(DataSource ds, long moblie)
        {
            return ExecuteCount<Member>(ds, P("Mobile", moblie) & P("VerMob", true)) == 0;
        }
        public static bool CheckEmail(DataSource ds, string email)
        {
            return ExecuteCount<Member>(ds, P("Email", email) & P("VerMail", true)) == 0;
        }
        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            bool emptyName = string.IsNullOrEmpty(Name);
            bool emptyEmail = string.IsNullOrEmpty(Email);
            bool emptyMobile = !Mobile.IsMobile();
            if (emptyName && emptyEmail && emptyMobile)
                return DataStatus.Failed;
            if (emptyMobile)
                Mobile = 0L;
            if (!emptyName)
            {
                if (!Web.Utility.PassportNameRegularExpression.IsMatch(ref Name))
                    return DataStatus.Failed;
                if (!CheckName(ds, Name))
                    return DataStatus.Exist;
            }
            if (!emptyEmail)
            {
                if (!Web.Utility.EmailRegularExpression.IsMatch(ref Email))
                    return DataStatus.Failed;
                if (!CheckEmail(ds, Email))
                    return DataStatus.Exist;
            }
            if (!emptyMobile)
            {
                if (!CheckMobile(ds, Mobile))
                    return DataStatus.Exist;
            }
            if (!emptyEmail || !emptyMobile)
            {
                if (emptyName)
                    Name = Guid.NewGuid().ToString("N");
            }
            if (!Web.Utility.PassportPasswordRegularExpression.IsMatch(Password))
                return DataStatus.ExistOther;
            Password = Password.MD5();
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "CreationDate");
            if (Include(columns, mode, "Password"))
            {
                if (!Web.Utility.PassportPasswordRegularExpression.IsMatch(Password))
                    return DataStatus.ExistOther;
                Password = Password.MD5();
            }
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static LoginStatus Login(DataSource ds, string name, string password, string ip, out int errCount, out Member member)
        {
            Guid token;
            return Login(ds, name, password, ip, null, out errCount, out member, out token);
        }
        public static LoginStatus Login(DataSource ds, string name, string password, string ip, string mark, out int errCount, out Guid token)
        {
            Member member;
            return Login(ds, name, password, ip, mark, out errCount, out member, out token);
        }
        public static LoginStatus ApiLogin(DataSource ds, string name, string password, string ip, string mark, out int errCount, out Guid token)
        {
            Member member;
            return ApiLogin(ds, name, password, ip, mark, out errCount, out member, out token);
        }
        private static LoginStatus ApiLogin(DataSource ds, string name, string password, string ip, string mark, out int errCount, out Member member, out Guid token)
        {
            errCount = PassportAuthentication.MaxInvalidPasswordAttempts;
            member = null;
            token = Guid.Empty;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password))
            {
                ds.Begin();
                try
                {
                    LoginStatus result;
                    member = Get(ds, name);
                    if (member != null)
                    {
                        if (member.Approved)
                        {
                            if (password != null)
                            {
                                if (string.Equals(password, member.Password))
                                {
                                    if (member.Locked)
                                    {
                                        DateTime now = DateTime.Now;
                                        if (member.LockTime.AddMinutes(PassportAuthentication.PasswordAnswerAttemptLockoutDuration) < now)
                                        {
                                            bool hasMark = !string.IsNullOrEmpty(mark);
                                            List<DataColumn> columns = new List<DataColumn>(6);
                                            columns.AddRange(new DataColumn[] { "LoginNum", "Locked", "LockNum", "LastIp", "LastTime" });
                                            if (hasMark)
                                            {
                                                member.Mark = mark;
                                                member.Token = Guid.NewGuid();
                                                columns.AddRange(new DataColumn[] { "Mark", "Token" });
                                            }
                                            member.Locked = false;
                                            member.LockNum = 0;
                                            member.LastIp = ip;
                                            member.LoginNum = member.LoginNum + 1;
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
                                        columns.AddRange(new DataColumn[] { "LoginNum", "LockNum", "LastIp", "LastTime" });
                                        if (hasMark)
                                        {
                                            member.Mark = mark;
                                            member.Token = Guid.NewGuid();
                                            columns.AddRange(new DataColumn[] { "Mark", "Token" });
                                        }
                                        member.LockNum = 0;
                                        member.LastIp = ip;
                                        member.LoginNum = member.LoginNum + 1;
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
                                    if (member.Locked)
                                    {
                                        result = LoginStatus.Locked;
                                    }
                                    else
                                    {
                                        member.LockNum = member.LockNum + 1;
                                        member.Locked = member.LockNum >= PassportAuthentication.MaxInvalidPasswordAttempts;
                                        member.LockTime = DateTime.Now;
                                        member.Update(ds, ColumnMode.Include, "Locked", "LockNum", "LockTime");
                                        errCount = PassportAuthentication.MaxInvalidPasswordAttempts - member.LockNum;
                                        if (errCount < 0)
                                            errCount = 0;
                                        result = LoginStatus.PasswordError;
                                    }
                                }
                            }
                            else
                            {
                                result = LoginStatus.PasswordError;
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
                return LoginStatus.PasswordError;
            }
        }

        public static long GetNumByParentId(DataSource ds, long parentId)
        {
            return Db<Member>.Query(ds).Select().Where(W("ParentId", parentId) & W("Approved", true)).Count();
        }

        private static LoginStatus Login(DataSource ds, string name, string password, string ip, string mark, out int errCount, out Member member, out Guid token)
        {
            errCount = PassportAuthentication.MaxInvalidPasswordAttempts;
            member = null;
            token = Guid.Empty;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password))
            {
                ds.Begin();
                try
                {
                    LoginStatus result;
                    member = Get(ds, name);
                    if (member != null)
                    {
                        if (member.Approved)
                        {
                            if (password != null)
                            {
                                if (string.Equals(password.MD5(), member.Password))
                                {
                                    if (member.Locked)
                                    {
                                        DateTime now = DateTime.Now;
                                        if (member.LockTime.AddMinutes(PassportAuthentication.PasswordAnswerAttemptLockoutDuration) < now)
                                        {
                                            bool hasMark = !string.IsNullOrEmpty(mark);
                                            List<DataColumn> columns = new List<DataColumn>(6);
                                            columns.AddRange(new DataColumn[] { "LoginNum", "Locked", "LockNum", "LastIp", "LastTime" });
                                            if (hasMark)
                                            {
                                                member.Mark = mark;
                                                member.Token = Guid.NewGuid();
                                                columns.AddRange(new DataColumn[] { "Mark", "Token" });
                                            }
                                            member.Locked = false;
                                            member.LockNum = 0;
                                            member.LoginNum = member.LoginNum + 1;
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
                                        columns.AddRange(new DataColumn[] { "LoginNum", "LockNum", "LastIp", "LastTime" });
                                        if (hasMark)
                                        {
                                            member.Mark = mark;
                                            member.Token = Guid.NewGuid();
                                            columns.AddRange(new DataColumn[] { "Mark", "Token" });
                                        }
                                        member.LockNum = 0;
                                        member.LastIp = ip;
                                        member.LoginNum = member.LoginNum + 1;
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
                                    if (member.Locked)
                                    {
                                        result = LoginStatus.Locked;
                                    }
                                    else
                                    {
                                        member.LockNum = member.LockNum + 1;
                                        member.Locked = member.LockNum >= PassportAuthentication.MaxInvalidPasswordAttempts;
                                        member.LockTime = DateTime.Now;
                                        member.Update(ds, ColumnMode.Include, "Locked", "LockNum", "LockTime");
                                        errCount = PassportAuthentication.MaxInvalidPasswordAttempts - member.LockNum;
                                        if (errCount < 0)
                                            errCount = 0;
                                        result = LoginStatus.PasswordError;
                                    }
                                }
                            }
                            else
                            {
                                result = LoginStatus.PasswordError;
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
                return LoginStatus.PasswordError;
            }
        }

        public static string GetPasswordById(DataSource ds, long id)
        {
            return ExecuteScalar<Member, string>(ds, "Password", P("Id", id));
        }
        public static Member GetById(DataSource ds, long id)
        {
            return ExecuteSingleRow<Member>(ds, Cs("Id", "Name", "Email", "VerMail", "Mobile", "VerMob", "RoleId", "ParentId", "CreationDate", "LoginNum", "LastIp", "LastTime", "Approved", "Locked", "LockNum", "LockTime"), P("Id", id));
        }

        public static IList<Member> GetByIds(DataSource ds, long[] ids)
        {
            return Db<Member>.Query(ds).Select(S("Name"), S("Id")).Where(W("Id", ids, DbWhereType.In)).ToList<Member>();
            //return ExecuteSingleRow<Member>(ds, Cs("Id", "Name", "Email", "VerMail", "Mobile", "VerMob", "RoleId", "ParentId", "CreationDate", "LoginNum", "LastIp", "LastTime", "Approved", "Locked", "LockNum", "LockTime"), P("Id", id));
        }
        public static Member Get(DataSource ds, string name)
        {
            long mobile;
            if (long.TryParse(name, out mobile))
            {
                return GetByMobile(ds, mobile);
            }
            else
            {
                if (Web.Utility.EmailRegularExpression.IsMatch(name))
                {
                    return GetByEmail(ds, name);
                }
                else
                {
                    return GetByName(ds, name);
                }
            }
        }
        internal static Member GetByType(DataSource ds, Member member, RegisterType type)
        {
            switch (type)
            {
                case RegisterType.Email: return GetByEmail(ds, member.Email);
                case RegisterType.Mobile: return GetByMobile(ds, member.Mobile);
                default: return GetByName(ds, member.Name);
            }
        }
        private static Member GetByName(DataSource ds, string name)
        {
            return ExecuteSingleRow<Member>(ds, Cs("Id", "Name", "Email", "VerMail", "Mobile", "VerMob", "Password", "RoleId", "CreationDate", "LoginNum", "LastIp", "LastTime", "Approved", "Locked", "LockNum", "LockTime"), P("Name", name));
        }
        private static Member GetByMobile(DataSource ds, long mobile)
        {
            return ExecuteSingleRow<Member>(ds, Cs("Id", "Name", "Email", "VerMail", "Mobile", "VerMob", "Password", "RoleId", "CreationDate", "LoginNum", "LastIp", "LastTime", "Approved", "Locked", "LockNum", "LockTime"), P("VerMob", true) & P("Mobile", mobile));
        }
        private static Member GetByEmail(DataSource ds, string email)
        {
            return ExecuteSingleRow<Member>(ds, Cs("Id", "Name", "Email", "VerMail", "Mobile", "VerMob", "Password", "RoleId", "CreationDate", "LoginNum", "LastIp", "LastTime", "Approved", "Locked", "LockNum", "LockTime"), P("VerMail", true) & P("Email", email));
        }

        public static Member GetByToken(DataSource ds, Guid token)
        {
            return Db<Member>.Query(ds).Select().Where(W("Token", token)).First<Member>();
        }
        public static Member GetNameById(DataSource ds, long id)
        {
            IList<Member> list = Db<Member>.Query(ds).Select("Name").Where(W("Id", id)).ToList<Member>();
            return new Member();
        }

    }

    [Serializable, DataTable("Member")]
    public sealed class MemberInfo : Member
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [DataColumn(32)]
        public string NickName = null;
        /// <summary>
        /// 头像
        /// </summary>
        [DataColumn(256)]
        public string Image = null;
        /// <summary>
        /// 支付密码
        /// </summary>
        [DataColumn(36)]
        public string PayPassword = null;
        /// <summary>
        /// 真实姓名
        /// </summary>
        [DataColumn(16)]
        public string RealName = null;
        /// <summary>
        /// 性别
        /// </summary>
        public MemberSex Sex = MemberSex.Unkown;
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 省、直辖市
        /// </summary>
        public int Province = 0;
        /// <summary>
        /// 城市
        /// </summary>
        public int City = 0;
        /// <summary>
        /// 县、区
        /// </summary>
        public int County = 0;
        /// <summary>
        /// 县、区
        /// </summary>
        [DataColumn(128)]
        public string Address = null;
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard = null;
        /// <summary>
        /// QQ
        /// </summary>
        [DataColumn(256)]
        public string QQ = null;
        /// <summary>
        /// 阿里旺旺
        /// </summary>
        [DataColumn(256)]
        public string AliWW = null;
        /// <summary>
        /// 简介
        /// </summary>
        [DataColumn(1024)]
        public string Summary = null;
        /// <summary>
        /// 积分
        /// </summary>
        [DataColumn(null, false, false, false, 4, false, 0)]
        public int Integral = 0;
        /// <summary>
        /// 经验
        /// </summary>
        [DataColumn(null, false, false, false, 4, false, 0)]
        public int Experience = 0;
        /// <summary>
        /// 可用预存款
        /// </summary>
        [DataColumn(null, false, false, false, 8, false, 0)]
        public Money Money = 0;
        /// <summary>
        /// 冻结预存款
        /// </summary>
        [DataColumn(null, false, false, false, 8, false, 0)]
        public Money FreezeMoney = 0;

        public override bool CanInstall
        {
            get { return true; }
        }

        public string GetImage(string value)
        {
            if (!string.IsNullOrEmpty(Image))
                return Image;
            return value;
        }

        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (Include(columns, mode, "PayPassword"))
            {
                if (!Web.Utility.PassportPasswordRegularExpression.IsMatch(PayPassword))
                    return DataStatus.ExistOther;
                PayPassword = PayPassword.MD5();
            }
            return base.OnUpdateBefor(ds, mode, ref columns);
        }

        /// <summary>
        /// 修改积分
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataStatus ModifyIntegral(DataSource ds, long id, int value, string title, int type = 0, string targetId = null)
        {
            if (value == 0)
                return DataStatus.Success;
            ds.Begin();
            try
            {
                DataStatus status = (new IntegralRecord()
                {
                    MemberId = id,
                    Title = title,
                    Type = type,
                    TargetId = targetId,
                    Value = value,
                    CreationDate = DateTime.Now
                }).Insert(ds);
                if (status != DataStatus.Success)
                    throw new Exception();
                status = (new MemberInfo() { Id = id }).Update(ds, ColumnMode.Include, MODC("Integral", value));
                if (status != DataStatus.Success)
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
        /// <summary>
        /// 修改经验
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataStatus ModifyExperience(DataSource ds, long id, int value, string title, int type = 0, string targetId = null)
        {
            if (value == 0)
                return DataStatus.Success;
            ds.Begin();
            try
            {
                DataStatus status = (new ExperienceRecord()
                {
                    MemberId = id,
                    Title = title,
                    Type = type,
                    TargetId = targetId,
                    Value = value,
                    CreationDate = DateTime.Now
                }).Insert(ds);
                if (status != DataStatus.Success)
                    throw new Exception();
                status = (new MemberInfo() { Id = id }).Update(ds, ColumnMode.Include, MODC("Experience", value));
                if (status != DataStatus.Success)
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
        /// <summary>
        /// 修改可用预存款
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataStatus ModifyMoney(DataSource ds, long id, Money value, string title, int type = 0, string targetId = null)
        {
            if (value == 0)
                return DataStatus.Success;
            ds.Begin();
            try
            {
                DataStatus status = (new MoneyRecord()
                {
                    MemberId = id,
                    Title = title,
                    Type = type,
                    TargetId = targetId,
                    Value = value,
                    CreationDate = DateTime.Now
                }).Insert(ds);
                if (status != DataStatus.Success)
                    throw new Exception();
                DataWhereQueue where = P("Id", id);
                if (value < 0)
                    where = WN("Money", -value, "OldMoney", ">=") & where;
                status = (new MemberInfo() { Id = id }).Update(ds, ColumnMode.Include, Cs(MODC("Money", value)), where);
                if (status != DataStatus.Success)
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
        /// <summary>
        /// 修改冻结预存款，将自动扣除可用预存款
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataStatus ModifyFreezeMoney(DataSource ds, long id, Money value, string title, int type = 0, string targetId = null)
        {
            if (value == 0)
                return DataStatus.Success;
            ds.Begin();
            try
            {
                DataStatus status = (new MoneyRecord()
                {
                    MemberId = id,
                    Title = string.Concat(title, "[冻结]"),
                    Type = type,
                    TargetId = targetId,
                    Value = -value,
                    CreationDate = DateTime.Now
                }).Insert(ds);
                if (status != DataStatus.Success)
                    throw new Exception();
                status = (new MemberInfo() { Id = id }).Update(ds, ColumnMode.Include, MODC("Money", -value), MODC("FreezeMoney", value));
                if (status != DataStatus.Success)
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

        public DataStatus Modify(DataSource ds)
        {
            return Update(ds, ColumnMode.Include, "Image", "NickName", "RealName", "Sex", "Email", "Birthday", "Province", "City", "County", "Address", "Summary");
        }

        public static string GetPayPasswordById(DataSource ds, long id)
        {
            return ExecuteScalar<MemberInfo, string>(ds, "PayPassword", P("Id", id));
        }
        public static MemberInfo GetByModify(DataSource ds, long id)
        {
            return ExecuteSingleRow<MemberInfo>(ds, Cs("Image", "NickName", "RealName", "Sex", "Mobile", "Email", "Birthday", "Province", "City", "County", "Address", "Summary"), P("Id", id));
        }
        public static MemberInfo GetBySecurity(DataSource ds, long id)
        {
            return ExecuteSingleRow<MemberInfo>(ds, Cs("Email", "VerMail", "Mobile", "VerMob", "Password", "PayPassword"), P("Id", id));
        }
        public static MemberInfo GetByRecharge(DataSource ds, long id)
        {
            return ExecuteSingleRow<MemberInfo>(ds, Cs("Money", "FreezeMoney", "ParentId"), P("Id", id));
        }

        public static MemberInfo GetById(DataSource ds, long id, ColumnMode mode = ColumnMode.Exclude, params string[] columns)
        {
            columns = Exclude(columns, ColumnMode.Include, "Password", "PayPassword", "CreationDate");
            return ExecuteSingleRow<MemberInfo>(ds, columns, P("Id", id));
        }
        public static IList<MemberInfo> GetByIds(DataSource ds, long[] id)
        {
            return Db<MemberInfo>.Query(ds).Select(S("Id"), S("Sex"), S("Mobile"), S("Birthday"), S("Province"), S("City"), S("County"), S("Address"), S("Summary"), S("NickName"), S("Email"), S("Name"), S("RealName"), S("Image")).Where(W("Id", id, DbWhereType.In)).ToList<MemberInfo>();
        }
        public static bool CheckPayPassword(DataSource ds, long id, string pwd)
        {
            return string.Equals(pwd.MD5(), DataQuery
                .Select<MemberInfo>(ds, "PayPassword")
                .Where(P("Id", id))
                .Single<string>());
        }
        public static bool ApiCheckPayPassword(DataSource ds, long id, string pwd)
        {
            return string.Equals(pwd, DataQuery
                .Select<MemberInfo>(ds, "PayPassword")
                .Where(P("Id", id))
                .Single<string>());
        }
        public static Money GetMoney(DataSource ds, long id)
        {
            return Db<MemberInfo>.Query(ds)
                .Select("Money")
                .Where(W("Id", id))
                .Single<Money>();
        }

        public static SplitPageData<MemberInfo> GetByParentId(DataSource ds, long parentid, int index, int size, int show = 8)
        {
            long count;
            IList<MemberInfo> list = Db<MemberInfo>.Query(ds)
            .Select(S("Id"), S("Name"), S("Email"), S("VerMail"), S("Mobile"), S("VerMob"), S("Password"), S("RoleId"), S("CreationDate"), S("LoginNum"), S("LastIp"), S("LastTime"), S("Approved"), S("Locked"), S("LockNum"), S("LockTime")
            , S("Image"), S("NickName"), S("RealName"), S("Sex"), S("Birthday"), S("Province"), S("City"), S("County"), S("Address"), S("Summary")
            )
            .Where(W("ParentId", parentid))
            .OrderBy(D("CreationDate"))
            .ToList<MemberInfo>(size, index, out count);
            return new SplitPageData<MemberInfo>(index, size, list, count, show);
        }

    }
}
