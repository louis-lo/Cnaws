using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Sms.Modules;

namespace Cnaws.Sms.Controllers
{
    public abstract class Sms : DataController
    {
        public void SendImpl(string name, long mobile, string body, params string[] args)
        {
            SmsProvider provider = SmsProvider.Create(name);
            M.Sms sms = M.Sms.GetById(DataSource, provider.Key);
            if (!sms.Enabled)
                throw new Exception();
            provider.Account = sms.Account;
            provider.Token = sms.Token;
            provider.AppId = sms.AppId;
            provider.Log = new SmsSqlLog(DataSource);
            provider.Send(mobile, body, args);
        }

        public void SendTemplateImpl(string name, long mobile, string temp, params string[] args)
        {
            SmsProvider provider = SmsProvider.Create(name);
            M.Sms sms = M.Sms.GetById(DataSource, provider.Key);
            if (!sms.Enabled)
                throw new Exception();
            provider.Account = sms.Account;
            provider.Token = sms.Token;
            provider.AppId = sms.AppId;
            provider.Log = new SmsSqlLog(DataSource);
            provider.SendTemplate(mobile, temp, args);
        }
    }
}
