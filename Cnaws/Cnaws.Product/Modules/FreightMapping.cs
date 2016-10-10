using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public sealed class FreightMapping : LongIdentityModule
    {
        /// <summary>
        /// 模板Id
        /// </summary>
        public long TemplateId = 0L;
        /// <summary>
        /// 数量(重量和体积都*100)
        /// </summary>
        public int Number = 0;
        /// <summary>
        /// 价格
        /// </summary>
        public Money Money = 0;
        /// <summary>
        /// 增加数量(重量和体积都*100)
        /// </summary>
        public int StepNumber = 0;
        /// <summary>
        /// 增加价格
        /// </summary>
        public Money StepMoney = 0;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "TemplateId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "TemplateId", "TemplateId");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (TemplateId <= 0)
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "TemplateId");
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            ds.Begin();
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            try
            {
                if (FreightAreaMapping.DeleteByMapping(ds, Id) == DataStatus.Success)
                {
                    ds.Commit();
                    return DataStatus.Success;
                }
            }
            catch(Exception)
            { }
            ds.Rollback();
            return DataStatus.Rollback;
        }
        protected override void OnDeleteFailed(DataSource ds)
        {
            ds.Rollback();
        }

        public IList<FreightAreaMapping> GetMapping(DataSource ds)
        {
            return FreightAreaMapping.GetAllByMapping(ds, Id);
        }

        public DataStatus InsertOrUpdate(DataSource ds)
        {
            if (Id <= 0)
                return Insert(ds);
            else
                return Update(ds);
        }


        public static FreightMapping GetById(DataSource ds, long id)
        {
            return ExecuteSingleRow<FreightMapping>(ds, P("Id", id));
        }
        public static FreightMapping GetMapping(DataSource ds, long tempId, int provice, int city)
        {
            long id = FreightAreaMapping.GetMapping(ds, tempId, provice, city);
            if (id > 0)
                return GetById(ds, id);
            return null;
        }
        public static IList<FreightMapping> GetAllByTemplate(DataSource ds, long tempId)
        {
            return ExecuteReader<FreightMapping>(ds, P("TemplateId", tempId));
        }
        public static DataStatus DelById(DataSource ds,long mappingid)
        {
            return new FreightMapping() { Id = mappingid }.Delete(ds);
        }

    }
}
