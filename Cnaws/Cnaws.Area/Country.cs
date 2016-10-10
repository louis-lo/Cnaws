using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Cnaws.Area
{
    public abstract class Country : IDisposable
    {
        private bool disposed;
        private DataTable _table;

        protected Country()
        {
            disposed = false;
            _table = new DataTable();
            _table.CaseSensitive = false;
            using(Stream s = GetDataStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                _table = (DataTable)formatter.Deserialize(s);
                s.Close();
            }
        }

        protected abstract Stream GetDataStream();

        public static Country GetCountry(string name = null)
        {
            return new China();
        }
        public City[] GetAll()
        {
            DataRow[] rows = _table.Select();
            City[] cities = new City[rows.Length];
            for (int i = 0; i < rows.Length; ++i)
                cities[i] = new City(rows[i]);
            return cities;
        }
        public Dictionary<string, City> GetAllJson()
        {
            City city;
            DataRow[] rows = _table.Select();
            Dictionary<string, City> cities = new Dictionary<string, City>(rows.Length);
            for (int i = 0; i < rows.Length; ++i)
            {
                city = new City(rows[i]);
                cities.Add(city.Id.ToString(), city);
            }
            return cities;
        }

        public City[] GetProvinces()
        {
            DataRow[] rows = _table.Select("Level=1");
            City[] cities = new City[rows.Length];
            for (int i = 0; i < rows.Length; ++i)
                cities[i] = new City(rows[i]);
            return cities;
        }
        public City[] GetCities(int parent)
        {
            DataRow[] rows = _table.Select(string.Concat("ParentId=", parent, " AND Level=2"));
            City[] cities = new City[rows.Length];
            for (int i = 0; i < rows.Length; ++i)
                cities[i] = new City(rows[i]);
            return cities;
        }
        public City[] GetCounties(int parent)
        {
            DataRow[] rows = _table.Select(string.Concat("ParentId=", parent, " AND Level=3"));
            City[] cities = new City[rows.Length];
            for (int i = 0; i < rows.Length; ++i)
                cities[i] = new City(rows[i]);
            return cities;
        }
        public City[] GetParent(int value)
        {
            List<City> list = new List<City>(4);
            do
            {
                DataRow[] rows = _table.Select(string.Concat("Id=", value));
                if (rows.Length > 0)
                {
                    City row = new City(rows[0]);
                    list.Insert(0, row);
                    value = row.ParentId;
                }
            } while (value > 0);
            return list.ToArray();
        }

        public City GetCity(int id)
        {
            DataRow[] rows = _table.Select(string.Concat("Id=", id));
            if (rows.Length > 0)
                return new City(rows[0]);
            return null;
        }
        public City GetProvince(string name)
        {
            DataRow[] rows = _table.Select(string.Concat("Name LIKE '", name, "%' AND Level=1"));
            if (rows.Length > 0)
                return new City(rows[0]);
            return null;
        }
        public City GetCity(string name)
        {
            DataRow[] rows = _table.Select(string.Concat("Name LIKE '", name, "%' AND Level=2"));
            if (rows.Length > 0)
                return new City(rows[0]);
            return null;
        }
        public City GetCounty(string name)
        {
            DataRow[] rows = _table.Select(string.Concat("Name LIKE '", name, "%' AND Level=3"));
            if (rows.Length > 0)
                return new City(rows[0]);
            return null;
        }

        public City GetCityByPingYin(string name)
        {
            DataRow[] rows = _table.Select(string.Concat("Chinese='", name.Replace("-", "''"), "' AND Level=2"));
            if (rows.Length > 0)
                return new City(rows[0]);
            return null;
        }

        public Dictionary<string, List<KeyValuePair<string, string>>> GetAllCityPingYin()
        {
            string ch;
            List<KeyValuePair<string, string>> tmp;
            City[] cities = GetAll();
            Dictionary<string, List<KeyValuePair<string, string>>> dict = new Dictionary<string, List<KeyValuePair<string, string>>>();
            foreach (City c in cities)
            {
                if (c.Level == CityLevel.City && c.Chinese != null && c.Chinese.Length > 0)
                {
                    ch = c.Chinese[0].ToString();
                    if (dict.TryGetValue(ch, out tmp))
                    {
                        tmp.Add(new KeyValuePair<string, string>(c.Chinese, c.ShortName));
                    }
                    else
                    {
                        tmp = new List<KeyValuePair<string, string>>();
                        tmp.Add(new KeyValuePair<string, string>(c.Chinese, c.ShortName));
                        dict.Add(ch, tmp);
                    }
                }
            }
            return dict;
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
                    if (_table != null)
                    {
                        _table.Dispose();
                        _table = null;
                    }
                }
                disposed = true;
            }
        }
        ~Country()
        {
            Dispose(false);
        }
    }
}
