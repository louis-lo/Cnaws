using System;
using System.Data;

namespace Cnaws.Area
{
    public enum CityLevel
    {
        Country = 0,
        Province = 1,
        City = 2,
        County = 3
    }

    [Serializable]
    public sealed class City
    {
        private int id;
        private string name;
        private int parent;
        private string sname;
        private CityLevel level;
        private string ccode;
        private string zcode;
        private double longitude;
        private double latitude;
        private string chinese;

        internal City() { }
        internal City(DataRow row)
        {
            id = (int)row["Id"];
            name = (string)row["Name"];
            parent = (int)row["ParentId"];
            sname = (string)row["ShortName"];
            level = GetLevel((short)row["Level"]);
            ccode = string.Concat("0", ((int)row["CityCode"]).ToString());
            zcode = ((int)row["ZipCode"]).ToString();
            longitude = (double)row["Longitude"];
            latitude = (double)row["Latitude"];
            chinese = (string)row["Chinese"];
        }

        private CityLevel GetLevel(short value)
        {
            switch (value)
            {
                case 0: return CityLevel.Country;
                case 1: return CityLevel.Province;
                case 2: return CityLevel.City;
                case 3: return CityLevel.County;
            }
            throw new ArgumentException();
        }

        public int Id { get { return id; } }
        public string Name { get { return name; } }
        public int ParentId { get { return parent; } }
        public string ShortName { get { return sname; } }
        public CityLevel Level { get { return level; } }
        public string CityCode { get { return ccode; } }
        public string ZipCode { get { return zcode; } }
        public double Longitude { get { return longitude; } }
        public double Latitude { get { return latitude; } }
        public string Chinese { get { return chinese; } }
        
        public override string ToString()
        {
            return string.Concat("ID：", id, "，全称：",name, "，父ID：", parent, "，简称：", sname, "，级别：", level, "，区号：", ccode, "，邮编：", zcode, "，经度：", longitude, "，纬度：", latitude, "，拼音：", chinese);
        }
    }
}
