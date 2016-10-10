using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Web.Templates;
using M = Cnaws.Comment.Modules;

namespace Cnaws.Comment.Controllers
{
    public class Comment : PassportController
    {
        [HttpGet]
        public virtual void List(int type, long id, int state = 0, int star1 = 0, int star2 = 0, long page = 1)
        {
            SplitPageData<M.Comment> data;
            switch (state)
            {
                case 1:
                    data = M.Comment.GetPageByTypeAndIdAndStar(DataSource, type, id, star1, star2, Math.Max(page, 1), 20, 8);
                    break;
                case 2:
                    data = M.Comment.GetPageByTypeAndIdAndImage(DataSource, type, id, Math.Max(page, 1), 20, 8);
                    break;
                default:
                    data = M.Comment.GetPageByTypeAndId(DataSource, type, id, Math.Max(page, 1), 20, 8);
                    break;
            }
            this["CommentList"] = data;
            this["GetPageUrl"] = new FuncHandler((args) =>
              {
                  return GetUrl("/comment/list/", type.ToString(), "/", id.ToString(), "/", state.ToString(), "/", star1.ToString(), "/", star2.ToString(), "/", Convert.ToInt64(args[0]).ToString());
              });
            Render("comment.html");
        }

        [HttpGet]
        [HttpAjax]
        public virtual void Keywords(int type, long id)
        {
            SetResult(true, M.Comment.GetAllKeywords(DataSource, type, id));
        }

        [HttpGet]
        [HttpAjax]
        public virtual void Count(int type, long id, int state, int star1, int star2)
        {
            long count;
            switch (state)
            {
                case 1:
                    count = M.Comment.GetCountByTypeAndIdAndStar(DataSource, type, id, star1, star2);
                    break;
                case 2:
                    count = M.Comment.GetCountByTypeAndIdAndImage(DataSource, type, id);
                    break;
                default:
                    count = M.Comment.GetCountByTypeAndId(DataSource, type, id);
                    break;
            }
            SetResult(true, count);
        }

        [HttpPost]
        [HttpAjax]
        [Authorize]
        public virtual void Submit()
        {
            try
            {
                M.Comment value = DbTable.Load<M.Comment>(Request.Form);
                value.UserId = User.Identity.Id;
                value.CreationDate = DateTime.Now;
                value.Ip = ClientIp;
                SetResult(value.Insert(DataSource));
            }
            catch (Exception)
            {
                SetResult(false);
            }
        }
    }
}
