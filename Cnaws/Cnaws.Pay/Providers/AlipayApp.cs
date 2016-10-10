using Cnaws.Web;
using System;
using System.Web;

namespace Cnaws.Pay.Providers
{
    internal sealed class AlipayApp : Alipay
    {
        private const string PublicKey = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";

        public override string Name
        {
            get { return "支付宝钱包"; }
        }

        public override string PartnerKey
        {
            get { return PublicKey; }
            set { }
        }

        protected override string SignType
        {
            get { return "RSA"; }
        }

        //public override bool Callback(Controller context, out PaymentResult result)
        //{
        //    throw new NotSupportedException();
        //}
    }
}
