using Cnaws.Data;
using System;
using M = Cnaws.Pay.Modules;

namespace Cnaws.Pay
{
    internal sealed class PaySqlLog : IPayLog
    {
        private DataSource _ds;

        public PaySqlLog(DataSource ds)
        {
            _ds = ds;
        }

        public void Write(string provider, string id, string log)
        {
            try
            {
                (new M.PayLog() { Provider = provider, TradeNo = id, Message = log, CreationDate = DateTime.Now }).Insert(_ds);
            }
            catch (Exception) { }
        }
    }
}
