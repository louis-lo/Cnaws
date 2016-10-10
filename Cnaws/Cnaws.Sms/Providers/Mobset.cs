using Cnaws.Sms.WSRMobest;
using System;
using System.Text;

namespace Cnaws.Sms.Providers
{
    internal sealed class Mobset : SmsProvider
    {
        private const int ShortSms = 0;
        private const int LongSms = 1;
        private const int SendMax = 50;
        private const string AddNum = "001";
        private const string ApiUrl = "http://sms.mobset.com:8080/Api";

        private MobsetApi mobsetMms;

        public Mobset()
        {
            mobsetMms = new MobsetApi();
            mobsetMms.Url = ApiUrl;
        }

        public override string Name
        {
            get { return "首易"; }
        }

        private void SendImpl(long[] to, string body, params object[] arguments)
        {
            try
            {
                if (to == null || to.Length == 0)
                    throw new ArgumentNullException("to");

                if (string.IsNullOrEmpty(body))
                    throw new ArgumentNullException("body");

                string errMsg;
                SmsIDGroup[] smsIDGroup;

                string strMsg;
                if (arguments != null && arguments.Length > 0)
                    strMsg = string.Format(body, arguments);
                else
                    strMsg = body;

                int index = 0;
                while (index < to.Length)
                {
                    int count = Math.Min(to.Length - index, SendMax);
                    string strTimeStamp = DateTime.Now.ToString("MMddHHmmss");
                    string strInput = string.Concat(AppId, Token, strTimeStamp);
                    string strMd5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strInput, "MD5");

                    MobileListGroup[] strMobiles = new MobileListGroup[count];
                    for (int i = index; i < (index + count); ++i)
                        strMobiles[i - index] = new MobileListGroup() { Mobile = to[i].ToString() };

                    long ret = mobsetMms.Sms_Send(int.Parse(AppId), Account, strMd5, strTimeStamp, AddNum, string.Empty, LongSms, strMobiles, strMsg, out errMsg, out smsIDGroup);
                    if (ret > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Total:").Append(to.Length).Append(' ').Append("Succuss:").Append(ret).AppendLine();
                        foreach (SmsIDGroup item in smsIDGroup)
                            sb.Append(item.Mobile).Append(':').Append(item.SmsID).Append(',');
                        WriteLog(string.Concat("SendSMS", Environment.NewLine, sb.ToString()));
                    }
                    else
                    {
                        throw new Exception(errMsg);
                    }
                    index += count;
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Concat("SendSMS", Environment.NewLine, ex.ToString()));
            }
        }
        private object[] FormatArguments(string[] arguments)
        {
            if (arguments != null && arguments.Length > 0)
                return Array.ConvertAll(arguments, new Converter<string, object>((x) => x));
            return null;
        }

        public override void Send(long[] to, string body, params string[] arguments)
        {
            SendImpl(to, body, FormatArguments(arguments));
        }

        public override void Send(long to, string body, params string[] arguments)
        {
            SendImpl(new long[] { to }, body, FormatArguments(arguments));
        }

        public override void SendTemplate(long[] to, string tempId, params string[] arguments)
        {
            throw new NotImplementedException();
        }

        public override void SendTemplate(long to, string tempId, params string[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}
