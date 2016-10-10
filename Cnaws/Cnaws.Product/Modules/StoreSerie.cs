using Cnaws.Data;
using Cnaws.Data.Query;
using Cnaws.Templates;
using Cnaws.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public class StoreSerie : LongIdentityModule
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public long UserId = 0L;
        /// <summary>
        /// 系列名称
        /// </summary>
        [DataColumn(32)]
        public string Name = null;
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault = false;
        /// <summary>
        /// 创健时间
        /// </summary>
        public DateTime CreationDate= (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "UserId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "UserId", "UserId");
        }

        public static IList<StoreSerie> GetByUser(DataSource ds, long userId)
        {
            return Db<StoreSerie>.Query(ds).Select().Where(W("UserId", userId)).ToList<StoreSerie>();
        }

        public static StoreSerie GetById(DataSource ds, long id)
        {
            return Db<StoreSerie>.Query(ds).Select().Where(W("Id", id)).First<StoreSerie>();
        }
        public IList<StoreAttribute> GetAttributes(DataSource ds)
        {
            return Db<StoreAttribute>.Query(ds).Select().Where(W("SerieId", Id)).ToList<StoreAttribute>();
        }
        public static DataStatus DelbyId(DataSource ds, long id)
        {
            ds.Begin();
            try
            {
                if (StoreAttribute.DelbySerieId(ds, id) != DataStatus.Success)
                {
                    throw new Exception();
                }
                if (new StoreSerie() { Id = id }.Delete(ds) == DataStatus.Success)
                {
                    ds.Commit();
                    return DataStatus.Success;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                ds.Rollback();
                return DataStatus.Failed;
            }
        }

        public DataStatus ModByIdAndUserId(DataSource ds)
        {
            return Update(ds, ColumnMode.Exclude, new DataColumn[2] { "Id", "UserId" }, P("Id", Id) & P("UserId", UserId));
        }
    }
}
