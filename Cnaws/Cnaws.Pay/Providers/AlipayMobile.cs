using System;

namespace Cnaws.Pay.Providers
{
    internal sealed class AlipayMobile : Alipay
    {
        public override string Name
        {
            get { return "支付宝(移动支付)"; }
        }
        protected override string Service
        {
            get { return "alipay.wap.create.direct.pay.by.user"; }
        }
        protected override string SignType
        {
            get { return "MD5"; }
        }
        //protected override string PublicKey
        //{
        //    get { return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB"; }
        //}
    }
}
