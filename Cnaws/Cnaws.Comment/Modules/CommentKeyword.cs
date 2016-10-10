using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Comment.Modules
{
    [Serializable]
    public sealed class CommentKeyword : NoIdentityModule
    {
        public long Id = 0L;
        [DataColumn(16)]
        public string Keyword = null;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Id");
            DropIndex(ds, "Keyword");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Id", "Id");
            CreateIndex(ds, "Keyword", "Keyword");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (Id <= 0L)
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Keyword))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static IList<CommentKeyword> GetAllById(DataSource ds, long id)
        {
            return Db<CommentKeyword>.Query(ds)
                .Select()
                .Where(W("Id", id))
                .ToList<CommentKeyword>();
        }
        public static IList<CommentKeyword> GetAllByIds(DataSource ds, long[] id)
        {
            return Db<CommentKeyword>.Query(ds)
                .Select()
                .Where(W("Id", id, DbWhereType.In))
                .ToList<CommentKeyword>();
        }

        public static IList<CommentKeyword> GetHotKeyWord(DataSource ds)
        {
            return Db<CommentKeyword>.Query(ds)
                .Select("Keyword", S_COUNT("Id"))
                .GroupBy(G("Keyword"))
                .OrderBy(D("Id"))
                .ToList<CommentKeyword>(15, 1);
        }

    }
}
