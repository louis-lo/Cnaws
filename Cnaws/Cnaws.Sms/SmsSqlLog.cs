using Cnaws.Data;
using System;
using M = Cnaws.Sms.Modules;

namespace Cnaws.Sms
{
    public sealed class SmsSqlLog : ISmsLog
    {
        private DataSource _ds;

        public SmsSqlLog(DataSource ds)
        {
            _ds = ds;
        }

        public void Write(string provider, string log)
        {
            try
            {
                (new M.SmsLog() { Id = Guid.NewGuid(), Provider = provider, Message = log, CreationDate = DateTime.Now }).Insert(_ds);
            }
            catch (Exception) { }
        }
    }
}
