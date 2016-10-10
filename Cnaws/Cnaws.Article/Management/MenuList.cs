using System;
using Cnaws.Management;

namespace Cnaws.Article.Management
{
    public sealed class MenuList : ManagementMenus
    {
        protected override void InitMenus()
        {
            AddMenu(1, 0, "文章管理")
                .AddSubMenu("文章分类", "/articlecategory")
                .AddSubMenu("文章列表", "/article");
        }
    }
}
