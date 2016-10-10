using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;

namespace Cnaws.Management.Modules
{
    [Serializable]
    public sealed class Admin : IdentityModule, IPassportUserInfo
    {
        [DataColumn(32)]
        public string Name = null;
        [DataColumn(36)]
        public string Password = null;
        public int RoleId = 0;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        [DataColumn(256)]
        public string Email = null;
        [DataColumn(64)]
        public string LastIp = "127.0.0.1";
        public DateTime LastTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        public bool Locked = false;
        public int LockNum = 0;
        public DateTime LockTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        public long UserId = 0L;

        #region IPassportUserInfo
        long IPassportUserInfo.Id
        {
            get { return UserId; }
        }
        long IPassportUserInfo.AdminId
        {
            get { return Id; }
        }
        string IPassportUserInfo.Name
        {
            get { return Name; }
        }
        long IPassportUserInfo.RoleId
        {
            get { return 0; }
        }
        long IPassportUserInfo.AdminRoleId
        {
            get { return RoleId; }
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
            get { return 0; }
        }
        string IPassportUserInfo.UserData
        {
            get { return string.Empty; }
        }
        #endregion

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Name");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Name", "Name");
        }

        public static bool CheckName(DataSource ds, string name)
        {
            return ExecuteCount<Admin>(ds, P("Name", name)) == 0;
        }

        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            if (Id == 1)
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        public static DataStatus Delete(DataSource ds, int id)
        {
            return (new Admin() { Id = id }).Delete(ds);
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (!Utility.PassportNameRegularExpression.IsMatch(ref Name))
                return DataStatus.Failed;
            if (!Utility.PassportPasswordRegularExpression.IsMatch(Password))
                return DataStatus.Failed;
            if (!Utility.EmailRegularExpression.IsMatch(ref Email))
                return DataStatus.Failed;
            if (!CheckName(ds, Name))
                return DataStatus.Exist;
            Password = Password.MD5();
            return DataStatus.Success;
        }
        public static DataStatus Insert(DataSource ds, string name, string password, int roleId, string email)
        {
            return (new Admin() { Name = name, Password = password, RoleId = roleId, Email = email, CreationDate = DateTime.Now }).Insert(ds);
        }

        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "Name", "CreationDate");
            if (Include(columns, mode, "Password"))
            {
                if (!Utility.PassportPasswordRegularExpression.IsMatch(Password))
                    return DataStatus.Failed;
                Password = Password.MD5();
            }
            if (Include(columns, mode, "Email"))
            {
                if (!Utility.EmailRegularExpression.IsMatch(ref Email))
                    return DataStatus.Failed;
            }
            return DataStatus.Success;
        }
        public static DataStatus Update(DataSource ds, int id, string password, int roleId, string email)
        {
            Admin admin = new Admin() { Id = id, RoleId = roleId, Email = email };
            List<DataColumn> ignore = new List<DataColumn>();
            ignore.AddRange(new DataColumn[] { "RoleId", "Email" });
            if (!string.IsNullOrEmpty(password))
            {
                admin.Password = password;
                ignore.Add("Password");
            }
            return admin.Update(ds, ColumnMode.Include, ignore.ToArray());
        }

        public static Admin GetById(DataSource ds, int id)
        {
            return ExecuteSingleRow<Admin>(ds, Cs("Id", "Name", "RoleId", "Email"), P("Id", id));
        }
        public static SplitPageData<Admin> GetPage(DataSource ds, int index, int size, int show = 8)
        {
            int count;
            IList<Admin> list = ExecuteReader<Admin>(ds, Cs("Id", "Name", "RoleId", "CreationDate", "Email", "LastIp", "LastTime"), Os(Oa("Id")), index, size, out count);
            return new SplitPageData<Admin>(index, size, list, count, show);
        }
        public static int GetCountByRoleId(DataSource ds, int id)
        {
            return (int)ExecuteCount<Admin>(ds, P("RoleId", id));
        }
        public static Admin Login(DataSource ds, string name, string password, string lastIp, out int count)
        {
            count = PassportAuthentication.MaxInvalidPasswordAttempts;
            if (Utility.PassportNameRegularExpression.IsMatch(ref name))
            {
                if (Utility.PassportPasswordRegularExpression.IsMatch(password))
                {
                    Admin admin = ExecuteSingleRow<Admin>(ds, Cs("Id", "UserId", "Name", "Password", "RoleId", "CreationDate", "Locked", "LockNum", "LockTime"), P("Name", name));
                    if (admin != null)
                    {
                        if (string.Equals(admin.Password, password.MD5()))
                        {
                            if (admin.Locked)
                            {
                                DateTime now = DateTime.Now;
                                if (admin.LockTime.AddMinutes(PassportAuthentication.PasswordAnswerAttemptLockoutDuration) < now)
                                {
                                    admin.Locked = false;
                                    admin.LockNum = 0;
                                    admin.LastIp = lastIp;
                                    admin.LastTime = DateTime.Now;
                                    admin.Update(ds, ColumnMode.Include, "Locked", "LockNum", "LastIp", "LastTime");
                                }
                            }
                            else
                            {
                                admin.LockNum = 0;
                                admin.LastIp = lastIp;
                                admin.LastTime = DateTime.Now;
                                admin.Update(ds, ColumnMode.Include, "LockNum", "LastIp", "LastTime");
                            }
                            return admin;
                        }
                        else
                        {
                            if (admin.Locked)
                                return admin;
                            admin.LockNum = admin.LockNum + 1;
                            admin.Locked = admin.LockNum >= PassportAuthentication.MaxInvalidPasswordAttempts;
                            admin.LockTime = DateTime.Now;
                            admin.Update(ds, ColumnMode.Include, "Locked", "LockNum", "LockTime");
                            count = PassportAuthentication.MaxInvalidPasswordAttempts - admin.LockNum;
                            if (count < 0) count = 0;
                        }
                    }
                }
            }
            return null;
        }
        public static DataStatus ChangePassword(DataSource ds, int id, string newPass, string oldPass)
        {
            if (Utility.PassportPasswordRegularExpression.IsMatch(oldPass))
            {
                return (new Admin() { Id = id, Password = newPass }).Update(ds, ColumnMode.Include,
                        Cs("Password"),
                        WN("Password", oldPass.MD5(), "OldPassword") & P("Id", id));
            }
            return DataStatus.Failed;
        }
        public bool HasRight(DataSource ds, string right)
        {
            return AdminRole.HasRight(ds, RoleId, right);
        }
        public static bool HasRight(DataSource ds, int roleId, string right)
        {
            return AdminRole.HasRight(ds, roleId, right);
        }
    }
}
