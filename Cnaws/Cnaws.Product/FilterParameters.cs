using Cnaws.Data;
using Cnaws.Product.Modules;
using Cnaws.Web;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Cnaws.Product
{
    public sealed class FilterParameters2
    {
        private long _page;
        private int _size;
        private int _brand;
        private int _orderBy;
        private string _keyword;
        private string _attribute;
        private int _suppliertype;
        private long _storeid;
        private int _storecategoryid;
        private string _price;
        private int _province;
        private int _city;
        private int _county;
        private SortedDictionary<long, object> _parameters;

        public FilterParameters2()
        {
            _page = 1L;
            _size = 10;
            _brand = 0;
            _orderBy = 0;
            _keyword = string.Empty;
            _attribute = string.Empty;
            _suppliertype = -1;
            _storeid = 0L;
            _storecategoryid = 0;
            _price = string.Empty;
            _province = 0;
            _city = 0;
            _county = 0;
            _parameters = new SortedDictionary<long, object>();
        }
        private FilterParameters2(FilterParameters2 value)
        {
            _page = 1;
            _size = value._size;
            _brand = value._brand;
            _orderBy = value._orderBy;
            _keyword = value._keyword;
            _attribute = value._attribute;
            _suppliertype = value._suppliertype;
            _storeid = value._storeid;
            _storecategoryid = value._storecategoryid;
            _price = value._price;
            _province = value._province;
            _city = value._city;
            _county = value._county;
            _parameters = new SortedDictionary<long, object>(value._parameters);
        }
        public FilterParameters2(IList<ProductAttribute> args)
            : this()
        {
            //foreach (ProductAttribute pa in args)
            //    _parameters.Add(pa.Id, null);
        }
        public void Load(long page, Arguments args)
        {
            Page = page;
            if (args != null)
            {
                if (args.Count > 0)
                {
                    try { Brand = int.Parse(args[0]); }
                    catch (Exception) { }
                }
                if (args.Count > 1)
                {
                    try { OrderBy = int.Parse(args[1]); }
                    catch (Exception) { }
                }
                if (args.Count > 2)
                {
                    try { Attribute = args[2].ToString(); }
                    catch (Exception) { }
                }
                if (args.Count > 3)
                {
                    try { StoreId = long.Parse(args[3]); }
                    catch (Exception) { }
                }
                if (args.Count > 4)
                {
                    try { Price = args[4].ToString(); }
                    catch (Exception) { }
                }
                if (args.Count > 5)
                {
                    long[] keys = new long[_parameters.Keys.Count];
                    _parameters.Keys.CopyTo(keys, 0);
                    for (int i = 2; i < Math.Min(args.Count, _parameters.Count); ++i)
                        _parameters[keys[i - 2]] = args[i];
                }
            }
        }

        public FilterParameters2 CopyByPage(long page)
        {
            FilterParameters2 fp = new FilterParameters2(this);
            fp.Page = page;
            return fp;
        }
        public FilterParameters2 CopyByBrand(int brand)
        {
            FilterParameters2 fp = new FilterParameters2(this);
            fp.Brand = brand;
            return fp;
        }
        public FilterParameters2 CopyByStore(int storeid)
        {
            FilterParameters2 fp = new FilterParameters2(this);
            fp.StoreId = storeid;
            return fp;
        }
        public FilterParameters2 CopyByOrderBy(int orderBy)
        {
            FilterParameters2 fp = new FilterParameters2(this);
            fp.OrderBy = orderBy;
            return fp;
        }
        public FilterParameters2 CopyByPrice(string price1,string price2)
        {
            FilterParameters2 fp = new FilterParameters2(this);
            fp.Price = $"{price1}-{price2}";
            return fp;
        }
        public FilterParameters2 CopyByAttr(long attrId, string attrVal)
        {
            FilterParameters2 fp = new FilterParameters2(this);

            string newAttr = "";
            if (!string.IsNullOrEmpty(fp.Attribute))
            {
                if (fp.Attribute.IndexOf('@') != -1)
                {
                    string[] Attributes = fp.Attribute.Split('@');
                    foreach (string Attr_Item in Attributes)
                    {
                        if (!string.IsNullOrEmpty(Attr_Item))
                        {

                            if (Attr_Item.IndexOf('_') != -1)
                            {
                                string[] Attr_Value = Attr_Item.Split('_');
                                if (!string.IsNullOrEmpty(Attr_Value[0]) && !string.IsNullOrEmpty(Attr_Value[1]))
                                {
                                    if (long.Parse(Attr_Value[0]) != attrId)
                                        newAttr += $"@{ Attr_Value[0]}_{ Attr_Value[1]}";
                                }
                            }
                        }
                    }
                }
            }
            newAttr += $"@{attrId}_{attrVal}";
            fp.Attribute = newAttr;
            return fp;
        }
        public object this[long key]
        {
            get
            {
                object value;
                if (_parameters.TryGetValue(key, out value))
                    return FormatValue(value).ToString();
                return null;
            }
            set
            {
                if (_parameters.ContainsKey(key))
                    _parameters[key] = value;
            }
        }
        public bool Is(long key, string value)
        {
            return string.Equals(this[key] as string, value);
        }

        private object FormatValue(object value)
        {
            if (value == null || (value is string && string.IsNullOrEmpty((string)value)))
                return '_';
            if (value is bool)
                return (bool)value ? 1 : 0;
            if (value.GetType().IsEnum)
                return (int)value;
            return value;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('/').Append(_page);
            sb.Append('/').Append(_brand);           
            sb.Append('/').Append(_orderBy);
            sb.Append('/').Append(FormatValue(_attribute).ToString());
            sb.Append('/').Append(_storeid);
            sb.Append('/').Append(FormatValue(_price).ToString());
            //foreach (long key in _parameters.Keys)
            //    sb.Append('/').Append(HttpUtility.UrlEncode(FormatValue(_parameters[key]).ToString()));
            return sb.ToString();
        }
        public long Page
        {
            get { return _page; }
            set { _page = Math.Max(1, value); }
        }
        public int Size
        {
            get { return _size; }
            set { _size = Math.Max(0, value); }
        }
        public int Brand
        {
            get { return _brand; }
            set { _brand = Math.Max(0, value); }
        }
        public int OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = Math.Max(0, value); }
        }
        public string KeyWord
        {
            get { return _keyword; }
            set { _keyword = value; }
        }
        public string Attribute
        {
            get { return _attribute; }
            set { _attribute = value; }
        }
        public int SupplierType
        {
            get { return _suppliertype; }
            set { _suppliertype = value; }
        }
        public long StoreId
        {
            get { return _storeid; }
            set { _storeid = value; }
        }
        public int StoreCategoryId
        {
            get { return _storecategoryid; }
            set { _storecategoryid = value; }
        }
        public string Price
        {
            get { return _price; }
            set { _price = value; }
        }
        public int Province
        {
            get { return _province; }
            set { _province = value; }
        }

        public int City
        {
            get { return _city; }
            set { _city = value; }
        }

        public int County
        {
            get { return _county; }
            set { _county = value; }
        }

    }

    public sealed class FilterParameters
    {
        private long _page;
        private int _brand;
        private int _orderBy;
        private SortedDictionary<long, object> _parameters;

        public FilterParameters()
        {
            _page = 1L;
            _brand = 0;
            _orderBy = 0;
            _parameters = new SortedDictionary<long, object>();
        }
        public FilterParameters(IList<ProductAttribute> args)
            : this()
        {
            foreach (ProductAttribute pa in args)
                _parameters.Add(pa.Id, null);
        }
        private FilterParameters(FilterParameters value)
        {
            _page = 1;
            _brand = value._brand;
            _orderBy = value._orderBy;
            _parameters = new SortedDictionary<long, object>(value._parameters);
        }

        public static FilterParameters Create(DataSource ds, int id)
        {
            if (id > 0)
                return new FilterParameters(ProductAttribute.GetAllByCategory(ds, id));
            return new FilterParameters();
        }

        public void Load(long page, Arguments args)
        {
            Page = page;
            if (args != null)
            {
                if (args.Count > 0)
                {
                    try { Brand = int.Parse(args[0]); }
                    catch (Exception) { }
                }
                if (args.Count > 1)
                {
                    try { OrderBy = int.Parse(args[1]); }
                    catch (Exception) { }
                }
                if (args.Count > 2)
                {
                    long[] keys = new long[_parameters.Keys.Count];
                    _parameters.Keys.CopyTo(keys, 0);
                    for (int i = 2; i < Math.Min(args.Count, _parameters.Count); ++i)
                        _parameters[keys[i - 2]] = args[i];
                }
            }
        }
        public FilterParameters LoadArguments(Arguments args)
        {
            if (args.Count > 0)
            {
                try { Page = long.Parse(args[0]); }
                catch (Exception) { }
            }
            if (args.Count > 1)
            {
                try { Brand = int.Parse(args[1]); }
                catch (Exception) { }
            }
            if (args.Count > 2)
            {
                try { OrderBy = int.Parse(args[2]); }
                catch (Exception) { }
            }
            if (args.Count > 3)
            {
                long[] keys = new long[_parameters.Keys.Count];
                _parameters.Keys.CopyTo(keys, 0);
                for (int i = 3; i < Math.Min(args.Count, _parameters.Count); ++i)
                    _parameters[keys[i - 3]] = args[i];
            }
            return this;
        }

        public FilterParameters CopyByPage(long page)
        {
            FilterParameters fp = new FilterParameters(this);
            fp.Page = page;
            return fp;
        }
        public FilterParameters CopyByBrand(int brand)
        {
            FilterParameters fp = new FilterParameters(this);
            fp.Brand = brand;
            return fp;
        }
        public FilterParameters CopyByOrderBy(int orderBy)
        {
            FilterParameters fp = new FilterParameters(this);
            fp.OrderBy = orderBy;
            return fp;
        }
        public FilterParameters Copy(long key, object value)
        {
            FilterParameters fp = new FilterParameters(this);
            fp[key] = value;
            return fp;
        }

        public long Page
        {
            get { return _page; }
            set { _page = Math.Max(1, value); }
        }
        public int Brand
        {
            get { return _brand; }
            set { _brand = Math.Max(0, value); }
        }
        public int OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = Math.Max(0, value); }
        }
        public int Count
        {
            get { return _parameters.Count; }
        }
        public ICollection<long> Keys
        {
            get { return _parameters.Keys; }
        }
        public object this[long key]
        {
            get
            {
                object value;
                if (_parameters.TryGetValue(key, out value))
                    return FormatValue(value).ToString();
                return null;
            }
            set
            {
                if (_parameters.ContainsKey(key))
                    _parameters[key] = value;
            }
        }
        public bool Is(long key, string value)
        {
            return string.Equals(this[key] as string, value);
        }

        private object FormatValue(object value)
        {
            if (value == null || (value is string && string.IsNullOrEmpty((string)value)))
                return '-';
            if (value is bool)
                return (bool)value ? 1 : 0;
            if (value.GetType().IsEnum)
                return (int)value;
            return value;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('/').Append(_page);
            sb.Append('/').Append(_brand);
            sb.Append('/').Append(_orderBy);
            foreach (long key in _parameters.Keys)
                sb.Append('/').Append(HttpUtility.UrlEncode(FormatValue(_parameters[key]).ToString()));
            return sb.ToString();
        }
    }

    public sealed class FilterData<T> where T : IDbReader
    {
        private FilterParameters _filter;
        private SplitPageData<T> _data;

        public FilterData(FilterParameters filter, SplitPageData<T> data)
        {
            _filter = filter;
            _data = data;
        }

        public FilterParameters Filter
        {
            get { return _filter; }
        }
        public SplitPageData<T> Data
        {
            get { return _data; }
        }
    }
}
