using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public sealed class FreightAreaMapping : NoIdentityModule
    {
        public long TemplateId = 0L;
        [DataColumn(true)]
        public long MappingId = 0L;
        [DataColumn(true)]
        public int ProvinceId = 0;
        [DataColumn(true)]
        public int CityId = 0;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "MappingId");
            DropIndex(ds, "TemplateIdProvinceIdCityId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "MappingId", "MappingId");
            CreateIndex(ds, "TemplateIdProvinceIdCityId", "TemplateId", "ProvinceId", "CityId");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (TemplateId <= 0)
                return DataStatus.Failed;
            if (MappingId <= 0)
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static long GetMapping(DataSource ds, long tempId, int provice, int city)
        {
            FreightAreaMapping area = ExecuteSingleRow<FreightAreaMapping>(ds, P("ProvinceId", provice) & P("CityId", city) & P("TemplateId", tempId));
            if (area == null)
                area = ExecuteSingleRow<FreightAreaMapping>(ds, P("ProvinceId", provice) & P("CityId", 0) & P("TemplateId", tempId));
            if (area == null)
                area = ExecuteSingleRow<FreightAreaMapping>(ds, P("ProvinceId", 0) & P("CityId", 0) & P("TemplateId", tempId));
            if (area != null)
                return area.MappingId;
            return 0;
        }
        public static IList<FreightAreaMapping> GetAllByMapping(DataSource ds, long mappingId)
        {
            return ExecuteReader<FreightAreaMapping>(ds, P("MappingId", mappingId));
        }
        public static DataStatus DeleteByMapping(DataSource ds, long mappingId)
        {
            return (new FreightAreaMapping() { MappingId = mappingId }).Delete(ds, "MappingId");
        }
    }
}
