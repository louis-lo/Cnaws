using Cnaws.Web;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Cnaws.Pay.Providers
{
    internal sealed class Balance : PayProvider
    {
        public override string Name
        {
            get { return "余额支付"; }
        }
        protected override string GatewayUrl
        {
            get { return null; }
        }
        public override bool IsNeedNotify
        {
            get { return false; }
        }
        public override bool IsOnlinePay
        {
            get { return false; }
        }

        public override bool AsyncCallback(Controller context, out PaymentResult result)
        {
            throw new NotSupportedException();
        }
        public override void AsyncStop(Controller context)
        {
            throw new NotSupportedException();
        }
        public override string Submit(Controller context, SortedDictionary<string, string> para, string button, string url)
        {
            throw new NotSupportedException();
        }
        public override bool Callback(Controller context, out PaymentResult result)
        {
            throw new NotSupportedException();
        }
        public override string NewBatchNo(int no)
        {
            throw new NotSupportedException();
        }
        public override SortedDictionary<string, string> PackData(IPayOrder order)
        {
            throw new NotSupportedException();
        }
        public override SortedDictionary<string, string> PackData(string batchNo, IRefundOrder[] orders)
        {
            throw new NotSupportedException();
        }
    }
}
