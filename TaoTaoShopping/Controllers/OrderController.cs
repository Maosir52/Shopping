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

    public class OrderController : Controller
    {
        private TaoTaoProjectDBEntities db = new TaoTaoProjectDBEntities();

        // 订单管理的部分
        [AdminAuthen]
        public ActionResult Index(string keyword="")
        {
            var order = db.order.Include(o => o.user).Where(p => p.user.nickname.Contains(keyword));
            return View(order.OrderByDescending(p=>p.id).ToList());
        }

        //我的订单
        [UserAuthen]
        public ActionResult Index2(string keyword="")
        {
            int id = 0;
            if (Session["user_id"] != null)
            {
                id = int.Parse(Session["user_id"].ToString());
            }
            var order = db.order.Include(o => o.user).Where(p=>p.uid == id && p.order_num.Contains(keyword));
            return View(order.OrderByDescending(p => p.id).ToList());
        }

        //修改订单状态
        public ActionResult Send(int order_id,short state)
        {
            var info = db.order.FirstOrDefault(p => p.id == order_id);
            info.state = state;
            db.Entry(info).State = EntityState.Modified;
            db.SaveChanges();
            return Content("<script>alert('操作成功！');window.history.back(-1);</script>");
        }

        // 订单详情
        [AdminAuthen]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.order = db.order.Find(id);
            //以及订单详情
            var list = db.order_detail.Where(p => p.order_id == id).ToList();
            return View(list);
        }

        //用户的订单详情
        [UserAuthen]
        public ActionResult Details2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.order = db.order.Find(id);
            //以及订单详情
            var list = db.order_detail.Where(p => p.order_id == id).ToList();
            return View(list);
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
