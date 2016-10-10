using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;

namespace Cnaws.Product.Modules
{
    public enum FreightType
    {
        /// <summary>
        /// 固定价格
        /// </summary>
        Fix = 0,
        /// <summary>
        /// 运费模板
        /// </summary>
        Template = 1
    }

    [Serializable]
    public abstract class ProductBase<T> : LongIdentityModule where T : Module
    {
        public const char ImageSplitChar = '|';
         
        /// <summary>
        /// 供应商Id
        /// </summary>
        public long SupplierId = 0L;
        /// <summary>
        /// 品牌Id
        /// </summary>
        public int BrandId = 0;
        /// <summary>
        /// 分类Id
        /// </summary>
        public int CategoryId = 0;
        /// <summary>
        /// 标题
        /// </summary>
        [DataColumn(128)]
        public string Title = null;
        /// <summary>
        /// 图片，以“|”分割
        /// </summary>
        [DataColumn(256)]
        public string Image = null;
        /// <summary>
        /// 正文
        /// </summary>
        public string Content = null;
        /// <summary>
        /// 条形码
        /// </summary>
        [DataColumn(32)]
        public string BarCode = null;
        /// <summary>
        /// 单位
        /// </summary>
        [DataColumn(8)]
        public string Unit = null;
        /// <summary>
        /// 库存
        /// </summary>
        public int Count = 0;
        /// <summary>
        /// 单价
        /// </summary>
        public Money Price = 0;
        /// <summary>
        /// 库存预警
        /// </summary>
        public int InventoryAlert = 0;
        /// <summary>
        /// 省
        /// </summary>
        public int ProvinceId = 0;
        /// <summary>
        /// 市
        /// </summary>
        public int CityId = 0;
        /// <summary>
        /// 运费类型
        /// </summary>
        public FreightType FreightType = FreightType.Fix;
        /// <summary>
        /// 固定运费价格
        /// </summary>
        public Money FreightMoney = 0;
        /// <summary>
        /// 运费模板Id
        /// </summary>
        public int FreightTemplate = 0;
        /// <summary>
        /// 是否开增值税发票
        /// </summary>
        public bool HasReceipt = false;
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool Approved = false;
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel = false;

        public string[] GetImages()
        {
            if (Image != null)
                return Image.Split(ImageSplitChar);
            return new string[] { };
        }

        public static long GetCountByCategoryId(DataSource ds, int categoryId)
        {
            return ExecuteCount<T>(ds, P("CategoryId", categoryId));
        }
        public static long GetCountByBrandId(DataSource ds, int brandId)
        {
            return ExecuteCount<T>(ds, P("BrandId", brandId));
        }
    }
}
