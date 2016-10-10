using Cnaws.Templates;
using System;

namespace Cnaws.Sms
{
    public interface ISmsLog
    {
        void Write(string provider, string log);
    }

    public abstract class SmsProvider
    {
        public string Key
        {
            get { return GetType().Name; }
        }
        public abstract string Name
        {
            get;
        }
        public virtual Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }

        public string Account
        {
            get; set;
        }
        public string Token
        {
            get; set;
        }
        public string AppId
        {
            get; set;
        }

        public ISmsLog Log
        {
            get; set;
        }

        public static SmsProvider Create(string name)
        {
            try
            {
                Type type = Type.GetType(string.Concat("Cnaws.Sms.Providers.", name, ",Cnaws.Sms"), true, true);
                object result = Activator.CreateInstance(type);
                if (TType<SmsProvider>.Type.IsAssignableFrom(result.GetType()))
                    return (SmsProvider)result;
            }
            catch (Exception) { }
            return null;
        }

        public abstract void Send(long to, string body, params string[] arguments);
        public abstract void Send(long[] to, string body, params string[] arguments);
        public abstract void SendTemplate(long to, string tempId, params string[] arguments);
        public abstract void SendTemplate(long[] to, string tempId, params string[] arguments);

        protected void WriteLog(string log)
        {
            if (Log != null)
                Log.Write(Key, log);
        }
    }
}
