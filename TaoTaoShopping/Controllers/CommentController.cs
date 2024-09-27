using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaoTaoShopping.Filter;
using TaoTaoShopping.Models;

namespace TaoTaoShopping.Controllers
{
    public class CommentController : Controller
    {
        private TaoTaoProjectDBEntities db = new TaoTaoProjectDBEntities();

        // 评论列表
        [AdminAuthen]
        public ActionResult Index(string keyword = "")
        {
            var comment = db.comment.Include(c => c.shopping).Include(c => c.user);
            return View(comment.Where(p => p.detail.Contains(keyword)).ToList());
        }

        // 用户的评论列表
        [UserAuthen]
        public ActionResult Index2(string keyword = "")
        {
            int id = 0;
            if (Session["user_id"] != null)
            {
                id = int.Parse(Session["user_id"].ToString());
            }
            var comment = db.comment.Include(c => c.shopping).Include(c => c.user);
            return View(comment.Where(p => p.detail.Contains(keyword) && p.uid == id).ToList());
        }

        // 保存添加评论
        [HttpPost]
        public ActionResult Create([Bind(Include = "id,detail,uid,shop_id,createtime")] comment comment)
        {
            db.comment.Add(comment);
            if (db.SaveChanges() > 0)
            {
                return Content("200");
            }
            else
            {
                return Content("201");
            }
        }

        // 删除评论
        public ActionResult Delete(int id)
        {
            comment comment = db.comment.Find(id);
            db.comment.Remove(comment);
            db.SaveChanges();
            return Content("<script>alert('删除成功！');window.history.back(-1);</script>");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
