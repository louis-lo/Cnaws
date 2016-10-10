using System;
using Cnaws.Web;
using Cnaws.Web.Templates;
using M = Cnaws.Article.Modules;

namespace Cnaws.Article.Controllers
{
    public sealed class Article : DataController
    {
        public void Info(int id)
        {
            M.Article article = M.Article.GetById(DataSource, id);
            M.ArticleCategory cate = M.ArticleCategory.GetById(DataSource, article.CategoryId);
            this["Article"] = article;
            this["Category"] = cate;
            Render("article.html");
        }

        public void List(int category, int page = 1)
        {
            this["Category"] = M.ArticleCategory.GetById(DataSource, category);
            this["ArticleList"] = M.Article.GetPage(DataSource, category, page, 10, 8);
            Render("article_list.html");
        }
    }
}
