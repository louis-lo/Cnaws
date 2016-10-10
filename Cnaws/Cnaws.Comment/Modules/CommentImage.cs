using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Comment.Modules
{
    [Serializable]
    public sealed class CommentImage : NoIdentityModule
    {
        public long Id = 0L;
        [DataColumn(256)]
        public string Image = null;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Id");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Id", "Id");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (Id <= 0L)
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Image))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static IList<CommentImage> GetAllById(DataSource ds, long id)
        {
            return Db<CommentImage>.Query(ds)
                .Select()
                .Where(W("Id", id))
                .ToList<CommentImage>();
        }
        public static IList<CommentImage> GetAllByIds(DataSource ds, long[] id)
        {
            return Db<CommentImage>.Query(ds)
                .Select()
                .Where(W("Id", id, DbWhereType.In))
                .ToList<CommentImage>();
        }
    }
}
