using System;
using Cnaws.Management;

namespace Cnaws.Article.Management
{
    public sealed class RightList : ManagementRights
    {
        protected override void InitRights()
        {
            AddRight("文章分类", "management.articlecategory");
            AddRight("文章管理", "management.article");
        }
    }
}
