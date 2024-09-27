using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaoTaoShopping.Filter;
using TaoTaoShopping.Models;

namespace TaoTaoShopping.Controllers
{
    [UserAuthen]
    public class BackController : Controller
    {
        private TaoTaoProjectDBEntities db = new TaoTaoProjectDBEntities();

        // 实现用户修改的页面展示
        public ActionResult Index()
        {
            int id = 0;
            if(Session["user_id"] != null)
            {
                id = int.Parse(Session["user_id"].ToString());
            }
            if (id == null)
            {
                return Content("<script>alert('未找到用户！');window.history.back(-1);</script>");
            }
            user user = db.user.FirstOrDefault(p=>p.id == id);
            if (user == null)
            {
                return Content("<script>alert('用户未存在！');window.history.back(-1);</script>");
            }
            return View(user);
        }

        //保存修改
        [HttpPost]
        public ActionResult Index(user user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return Content("<script>alert('修改成功！');window.history.back(-1);</script>");
            }
            return View(user);
        }
    }
}