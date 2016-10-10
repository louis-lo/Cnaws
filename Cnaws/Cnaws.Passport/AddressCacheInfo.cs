using System;
using Cnaws.Area;

namespace Cnaws.Passport
{
    [Serializable]
    public sealed class AddressCacheInfo
    {
        public string Consignee = null;
        public long Mobile = 0L;
        public int PostId = 0;
        public string Province = null;
        public string City = null;
        public string County = null;
        public string Address = null;

        public AddressCacheInfo()
        {

        }
        public AddressCacheInfo(Modules.ShippingAddress address)
        {
            Consignee = address.Consignee;
            Mobile = address.Mobile;
            PostId = address.PostId;
            using (Country c = Country.GetCountry())
            {
                Province = c.GetCity(address.Province).Name;
                City = c.GetCity(address.City).Name;
                County = c.GetCity(address.County).Name;
            }
            Address = address.Address;
        }
    }
}
