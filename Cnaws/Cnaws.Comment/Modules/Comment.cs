using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Comment.Modules
{
    [Serializable]
    public class Comment : LongIdentityModule
    {
        public long UserId = 0L;
        public long ParentId = 0L;
        public int TargetType = 0;
        public long TargetId = 0L;
        [DataColumn(36)]
        public string TargetData = null;
        public int Star = 0;
        [DataColumn(512)]
        public string Content = null;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        public int Channel = 0;
        [DataColumn(64)]
        public string Ip = null;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "UserIdParentId");
            DropIndex(ds, "UserIdTargetTypeParentId");
            DropIndex(ds, "TargetTypeTargetIdParentId");
            DropIndex(ds, "TargetTypeTargetIdParentIdStar");
            DropIndex(ds, "TargetTypeTargetIdTargetDataParentId");
            DropIndex(ds, "TargetTypeTargetIdTargetDataParentIdStar");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "UserIdParentId", "UserId", "ParentId");
            CreateIndex(ds, "UserIdTargetTypeParentId", "UserId", "TargetType", "ParentId");
            CreateIndex(ds, "TargetTypeTargetIdParentId", "TargetType", "TargetId", "ParentId");
            CreateIndex(ds, "TargetTypeTargetIdParentIdStar", "TargetType", "TargetId", "ParentId", "Star");
            CreateIndex(ds, "TargetTypeTargetIdTargetDataParentId", "TargetType", "TargetId", "TargetData", "ParentId");
            CreateIndex(ds, "TargetTypeTargetIdTargetDataParentIdStar", "TargetType", "TargetId", "TargetData", "ParentId", "Star");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (UserId <= 0L)
                return DataStatus.Failed;
            if (TargetId <= 0L)
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Content))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "UserId", "ParentId", "TargetType", "TargetId", "CreationDate");
            if (string.IsNullOrEmpty(Content))
                return DataStatus.Failed;
            return DataStatus.Success;
        }

        public IList<Comment> GetChildren(DataSource ds)
        {
            return Db<Comment>.Query(ds)
                .Select()
                .Where(W("ParentId", Id))
                .OrderBy(A("CreationDate"))
                .ToList<Comment>();
        }
        public IList<CommentKeyword> GetKeywords(DataSource ds)
        {
            return CommentKeyword.GetAllById(ds, Id);
        }
        public IList<CommentImage> GetImages(DataSource ds)
        {
            return CommentImage.GetAllById(ds, Id);
        }

        public static SplitPageData<Comment> GetPageByUser(DataSource ds, long userId, long index, int size, int show = 8)
        {
            long count;
            IList<Comment> list = Db<Comment>.Query(ds)
                .Select()
                .Where(W("ParentId", 0) & W("UserId", userId))
                .OrderBy(D("CreationDate"))
                .ToList<Comment>(size, index, out count);
            return new SplitPageData<Comment>(index, size, list, count, show);
        }
        public static SplitPageData<Comment> GetPageByUserAndType(DataSource ds, long userId, int targetType, long index, int size, int show = 8)
        {
            long count;
            IList<Comment> list = Db<Comment>.Query(ds)
                .Select()
                .Where(W("ParentId", 0) & W("TargetType", targetType) & W("UserId", userId))
                .OrderBy(D("CreationDate"))
                .ToList<Comment>(size, index, out count);
            return new SplitPageData<Comment>(index, size, list, count, show);
        }

        public static long GetCountByTypeAndId(DataSource ds, int targetType, long targetId)
        {
            return Db<Comment>.Query(ds)
                .Select()
                .Where(W("ParentId", 0) & W("TargetType", targetType) & W("TargetId", targetId))
                .Count();
        }
        public static SplitPageData<Comment> GetPageByTypeAndId(DataSource ds, int targetType, long targetId, long index, int size, int show = 8)
        {
            long count;
            IList<Comment> list = Db<Comment>.Query(ds)
                .Select()
                .Where(W("ParentId", 0) & W("TargetType", targetType) & W("TargetId", targetId))
                .OrderBy(D("CreationDate"))
                .ToList<Comment>(size, index, out count);
            return new SplitPageData<Comment>(index, size, list, count, show);
        }
        public static long GetCountByTypeAndIdAndStar(DataSource ds, int targetType, long targetId, int star1, int star2)
        {
            return Db<Comment>.Query(ds)
                .Select()
                .Where(W("Star", star1, DbWhereType.GreaterThanOrEqual) & W("Star", star2, DbWhereType.LessThan) & W("ParentId", 0) & W("TargetType", targetType) & W("TargetId", targetId))
                .Count();
        }
        public static SplitPageData<Comment> GetPageByTypeAndIdAndStar(DataSource ds, int targetType, long targetId, int star1, int star2, long index, int size, int show = 8)
        {
            long count;
            IList<Comment> list = Db<Comment>.Query(ds)
                .Select()
                .Where(W("Star", star1, DbWhereType.GreaterThanOrEqual) & W("Star", star2, DbWhereType.LessThan) & W("ParentId", 0) & W("TargetType", targetType) & W("TargetId", targetId))
                .OrderBy(D("CreationDate"))
                .ToList<Comment>(size, index, out count);
            return new SplitPageData<Comment>(index, size, list, count, show);
        }
        public static IList<CommentKeyword> GetAllKeywords(DataSource ds, int targetType, long targetId)
        {
            return Db<CommentKeyword>.Query(ds)
                .Select(S("Keyword"), S_COUNT("Id"))
                .Where(W("Id").InSelect<Comment>("Id").Where(W("ParentId", 0) & W("TargetType", targetType) & W("TargetId", targetId)).Result())
                .GroupBy("Keyword")
                .OrderBy(D("Id"))
                .ToList<CommentKeyword>();
        }
        public static long GetCountByTypeAndIdAndImage(DataSource ds, int targetType, long targetId)
        {
            return Db<Comment>.Query(ds)
                .Select(S<Comment>("*"))
                .RightJoin(O<Comment>("Id"), O<CommentImage>("Id"))
                .Where(W<Comment>("ParentId", 0) & W<Comment>("TargetType", targetType) & W<Comment>("TargetId", targetId))
                .Count();
        }
        public static SplitPageData<Comment> GetPageByTypeAndIdAndImage(DataSource ds, int targetType, long targetId, long index, int size, int show = 8)
        {
            long count;
            IList<Comment> list = Db<Comment>.Query(ds)
                .Select(S<Comment>("*"))
                .RightJoin(O<Comment>("Id"), O<CommentImage>("Id"))
                .Where(W<Comment>("ParentId", 0) & W<Comment>("TargetType", targetType) & W<Comment>("TargetId", targetId))
                .OrderBy(D<Comment>("CreationDate"))
                .ToList<Comment>(size, index, out count);
            return new SplitPageData<Comment>(index, size, list, count, show);
        }

        public static SplitPageData<Comment> ApiGetPagebyId(DataSource ds, int targetType, long targetId, int type,int star1, int star2, long index, int size, int show = 8)
        {
            long count = 0;
            IList<Comment> list = null;
            if (type == 2)///获取有图片的
            {
                list = Db<Comment>.Query(ds)
                .Select(S<Comment>("*"))
                .RightJoin(O<Comment>("Id"), O<CommentImage>("Id"))
                .Where(W<Comment>("ParentId", 0) & W<Comment>("TargetType", targetType) & W<Comment>("TargetId", targetId))
                .OrderBy(D<Comment>("CreationDate"))
                .ToList<Comment>(size, index, out count);
            }
            else if (type == 1)//根据星级区间获取
            {
                list = Db<Comment>.Query(ds)
                    .Select()
                    .Where(W("Star", star1, DbWhereType.GreaterThanOrEqual) & W("Star", star2, DbWhereType.LessThan) & W("ParentId", 0) & W("TargetType", targetType) & W("TargetId", targetId))
                    .OrderBy(D("CreationDate"))
                    .ToList<Comment>(size, index, out count);
            }
            else if (type == 0)//获取所有
            {
                list = Db<Comment>.Query(ds)
                .Select()
                .Where(W("ParentId", 0) & W("TargetType", targetType) & W("TargetId", targetId))
                .OrderBy(D("CreationDate"))
                .ToList<Comment>(size, index, out count); ;
            }
            return new SplitPageData<Comment>(index, size, list, count, show);

        }

        public static Comment GetByTargetIdAndTargetDate(DataSource ds,long targetId,string targetDate)
        {
            return Db<Comment>.Query(ds).Select().Where(W("TargetId", targetId) & W("TargetData", targetDate)).First<Comment>();
        }

    }
}
