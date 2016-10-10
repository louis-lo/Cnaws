using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Cnaws.Area
{
    [Serializable]
    public sealed class IPLocation
    {
        private static readonly Regex Reg1 = new Regex(@"(\w+)省(\w+)市", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex Reg2 = new Regex(@"(\w+)省", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex Reg3 = new Regex(@"(\w+)市(\w+)(区|市)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex Reg4 = new Regex(@"\w\w(\w+)市", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex Reg5 = new Regex(@"(\w+)市", RegexOptions.Compiled | RegexOptions.Singleline);

        private string _country;
        private string _area;

        public IPLocation()
        {
            _country = null;
            _area = null;
        }
        public IPLocation(string country, string area)
        {
            _country = country;
            _area = area;
        }

        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }
        public string Area
        {
            get { return _area; }
            set { _area = value; }
        }

        private City GetCity(Country country, string province)
        {
            City city = country.GetProvince(province);
            if(city != null)
            {
                City[] cities = country.GetCities(city.Id);
                if (cities != null && cities.Length > 0)
                    return cities[0];
            }
            return null;
        }
        public City GetCity(Country country)
        {
            City city = null;
            Match match = Reg1.Match(Country);
            if (match.Success)
            {
                city = country.GetCity(match.Groups[2].Value);
                if (city == null)
                    city = GetCity(country, match.Groups[1].Value);
                if (city != null)
                    return city;
            }
            match = Reg3.Match(Country);
            if (match.Success)
            {
                city = country.GetCity(match.Groups[2].Value);
                if (city == null)
                    city = GetCity(country, match.Groups[1].Value);
                if (city != null)
                    return city;
            }
            match = Reg2.Match(Country);
            if (match.Success)
            {
                city = GetCity(country, match.Groups[1].Value);
                if (city != null)
                    return city;
            }
            match = Reg4.Match(Country);
            if (match.Success)
            {
                city = country.GetCity(match.Groups[1].Value);
                if (city != null)
                    return city;
            }
            match = Reg5.Match(Country);
            if (match.Success)
            {
                city = GetCity(country, match.Groups[1].Value);
                if (city != null)
                    return city;
            }
            return null;
        }

        public override string ToString()
        {
            return string.Concat(_country, " ", _area);
        }
    }

    public sealed class IPArea : IDisposable
    {
        private bool disposed;
        private MemoryStream _stream;

        private long _indexBegin;
        private long _indexEnd;
        private int _count;

        private long[] _ipBlock;

        public IPArea()
        {
            disposed = false;
            _stream = new MemoryStream(Properties.Resources.qqwry);
            _indexBegin = ReadLong();
            _indexEnd = ReadLong();
            _count = (int)((_indexEnd - _indexBegin) / 7 + 1);

            byte[] temp = new byte[3];
            _ipBlock = new long[_count];
            _stream.Position = _indexBegin;
            for (int i = 0; i < _count; ++i)
            {
                _ipBlock[i] = ReadLong();
                _stream.Position += 3;
            }
        }

        public IPLocation Search(string s)
        {
            long ip = IPToLong(s);
            long offset = Search(ip, 0, _ipBlock.Length - 1) * 7 + 4;
            _stream.Position = _indexBegin + offset;
            _stream.Position = ReadPosition() + 4;

            IPLocation loc = new IPLocation();
            int flag = _stream.ReadByte();
            if (flag == 1)
            {
                _stream.Position = ReadPosition();
                flag = _stream.ReadByte();
            }
            long index = _stream.Position;
            loc.Country = ReadString(flag);

            if (flag == 2)
                _stream.Position = index + 3;
            flag = _stream.ReadByte();
            loc.Area = ReadString(flag);

            return loc;
        }
        private int Search(long ip, int begin, int end)
        {
            int middle = (begin + end) / 2;
            if (middle == begin)
                return middle;
            else if (ip < _ipBlock[middle])
                return Search(ip, begin, middle);
            else
                return Search(ip, middle, end);
        }

        private long ReadLong()
        {
            byte[] bytes = new byte[8];
            _stream.Read(bytes, 0, 4);
            return BitConverter.ToInt64(bytes, 0);
        }
        private long ReadPosition()
        {
            byte[] bytes = new byte[8];
            _stream.Read(bytes, 0, 3);
            return BitConverter.ToInt64(bytes, 0);
        }
        private string ReadString(int flag)
        {
            if (flag == 1 || flag == 2)
                _stream.Position = ReadPosition();
            else
                _stream.Position -= 1;

            List<byte> list = new List<byte>();
            byte b = (byte)_stream.ReadByte();
            while (b > 0)
            {
                list.Add(b);
                b = (byte)_stream.ReadByte();
            }
            return Encoding.Default.GetString(list.ToArray());
        }
        private long IPToLong(string s)
        {
            if (s.IndexOf(':') != -1)
                s = "127.0.0.1";
            byte[] bytes = new byte[8];
            string[] arr = s.Split('.');
            for (int i = 0; i < 4; ++i)
                bytes[i] = byte.Parse(arr[3 - i]);
            return BitConverter.ToInt64(bytes, 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_stream != null)
                    {
                        _stream.Close();
                        _stream.Dispose();
                        _stream = null;
                    }
                }
                disposed = true;
            }
        }
        ~IPArea()
        {
            Dispose(false);
        }
    }
}
