using System;
using System.Collections.Generic;

namespace Cnaws.Pay.Providers
{
    internal sealed class AlipayGateway : Alipay
    {
        private static readonly Dictionary<string, string> _banks;

        static AlipayGateway()
        {
            _banks = new Dictionary<string, string>(31);
            
            _banks.Add("ICBC", "ICBC-DEBIT");//中国工商银行
            _banks.Add("ICBCB2B", "ICBCBTB");//中国工商银行（B2B）

            _banks.Add("ABC", "ABC");//中国农业银行
            _banks.Add("ABCB2B", "ABCBTB");//中国农业银行（B2B）

            _banks.Add("CCB", "CCB-DEBIT");//中国建设银行
            _banks.Add("CCBB2B", "CCBBTB");//中国建设银行（B2B）

            _banks.Add("SPDB", "SPDB-DEBIT");//上海浦东发展银行
            _banks.Add("SPDBB2B", "SPDBB2B");//上海浦东发展银行（B2B）

            _banks.Add("BOC", "BOC-DEBIT");//中国银行
            _banks.Add("BOCB2B", "BOCBTB");//中国银行（B2B）

            _banks.Add("CMB", "CMB-DEBIT");//招商银行
            _banks.Add("CMBB2B", "CMBBTB");//招商银行（B2B）

            _banks.Add("COMM", "COMM-DEBIT");//交通银行

            _banks.Add("CIB", "CIB");//兴业银行

            _banks.Add("GDB", "GDB-DEBIT");//广发银行

            _banks.Add("CEB", "CEB-DEBIT");//中国光大银行

            _banks.Add("PSBC", "PSBC-DEBIT");//中国邮政储蓄银行

            _banks.Add("BJBANK", "BJBANK");//北京银行

            _banks.Add("BJRCB", "BJRCB");//北京农村商业银行

            _banks.Add("SHRCB", "SHRCB");//上海农商银行

            _banks.Add("WZCBB2C", "WZCBB2C-DEBIT");//温州银行

            _banks.Add("CMBC", "CMBC");//中国民生银行

            _banks.Add("FDB", "FDB");//富滇银行

            _banks.Add("HZCBB2C", "HZCBB2C");//杭州银行

            _banks.Add("SHBANK", "SHBANK");//上海银行

            _banks.Add("NBBANK", "NBBANK");//宁波银行
            
            _banks.Add("SPA", "SPA-DEBIT");//平安银行

            _banks.Add("CITIC", "CITIC-DEBIT");//中信银行

            _banks.Add("POSTGC", "POSTGC");//中国邮政储蓄银行

            _banks.Add("abc1003", "abc1003");//visa

            _banks.Add("abc1004", "abc1004");//master
        }

        public override string Name
        {
            get { return "支付宝(网关支付)"; }
        }

        protected override void PackData(SortedDictionary<string, string> sParaTemp)
        {
            sParaTemp.Add("paymethod", "bankPay");
            sParaTemp.Add("defaultbank", "ICBC");
        }
    }
}
