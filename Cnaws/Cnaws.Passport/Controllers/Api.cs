using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Passport.Modules;
using Cnaws.Web.Configuration;

namespace Cnaws.Passport.Controllers
{
    public enum ApiStatus
    {
        MarkError = -1020,
        TokenError = -1021,
        Exist = -1022
    }

    public class Api : DataController
    {
        //public void Register()
        //{
        //    object value = null;
        //    int result = (int)M.LoginStatus.DataError;
        //    string mark = Request["mark"];
        //    if (!string.IsNullOrEmpty(mark))
        //    {
        //        long pid = 0L;
        //        string pidstr = Request["parent"];//手机号
        //        if (!string.IsNullOrEmpty(pidstr))
        //        {
        //            long pm;
        //            if (long.TryParse(pidstr, out pm))
        //            {
        //                M.Member pmember = M.Member.GetByMobile(DataSource, pm);
        //                if (pmember != null)
        //                    pid = pmember.Id;
        //            }
        //        }
        //        string name = Request["name"];
        //        string password = Request["password"];
        //        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password))
        //        {
        //            switch (M.Member.Register(DataSource, M.RegisterType.Mobile, name, password, PassportSection.GetSection().DefaultApproved, pid))
        //            {
        //                case DataStatus.Success:
        //                    {
        //                        int errCount;
        //                        Guid token;
        //                        M.LoginStatus status = M.Member.Login(DataSource, name, password, ClientIp, mark, out errCount, out token);
        //                        switch (status)
        //                        {
        //                            case M.LoginStatus.PasswordError:
        //                                value = errCount;
        //                                break;
        //                            case M.LoginStatus.Success:
        //                                value = token.ToString("N");
        //                                break;
        //                        }
        //                        result = (int)status;
        //                    }
        //                    break;
        //                case DataStatus.Exist:
        //                    result = (int)ApiStatus.Exist;
        //                    break;
        //                case DataStatus.ExistOther:
        //                    result = (int)M.LoginStatus.PasswordError;
        //                    break;
        //                default:
        //                    result = (int)M.LoginStatus.DataError;
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            result = (int)M.LoginStatus.PasswordError;
        //        }
        //    }
        //    else
        //    {
        //        result = (int)ApiStatus.MarkError;
        //    }
        //    SetResult(result, value);
        //}
        //public void Login()
        //{
        //    object value = null;
        //    int result = (int)M.LoginStatus.DataError;
        //    string mark = Request["mark"];
        //    if (!string.IsNullOrEmpty(mark))
        //    {
        //        string name = Request["name"];
        //        string password = Request["password"];
        //        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password))
        //        {
        //            int errCount;
        //            Guid token;
        //            M.LoginStatus status = M.Member.Login(DataSource, name, password, ClientIp, mark, out errCount, out token);
        //            switch (status)
        //            {
        //                case M.LoginStatus.PasswordError:
        //                    value = errCount;
        //                    break;
        //                case M.LoginStatus.Success:
        //                    value = token.ToString("N");
        //                    break;
        //            }
        //            result = (int)status;
        //        }
        //        else
        //        {
        //            result = (int)M.LoginStatus.NotFound;
        //        }
        //    }
        //    else
        //    {
        //        result = (int)ApiStatus.MarkError;
        //    }
        //    SetResult(result, value);
        //}
    }
}
