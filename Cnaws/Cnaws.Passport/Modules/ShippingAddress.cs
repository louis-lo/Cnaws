using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Area;

namespace Cnaws.Passport.Modules
{
    [Serializable]
    public class ShippingAddress : LongIdentityModule
    {
        public long UserId = 0L;
        /// <summary>
        /// 收货人
        /// </summary>
        [DataColumn(16)]
        public string Consignee = null;
        public long Mobile = 0L;
       // public string Email = null;
        public int PostId = 0;
        public int Province = 0;
        public int City = 0;
        public int County = 0;
        [DataColumn(128)]
        public string Address = null;
        public bool IsDefault = false;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "UserId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "UserId", "UserId");
        }

        public string BuildInfo()
        {
            return JsonValue.Serialize(new AddressCacheInfo(this));
        }
        public string GetAddress()
        {
            using (Country c = Country.GetCountry())
                return string.Concat(c.GetCity(Province).Name, c.GetCity(City).Name, c.GetCity(County).Name, Address);
        }

        public string GetArea()
        {
            using (Country c = Country.GetCountry())
                return string.Concat(c.GetCity(Province).Name, c.GetCity(City).Name, c.GetCity(County).Name);
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (UserId <= 0)
                return DataStatus.Failed;
            if (IsDefault)
                (new ShippingAddress() { UserId = long.MinValue, IsDefault = false }).Update(ds, ColumnMode.Include, Cs("IsDefault"), WN("IsDefault", true, "Value1") & WN("UserId", UserId, "Value2"));
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (UserId == long.MinValue)
                return DataStatus.Success;
            if (UserId <= 0)
                return DataStatus.Failed;
            if (IsDefault)
                (new ShippingAddress() { UserId = long.MinValue, IsDefault = false }).Update(ds, ColumnMode.Include, Cs("IsDefault"), WN("IsDefault", true, "Value1") & WN("UserId", UserId, "Value2"));
            return DataStatus.Success;
        }

        public DataStatus Modify(DataSource ds)
        {
            return Update(ds, ColumnMode.Exclude, Cs("UserId"), P("Id", Id));
        }

        public static ShippingAddress GetById(DataSource ds, long id, long userId)
        {
            return ExecuteSingleRow<ShippingAddress>(ds, P("UserId", userId) & P("Id", id));
        }
        public static IList<ShippingAddress> GetAll(DataSource ds, long userId)
        {
            return ExecuteReader<ShippingAddress>(ds, Os(Od("IsDefault"), Od("Id")), P("UserId", userId));
        }
        public static ShippingAddress GetDefault(DataSource ds, long userId)
        {
            return ExecuteSingleRow<ShippingAddress>(ds, Os(Od("IsDefault"), Od("Id")), P("UserId", userId)& P("IsDefault", true));
        }
        public static DataStatus Remove(DataSource ds, long id, long userId)
        {
            return (new ShippingAddress() { Id = id, UserId = userId }).Delete(ds, "UserId", "Id");
        }
        public static DataStatus SetDefault(DataSource ds, long id, long userId)
        {
            return (new ShippingAddress() { Id = id,IsDefault=true, UserId = userId }).Update(ds,ColumnMode.Include, "IsDefault");
        }
    }
}
