using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnaws.Pay.Providers
{
    internal sealed class AlipayTrad : Alipay
    {
        public override string Name
        {
            get { return "支付宝(担保交易)"; }
        }


    }
}
