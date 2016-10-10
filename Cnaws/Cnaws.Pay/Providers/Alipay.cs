using Cnaws.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cnaws.Pay.Providers
{
    internal abstract class Alipay : PayProvider
    {
        private static class AlipayRSA
        {
            public static string Sign(string content, string privateKey, string input_charset)
            {
                using (RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey))
                {
                    using (SHA1 sh = new SHA1CryptoServiceProvider())
                        return Convert.ToBase64String(rsa.SignData(Encoding.GetEncoding(input_charset).GetBytes(content), sh));
                }
            }
            public static bool Verify(string content, string signedString, string publicKey, string input_charset)
            {
                using (RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider())
                {
                    rsaPub.ImportParameters(ConvertFromPublicKey(publicKey));
                    using (SHA1 sh = new SHA1CryptoServiceProvider())
                        return rsaPub.VerifyData(Encoding.GetEncoding(input_charset).GetBytes(content), sh, Convert.FromBase64String(signedString));
                }
            }
            //public static string DecryptData(string resData, string privateKey, string input_charset)
            //{
            //    byte[] DataToDecrypt = Convert.FromBase64String(resData);
            //    List<byte> result = new List<byte>();
            //    for (int j = 0; j < DataToDecrypt.Length / 128; j++)
            //    {
            //        byte[] buf = new byte[128];
            //        for (int i = 0; i < 128; i++)
            //        {
            //            buf[i] = DataToDecrypt[i + 128 * j];
            //        }
            //        result.AddRange(Decrypt(buf, privateKey, input_charset));
            //    }
            //    byte[] source = result.ToArray();
            //    char[] asciiChars = new char[Encoding.GetEncoding(input_charset).GetCharCount(source, 0, source.Length)];
            //    Encoding.GetEncoding(input_charset).GetChars(source, 0, source.Length, asciiChars, 0);
            //    return new string(asciiChars);
            //}

            //private static byte[] Decrypt(byte[] data, string privateKey, string input_charset)
            //{
            //    using (RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey))
            //        return rsa.Decrypt(data, false);
            //}
            private static RSACryptoServiceProvider DecodePemPrivateKey(string pemstr)
            {
                byte[] pkcs8privatekey = Convert.FromBase64String(pemstr);
                if (pkcs8privatekey != null)
                    return DecodePrivateKeyInfo(pkcs8privatekey);
                return null;
            }
            private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
            {
                byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
                byte[] seq = new byte[15];

                using (MemoryStream mem = new MemoryStream(pkcs8))
                {
                    int lenstream = (int)mem.Length;
                    using (BinaryReader binr = new BinaryReader(mem))
                    {   //wrap Memory Stream with BinaryReader for easy reading
                        byte bt = 0;
                        ushort twobytes = 0;

                        try
                        {
                            twobytes = binr.ReadUInt16();
                            if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                                binr.ReadByte();    //advance 1 byte
                            else if (twobytes == 0x8230)
                                binr.ReadInt16();   //advance 2 bytes
                            else
                                return null;

                            bt = binr.ReadByte();
                            if (bt != 0x02)
                                return null;

                            twobytes = binr.ReadUInt16();

                            if (twobytes != 0x0001)
                                return null;

                            seq = binr.ReadBytes(15);       //read the Sequence OID
                            if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
                                return null;

                            bt = binr.ReadByte();
                            if (bt != 0x04) //expect an Octet string 
                                return null;

                            bt = binr.ReadByte();       //read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                            if (bt == 0x81)
                                binr.ReadByte();
                            else
                                if (bt == 0x82)
                                binr.ReadUInt16();
                            //------ at this stage, the remaining sequence should be the RSA private key

                            return DecodeRSAPrivateKey(binr.ReadBytes((int)(lenstream - mem.Position)));
                        }
                        catch (Exception) { }
                    }
                }

                return null;
            }
            private static bool CompareBytearrays(byte[] a, byte[] b)
            {
                if (a.Length != b.Length)
                    return false;
                int i = 0;
                foreach (byte c in a)
                {
                    if (c != b[i])
                        return false;
                    i++;
                }
                return true;
            }
            private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
            {
                byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

                // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
                using (MemoryStream mem = new MemoryStream(privkey))
                {
                    using (BinaryReader binr = new BinaryReader(mem))
                    {    //wrap Memory Stream with BinaryReader for easy reading
                        byte bt = 0;
                        ushort twobytes = 0;
                        int elems = 0;
                        try
                        {
                            twobytes = binr.ReadUInt16();
                            if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                                binr.ReadByte();    //advance 1 byte
                            else if (twobytes == 0x8230)
                                binr.ReadInt16();   //advance 2 bytes
                            else
                                return null;

                            twobytes = binr.ReadUInt16();
                            if (twobytes != 0x0102) //version number
                                return null;
                            bt = binr.ReadByte();
                            if (bt != 0x00)
                                return null;


                            //------  all private key components are Integer sequences ----
                            elems = GetIntegerSize(binr);
                            MODULUS = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            E = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            D = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            P = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            Q = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            DP = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            DQ = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            IQ = binr.ReadBytes(elems);

                            // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                            RSAParameters RSAparams = new RSAParameters();
                            RSAparams.Modulus = MODULUS;
                            RSAparams.Exponent = E;
                            RSAparams.D = D;
                            RSAparams.P = P;
                            RSAparams.Q = Q;
                            RSAparams.DP = DP;
                            RSAparams.DQ = DQ;
                            RSAparams.InverseQ = IQ;
                            RSA.ImportParameters(RSAparams);
                            return RSA;
                        }
                        catch (Exception) { }
                    }
                }

                return null;
            }
            private static int GetIntegerSize(BinaryReader binr)
            {
                byte bt = 0;
                byte lowbyte = 0x00;
                byte highbyte = 0x00;
                int count = 0;
                bt = binr.ReadByte();
                if (bt != 0x02)     //expect integer
                    return 0;
                bt = binr.ReadByte();

                if (bt == 0x81)
                    count = binr.ReadByte();    // data size in next byte
                else
                    if (bt == 0x82)
                {
                    highbyte = binr.ReadByte();	// data size in next 2 bytes
                    lowbyte = binr.ReadByte();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;		// we already have the data size
                }

                while (binr.ReadByte() == 0x00)
                {   //remove high order zeros in data
                    count -= 1;
                }
                binr.BaseStream.Seek(-1, SeekOrigin.Current);       //last ReadByte wasn't a removed zero, so back up a byte
                return count;
            }

            #region 解析.net 生成的Pem
            private static RSAParameters ConvertFromPublicKey(string pemFileConent)
            {
                byte[] keyData = Convert.FromBase64String(pemFileConent);
                if (keyData.Length < 162)
                    throw new ArgumentException("pem file content is incorrect.");
                byte[] pemModulus = new byte[128];
                byte[] pemPublicExponent = new byte[3];
                Array.Copy(keyData, 29, pemModulus, 0, 128);
                Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
                RSAParameters para = new RSAParameters();
                para.Modulus = pemModulus;
                para.Exponent = pemPublicExponent;
                return para;
            }
            //private static RSAParameters ConvertFromPrivateKey(string pemFileConent)
            //{
            //    byte[] keyData = Convert.FromBase64String(pemFileConent);
            //    if (keyData.Length < 609)
            //        throw new ArgumentException("pem file content is incorrect.");

            //    int index = 11;
            //    byte[] pemModulus = new byte[128];
            //    Array.Copy(keyData, index, pemModulus, 0, 128);

            //    index += 128;
            //    index += 2;//141
            //    byte[] pemPublicExponent = new byte[3];
            //    Array.Copy(keyData, index, pemPublicExponent, 0, 3);

            //    index += 3;
            //    index += 4;//148
            //    byte[] pemPrivateExponent = new byte[128];
            //    Array.Copy(keyData, index, pemPrivateExponent, 0, 128);

            //    index += 128;
            //    index += ((int)keyData[index + 1] == 64 ? 2 : 3);//279
            //    byte[] pemPrime1 = new byte[64];
            //    Array.Copy(keyData, index, pemPrime1, 0, 64);

            //    index += 64;
            //    index += ((int)keyData[index + 1] == 64 ? 2 : 3);//346
            //    byte[] pemPrime2 = new byte[64];
            //    Array.Copy(keyData, index, pemPrime2, 0, 64);

            //    index += 64;
            //    index += ((int)keyData[index + 1] == 64 ? 2 : 3);//412/413
            //    byte[] pemExponent1 = new byte[64];
            //    Array.Copy(keyData, index, pemExponent1, 0, 64);

            //    index += 64;
            //    index += ((int)keyData[index + 1] == 64 ? 2 : 3);//479/480
            //    byte[] pemExponent2 = new byte[64];
            //    Array.Copy(keyData, index, pemExponent2, 0, 64);

            //    index += 64;
            //    index += ((int)keyData[index + 1] == 64 ? 2 : 3);//545/546
            //    byte[] pemCoefficient = new byte[64];
            //    Array.Copy(keyData, index, pemCoefficient, 0, 64);

            //    RSAParameters para = new RSAParameters();
            //    para.Modulus = pemModulus;
            //    para.Exponent = pemPublicExponent;
            //    para.D = pemPrivateExponent;
            //    para.P = pemPrime1;
            //    para.Q = pemPrime2;
            //    para.DP = pemExponent1;
            //    para.DQ = pemExponent2;
            //    para.InverseQ = pemCoefficient;
            //    return para;
            //}
            #endregion

        }
        private static class AlipayMD5
        {
            public static string Sign(string prestr, string _key, string _input_charset)
            {
                byte[] t;
                StringBuilder sb = new StringBuilder(32);
                using (MD5 md5 = new MD5CryptoServiceProvider())
                    t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(string.Concat(prestr, _key)));
                for (int i = 0; i < t.Length; i++)
                    sb.Append(t[i].ToString("x2"));
                return sb.ToString();
            }
            public static bool Verify(string prestr, string sign, string _key, string _input_charset)
            {
                return string.Equals(sign, Sign(prestr, _key, _input_charset));
            }
        }

        protected sealed override string GatewayUrl
        {
            get { return "https://mapi.alipay.com/gateway.do?"; }
        }
        protected virtual string Service
        {
            get { return "create_direct_pay_by_user"; }
        }
        protected virtual string SignType
        {
            get { return "MD5"; }
        }
        protected virtual string PublicKey
        {
            get { return PartnerKey; }
        }

        protected virtual void PackData(SortedDictionary<string, string> sParaTemp)
        {
        }
        public sealed override SortedDictionary<string, string> PackData(IPayOrder order)
        {
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("service", Service);
            sParaTemp.Add("partner", Partner);
            sParaTemp.Add("_input_charset", Charset);

            sParaTemp.Add("notify_url", AsyncCallbackUrl);
            sParaTemp.Add("return_url", CallbackUrl);

            PackData(sParaTemp);

            sParaTemp.Add("out_trade_no", order.TradeNo);
            sParaTemp.Add("subject", order.Subject);
            sParaTemp.Add("payment_type", "1");
            sParaTemp.Add("total_fee", order.TotalFee.ToString("F2"));
            sParaTemp.Add("seller_id", PartnerId);

            sParaTemp.Add("sign", BuildRequestMysign(FilterPara(sParaTemp)));
            sParaTemp.Add("sign_type", SignType);

            return sParaTemp;
        }
        public override string NewBatchNo(int no)
        {
            return string.Concat(DateTime.Now.ToString("yyyyMMdd"), no);
        }
        private static unsafe string Format(string s)
        {
            int len = Math.Min(128, s.Length);
            if (len < s.Length)
                s = s.Substring(0, len);
            StringBuilder sb = new StringBuilder(len);
            fixed (char* ptr = s)
            {
                char* end = ptr + s.Length;
                for (char* p = ptr; p != end; ++p)
                {
                    switch (*p)
                    {
                        case '^':
                        case '|':
                        case '$':
                        case '#':
                            break;
                        default:
                            sb.Append(*p);
                            break;
                    }
                }
            }
            return sb.ToString();
        }
        private static string Format(IRefundOrder order)
        {
            return string.Concat(order.PayTradeNo, '^', order.TotalFee.ToString("F2"), '^', Format(order.Subject));
        }
        private static string Format(IRefundOrder[] order)
        {
            return string.Join("#", Array.ConvertAll(order, new Converter<IRefundOrder, string>((x) => Format(x))));
        }
        public sealed override SortedDictionary<string, string> PackData(string batchNo, params IRefundOrder[] orders)
        {
            //一笔交易可以多次退款，退款次数最多不能超过99次
            if (orders == null)
                throw new ArgumentNullException("orders");

            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("service", "refund_fastpay_by_platform_pwd");
            sParaTemp.Add("partner", Partner);
            sParaTemp.Add("_input_charset", Charset);

            sParaTemp.Add("notify_url", AsyncCallbackUrl);

            sParaTemp.Add("seller_user_id", PartnerId);
            sParaTemp.Add("refund_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sParaTemp.Add("batch_no", batchNo);
            sParaTemp.Add("batch_num", orders.Length.ToString());
            sParaTemp.Add("detail_data", Format(orders));

            sParaTemp.Add("sign", BuildRequestMysign(FilterPara(sParaTemp)));
            sParaTemp.Add("sign_type", SignType);

            return sParaTemp;
        }
        public override string Submit(Controller context, SortedDictionary<string, string> para, string button, string url)
        {
            Dictionary<string, string> dicPara = BuildRequestPara(para);
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='").Append(GatewayUrl).Append("_input_charset=").Append(Charset).Append("' method='").Append(Method).Append("'>");
            foreach (KeyValuePair<string, string> temp in dicPara)
                sbHtml.Append("<input type='hidden' name='").Append(temp.Key).Append("' value='").Append(temp.Value).Append("'/>");
            sbHtml.Append("<input type='submit' value='").Append(button).Append("' style='display:none;'></form>");
            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");
            return sbHtml.ToString();
        }
        public override string Refund(Controller context, SortedDictionary<string, string> para, string button)
        {
            return Submit(context, para, button, null);
        }
        public override bool Callback(Controller context, out PaymentResult result)
        {
            PayResult presult = new PayResult();
            SortedDictionary<string, string> sPara = GetRequest(context.Request.QueryString);
            if (sPara.Count > 0)
            {
                string id = context.Request.QueryString["out_trade_no"];
                presult.NotifyId = context.Request.QueryString["notify_id"];
                if (Verify(sPara, presult.NotifyId, context.Request.QueryString["sign"], id))
                {
                    presult.TradeNo = id;
                    presult.PayTradeNo = context.Request.QueryString["trade_no"];
                    presult.Status = context.Request.QueryString["trade_status"];
                    presult.TotalFee = Money.Parse(context.Request.QueryString["total_fee"]);
                    if ("TRADE_FINISHED".Equals(presult.Status) || "TRADE_SUCCESS".Equals(presult.Status))
                    {
                        presult.Message = "验证成功";
                        result = presult;
                        return true;
                    }
                    else
                    {
                        presult.Message = string.Concat("trade_status=", presult.Status);
                    }
                }
                else
                {
                    presult.Message = "验证失败";
                }
            }
            else
            {
                presult.Message = "无返回参数";
            }
            result = presult;
            return false;
        }
        private static RefundResult FormatResult(string s)
        {
            RefundResult result = new RefundResult();
            if (!string.IsNullOrEmpty(s))
            {
                string[] group, temp;
                string[] array = s.Split('#');
                foreach (string line in array)
                {
                    group = line.Split('$');
                    if (group.Length > 0)
                    {
                        temp = group[0].Split('^');
                        RefundInfo info = new RefundInfo();
                        if (temp.Length > 0)
                            info.PayTradeNo = temp[0];
                        if (temp.Length > 1)
                            info.TotalFee = Money.Parse(temp[1]);
                        if (temp.Length > 2)
                            info.Status = "SUCCESS".Equals(temp[2]);
                        //if (group.Length > 1)
                        //{
                        //    temp = group[1].Split('^');
                        //    if (temp.Length > 0)
                        //        info.Account = temp[0];
                        //    if (temp.Length > 1)
                        //        info.Fees = Money.Parse(temp[1]);
                        //    if (temp.Length > 2)
                        //        info.FeesStatus = "SUCCESS".Equals(temp[2]);
                        //}
                        result.Results.Add(info);
                    }
                }
            }
            return result;
        }
        public sealed override bool AsyncCallback(Controller context, out PaymentResult result)
        {
            PayResult presult = new PayResult();
            SortedDictionary<string, string> sPara = GetRequest(context.Request.Form);
            if (sPara.Count > 0)
            {
                bool refund = "batch_refund_notify".Equals(context.Request.Form["notify_type"]);
                string id = refund ? context.Request.Form["batch_no"] : context.Request.Form["out_trade_no"];
                presult.NotifyId = context.Request.Form["notify_id"];
                if (Verify(sPara, presult.NotifyId, context.Request.Form["sign"], id))
                {
                    if (refund)
                    {
                        string details = context.Request.Form["result_details"];
                        RefundResult rresult = FormatResult(details);
                        rresult.NotifyId = presult.NotifyId;
                        rresult.BatchNo = id;
                        rresult.SuccessNum = int.Parse(context.Request.Form["success_num"]);
                        rresult.Message = details;
                        result = rresult;
                        return true;
                    }
                    else
                    {
                        presult.TradeNo = id;
                        presult.PayTradeNo = context.Request.Form["trade_no"];
                        presult.Status = context.Request.Form["trade_status"];
                        presult.TotalFee = Money.Parse(context.Request.Form["total_fee"]);
                        if ("TRADE_FINISHED".Equals(presult.Status) || "TRADE_SUCCESS".Equals(presult.Status))
                        {
                            presult.Message = "验证成功";
                            result = presult;
                            return true;
                        }
                        else
                        {
                            presult.Message = string.Concat("trade_status=", presult.Status);
                        }
                    }
                }
                else
                {
                    presult.Message = "验证失败";
                }
            }
            else
            {
                presult.Message = "无返回参数";
            }
            result = presult;
            return false;
        }

        private SortedDictionary<string, string> GetRequest(NameValueCollection coll)
        {
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            foreach (string key in coll.Keys)
                sArray.Add(key, coll[key]);
            return sArray;
        }
        private Dictionary<string, string> FilterPara(SortedDictionary<string, string> dicArrayPre)
        {
            Dictionary<string, string> dicArray = new Dictionary<string, string>(dicArrayPre.Count);
            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                if (!"sign".Equals(temp.Key, StringComparison.OrdinalIgnoreCase)
                    && !"sign_type".Equals(temp.Key, StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrEmpty(temp.Value))
                    dicArray.Add(temp.Key, temp.Value);
            }
            return dicArray;
        }
        private string CreateLinkString(Dictionary<string, string> dicArray)
        {
            List<string> prestr = new List<string>(dicArray.Count);
            foreach (KeyValuePair<string, string> temp in dicArray)
                prestr.Add(string.Concat(temp.Key, '=', temp.Value));
            return string.Join("&", prestr.ToArray());
        }
        private string BuildRequestMysign(Dictionary<string, string> sPara)
        {
            switch (SignType)
            {
                case "MD5": return AlipayMD5.Sign(CreateLinkString(sPara), PartnerKey, Charset);
                case "RSA": return AlipayRSA.Sign(CreateLinkString(sPara), PartnerKey, Charset);
            }
            return string.Empty;
        }
        private bool Verify(SortedDictionary<string, string> inputPara, string notify_id, string sign, string id)
        {
            bool isSign = GetSignVeryfy(inputPara, sign);
            string responseTxt = "false";
            if (!string.IsNullOrEmpty(notify_id))
                responseTxt = GetResponseTxt(notify_id);
            WriteLog(id, string.Concat("responseTxt=", responseTxt, "\r\n isSign=", isSign, "\r\n 返回回来的参数：", GetPreSignStr(inputPara)));
            //判断responsetTxt是否为true，isSign是否为true
            //responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
            //isSign不是true，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关
            return isSign && "true".Equals(responseTxt);
        }
        private bool GetSignVeryfy(SortedDictionary<string, string> inputPara, string sign)
        {
            if (!string.IsNullOrEmpty(sign))
            {
                switch (SignType)
                {
                    case "MD5": return AlipayMD5.Verify(CreateLinkString(FilterPara(inputPara)), sign, PartnerKey, Charset);
                    case "RSA": return AlipayRSA.Verify(CreateLinkString(FilterPara(inputPara)), sign, PublicKey, Charset);
                }
            }
            return false;
        }
        private string GetResponseTxt(string notify_id)
        {
            string result;
            HttpRequest(string.Concat("https://mapi.alipay.com/gateway.do?service=notify_verify&partner=", Partner, "&notify_id=", notify_id), out result, Encoding.Default);
            return result;
        }
        private Dictionary<string, string> BuildRequestPara(SortedDictionary<string, string> sParaTemp)
        {
            Dictionary<string, string> sPara = FilterPara(sParaTemp);
            sPara.Add("sign", BuildRequestMysign(sPara));
            sPara.Add("sign_type", SignType);
            return sPara;
        }
        private string GetPreSignStr(SortedDictionary<string, string> inputPara)
        {
            return CreateLinkString(FilterPara(inputPara));
        }
    }
}
