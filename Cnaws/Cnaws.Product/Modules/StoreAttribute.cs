using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Data.Query;

namespace Cnaws.Product.Modules
{
    public class StoreAttribute : LongIdentityModule
    {
        public const char SplitChar = ',';
        /// <summary>
        /// 系列Id
        /// </summary>
        public long SerieId = 0;
        /// <summary>
        /// 规格名称
        /// </summary>
        public string SerieName = null;
        /// <summary>
        /// 规格对应的值用'|'隔开
        /// </summary>
        public string AttributorValue = null;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "SerieId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "SerieId", "SerieId");
        }

        public string[] GetAttributors()
        {
            if (AttributorValue != null)
                return AttributorValue.Split(SplitChar);
            return new string[] { };
        }
        public DataStatus InsertOrUpdate(DataSource ds)
        {
            if (Id <= 0)
                return Insert(ds);
            else
                return Update(ds);
        }
        public static IList<StoreAttribute> GetBySerieId(DataSource ds,long serieId)
        {
            return Db<StoreAttribute>.Query(ds).Select().Where(W("SerieId", serieId)).ToList<StoreAttribute>();
        }
        public static StoreAttribute GetById(DataSource ds, long id)
        {
            return Db<StoreAttribute>.Query(ds).Select().Where(W("Id", id)).First<StoreAttribute>();
        }
        public static bool Exists(DataSource ds, long id, long serieId, string seriename)
        {
            return Db<StoreAttribute>.Query(ds).Select().Where(W("Id", id, DbWhereType.NotEqual) & W("SerieId", serieId) & W("SerieName", seriename)).Count() > 0;
        }
        public static DataStatus DelbySerieId(DataSource ds, long serieId)
        {
            return new StoreAttribute() { SerieId = serieId }.Delete(ds, "SerieId");
        }

        public static DataStatus DelbyId(DataSource ds, long id)
        {
            return new StoreAttribute() { Id = id }.Delete(ds);
        }
    }
}
