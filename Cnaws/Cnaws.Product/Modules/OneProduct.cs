using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Data.Query;
using System.Collections.Generic;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public sealed class OneProduct : IdentityModule
    {
        public const char ImageSplitChar = '|';

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
        public string Image = null;
        /// <summary>
        /// 正文
        /// </summary>
        public string Content = null;
        /// <summary>
        /// 关键字
        /// </summary>
        [DataColumn(2000)]
        public string Keywords = null;
        /// <summary>
        /// 说明
        /// </summary>
        [DataColumn(2000)]
        public string Description = null;
        /// <summary>
        /// 数量
        /// </summary>
        public int Count = 0;
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool Approved = false;

        public string[] GetImages()
        {
            if (Image != null)
                return Image.Split(ImageSplitChar);
            return new string[] { };
        }
        public string GetImage()
        {
            string[] imgs = GetImages();
            if (imgs.Length > 0)
                return imgs[0];
            return string.Empty;
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "CategoryId");
            DropIndex(ds, "Approved");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "CategoryId", "CategoryId");
            CreateIndex(ds, "Approved", "Approved");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Title))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Content))
                return DataStatus.Failed;
            return base.OnInsertBefor(ds, mode, ref columns);
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "Count");
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static long GetCountByCategoryId(DataSource ds, int categoryId)
        {
            return ExecuteCount<OneProduct>(ds, P("CategoryId", categoryId));
        }

        public static OneProduct GetById(DataSource ds, int id)
        {
            return Db<OneProduct>.Query(ds)
                .Select()
                .Where(W("Id", id))
                .First<OneProduct>();
        }
        public DataStatus UpdateApproved(DataSource ds)
        {
            DataStatus status;
            ds.Begin();
            try
            {
                status = Update(ds, ColumnMode.Include, "Approved");
                if (status == DataStatus.Success)
                {
                    if (Approved)
                    {
                        if (!OneProductNumber.IsActive(ds, Id))
                        {
                            if (OneProductNumber.Create(ds, Id) != DataStatus.Success)
                                throw new Exception();
                        }
                    }
                }
                ds.Commit();
            }
            catch (Exception)
            {
                ds.Rollback();
                status = DataStatus.Failed;
            }
            return status;
        }

        public static SplitPageData<OneProduct> GetPage(DataSource ds, int categoryId, long index, int size, int show = 8)
        {
            long count;
            IList<OneProduct> list;
            if (categoryId > 0)
                list = Db<OneProduct>.Query(ds)
                .Select()
                .Where(W("CategoryId", categoryId))
                .OrderBy(D("Id"))
                .ToList<OneProduct>(size, index, out count);
            else if (categoryId == 0)
                list = Db<OneProduct>.Query(ds)
                .Select()
                .OrderBy(D("Id"))
                .ToList<OneProduct>(size, index, out count);
            else if (categoryId == -1)
                list = Db<OneProduct>.Query(ds)
                .Select()
                .Where(W("Approved", false))
                .OrderBy(D("Id"))
                .ToList<OneProduct>(size, index, out count);
            else if (categoryId == -2)
                list = Db<OneProduct>.Query(ds)
                .Select()
                .Where(W("Approved", true))
                .OrderBy(D("Id"))
                .ToList<OneProduct>(size, index, out count);
            else
                throw new Exception();
            return new SplitPageData<OneProduct>(index, size, list, count, show);
        }

        public static IList<DataJoin<OneProduct, OneProductNumber>> GetAllActive(DataSource ds)
        {
            return Db<OneProduct>.Query(ds)
                .Select(S<OneProduct>(), S<OneProductNumber>())
                .InnerJoin(O<OneProduct>("Id"), O<OneProductNumber>("ProductId"))
                .Where(W<OneProductNumber>("State", OneProductNumberState.Normal))
                .ToList<DataJoin<OneProduct, OneProductNumber>>();
        }
    }
}
