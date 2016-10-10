using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cnaws.Web.Caching
{
    internal sealed class RedisCache : CacheProvider
    {
        public static readonly RedisCache Instance;

        private readonly static byte[] SET = Encoding.UTF8.GetBytes("SET");
        private readonly static byte[] GET = Encoding.UTF8.GetBytes("GET");
        private readonly static byte[] DEL = Encoding.UTF8.GetBytes("DEL");
        private readonly static byte[] FLUSHDB = Encoding.UTF8.GetBytes("FLUSHDB");//FLUSHALL
        private readonly byte[] EndData = new[] { (byte)'\r', (byte)'\n' };

        private Socket _socket;

        static RedisCache()
        {
            Instance = new RedisCache();
        }
        private RedisCache()
        {
            _socket = null;
        }
        private void SafeClose()
        {
            if (_socket != null)
            {
                _socket.Dispose();
                _socket = null;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                SafeClose();
        }

        private void Connect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _socket.Connect("127.0.0.1", 6379);
                if (!_socket.Connected)
                {
                    _socket.Dispose();
                    _socket = null;
                }
            }
            catch (SocketException) { }
        }
        private bool Reconnect()
        {
            SafeClose();
            Connect();
            return _socket != null;
        }
        public static bool IsConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }
        private void TryConnectIfNeeded()
        {
            if (_socket == null)
            {
                Connect();
            }
            else
            {
                if (!IsConnected(_socket))
                    Reconnect();
            }
        }

        private void WriteHead(Stream buffer, int size)
        {
            byte[] temp = Encoding.UTF8.GetBytes(string.Concat('*', size));
            buffer.Write(temp, 0, temp.Length);
            buffer.Write(EndData, 0, EndData.Length);
        }
        private void Write(Stream buffer, params byte[][] datas)
        {
            byte[] temp;
            foreach (byte[] data in datas)
            {
                temp = Encoding.UTF8.GetBytes(string.Concat('$', data.Length));
                buffer.Write(temp, 0, temp.Length);
                buffer.Write(EndData, 0, EndData.Length);
                buffer.Write(data, 0, data.Length);
                buffer.Write(EndData, 0, EndData.Length);
            }
        }
        private byte[] SendReceive(bool format, byte[] command, byte[] key = null, byte[] value = null)
        {
            try
            {
                TryConnectIfNeeded();

                using (MemoryStream ms = new MemoryStream())
                {
                    int size = 1;
                    if (key != null) ++size;
                    if (value != null) ++size;
                    WriteHead(ms, size);
                    if (key != null) Write(ms, key);
                    if (value != null) Write(ms, value);
                    _socket.Send(ms.ToArray());
                }

                byte[] buff = new byte[16 * 1024];
                int count = _socket.Receive(buff);

                string s = Encoding.UTF8.GetString(buff, 0, count);
                return null;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        
        protected override string FormatKey(string key)
        {
            return key;
        }
        protected override string FormatKeys(params string[] keys)
        {
            return string.Join(".", keys);
        }
        public override void Clear()
        {
            SendReceive(false, FLUSHDB);
        }
        protected override void DeleteImpl(string key)
        {
            SendReceive(false, DEL, Encoding.UTF8.GetBytes(key));
        }
        protected override object GetImpl(string key)
        {
            byte[] data = SendReceive(true, GET, Encoding.UTF8.GetBytes(key));
            if (data != null)
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    BinaryFormatter format = new BinaryFormatter();
                    return format.Deserialize(ms);
                }
            }
            return null;
        }
        protected override void SetImpl(string key, object value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter format = new BinaryFormatter();
                format.Serialize(ms, value);
                SendReceive(false, SET, Encoding.UTF8.GetBytes(key), ms.ToArray());
            }
        }
    }
}
