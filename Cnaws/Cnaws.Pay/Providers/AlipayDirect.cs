using System;
using System.Collections.Generic;

namespace Cnaws.Pay.Providers
{
    internal class AlipayDirect : Alipay
    {
        public override string Name
        {
            get { return "支付宝(即时到账)"; }
        }
    }

    internal sealed class AlipayQR : AlipayDirect
    {
        public override string Name
        {
            get { return "支付宝(扫码支付)"; }
        }

        protected override void PackData(SortedDictionary<string, string> sParaTemp)
        {
            sParaTemp.Add("qr_pay_mode", "2");
        }
    }
}
