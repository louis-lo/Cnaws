using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Templates;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public sealed class FreightTemplate : LongIdentityModule
    {
        /// <summary>
        /// 计价方式
        /// </summary>
        public enum ValuationType
        {
            /// <summary>
            /// 按件计价
            /// </summary>
            ThePrece=0,
            /// <summary>
            /// 按重量计价
            /// </summary>
            Weight,
            /// <summary>
            /// 按体积计价
            /// </summary>
            Volume
        }

        /// <summary>
        /// 卖家Id
        /// </summary>
        public long SellerId = 0L;
        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name = null;
        /// <summary>
        /// 发货省
        /// </summary>
        public int Province = 0;
        /// <summary>
        /// 发货市
        /// </summary>
        public int City = 0;
        /// <summary>
        /// 发货区
        /// </summary>
        public int County = 0;
        /// <summary>
        /// 计价方式
        /// </summary>
        public ValuationType Type = ValuationType.ThePrece;
        /// <summary>
        /// 快递公司
        /// </summary>
        [DataColumn(64)]
        public string LogisticsCompany = null;
        /// <summary>
        /// 默认数量(重量和体积都*100)
        /// </summary>
        public int Number = 0;
        /// <summary>
        /// 默认价格
        /// </summary>
        public Money Money = 0;
        /// <summary>
        /// 默认增加数量(重量和体积都*100)
        /// </summary>
        public int StepNumber = 0;
        /// <summary>
        /// 默认增加价格
        /// </summary>
        public Money StepMoney = 0;
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime EditTime= (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);



        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "SellerId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "SellerId", "SellerId");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "SellerId");
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
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
                foreach (FreightMapping map in FreightMapping.GetAllByTemplate(ds, Id))
                {
                    if (map.Delete(ds) != DataStatus.Success)
                    {
                        ds.Rollback();
                        return DataStatus.Rollback;
                    }
                }
            }
            catch (Exception) { ds.Rollback();  return DataStatus.Rollback; }
            ds.Commit();
            return DataStatus.Success;
        }
        protected override void OnDeleteFailed(DataSource ds)
        {
            ds.Rollback();
        }

        public static Money GetFreight(DataSource ds,long id,int provice, int city,int county,Money Total,int Count,int Volume,int Weight)
        {
            FreightTemplate tmp = GetById(ds, id);
            if (tmp != null)
            {
                if (tmp.Number <= 0) tmp.Number = 1;
                FreightMapping map = FreightMapping.GetMapping(ds, tmp.Id, provice, city);
                if (map != null)
                {
                    if (map.Number <= 0) map.Number = 1;
                    if (tmp.Type == ValuationType.ThePrece)
                    {
                        if (Count > map.Number && map.StepNumber > 0 && map.StepMoney > 0)
                        {
                            if ((Count - map.Number) % map.StepNumber == 0)
                                return (Count - map.Number) / map.StepNumber * map.StepMoney+ map.Money;
                            else
                                return (((Count - map.Number) / map.StepNumber) + 1) * map.StepMoney + map.Money;
                        }
                        else
                            return map.Money;
                    }
                    else if (tmp.Type == ValuationType.Volume)
                    {
                        int FVolumn = Count * Volume;
                        if (FVolumn > map.Number && map.StepNumber > 0 && map.StepMoney > 0)
                        {
                            if ((FVolumn - map.Number) % map.StepNumber == 0)
                                return (FVolumn - map.Number) / map.StepNumber * map.StepMoney + map.Money;
                            else
                                return (((FVolumn - map.Number) / map.StepNumber) + 1) * map.StepMoney + map.Money;
                        }
                        else
                            return map.Money;
                    }
                    else if (tmp.Type == ValuationType.Weight)
                    {
                        int FVolumn = Count * Weight;
                        if (FVolumn > map.Number && map.StepNumber > 0 && map.StepMoney > 0)
                        {
                            if ((FVolumn - map.Number) % map.StepNumber == 0)
                                return (FVolumn - map.Number) / map.StepNumber * map.StepMoney + map.Money;
                            else
                                return (((FVolumn - map.Number) / map.StepNumber) + 1) * map.StepMoney + map.Money;
                        }
                        else
                            return map.Money;
                    }
                }
                else
                {
                    if (tmp.Type == ValuationType.ThePrece)
                    {
                        if (Count > tmp.Number && tmp.StepNumber > 0 && map.StepMoney > 0)
                        {
                            if ((Count - tmp.Number) % tmp.StepNumber == 0)
                                return (Count - tmp.Number) / tmp.StepNumber * tmp.StepMoney + tmp.Money;
                            else
                                return (((Count - tmp.Number) / tmp.StepNumber) + 1) * tmp.StepMoney + tmp.Money;
                        }
                        else
                            return tmp.Money;
                    }
                    else if (tmp.Type == ValuationType.Volume)
                    {
                        int FVolumn = Count * Volume;
                        if (FVolumn > tmp.Number && tmp.StepNumber > 0 && map.StepMoney > 0)
                        {
                            if ((FVolumn - tmp.Number) % tmp.StepNumber == 0)
                                return (FVolumn - tmp.Number) / tmp.StepNumber * tmp.StepMoney + tmp.Money;
                            else
                                return (((FVolumn - tmp.Number) / tmp.StepNumber) + 1) * tmp.StepMoney + tmp.Money;
                        }
                        else
                            return tmp.Money;
                    }
                    else if (tmp.Type == ValuationType.Weight)
                    {
                        int FVolumn = Count * Weight;
                        if (FVolumn > tmp.Number && tmp.StepNumber > 0 && map.StepMoney > 0)
                        {
                            if ((FVolumn - tmp.Number) % tmp.StepNumber == 0)
                                return (FVolumn - tmp.Number) / tmp.StepNumber * tmp.StepMoney + tmp.Money;
                            else
                                return (((FVolumn - tmp.Number) / tmp.StepNumber) + 1) * tmp.StepMoney + tmp.Money;
                        }
                        else
                            return tmp.Money;
                    }
                }
            }
            return 0;
        }

        public IList<FreightMapping> GetMapping(DataSource ds)
        {
            return FreightMapping.GetAllByTemplate(ds, Id);
        }

        public static FreightTemplate GetById(DataSource ds, long id)
        {
            return ExecuteSingleRow<FreightTemplate>(ds, P("Id", id));
        }

        public DataStatus ModByIdAndUserId(DataSource ds)
        {
            return Update(ds, ColumnMode.Exclude, new DataColumn[2] {"Id", "SellerId" }, P("Id", Id) & P("SellerId", SellerId));
        }

        public static IList<FreightTemplate> GetAllBySeller(DataSource ds, long sellerId)
        {
            return ExecuteReader<FreightTemplate>(ds, P("SellerId", sellerId));
        }
    }
}
