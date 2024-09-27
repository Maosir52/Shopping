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
    [AdminAuthen]
    public class ShoppingController : Controller
    {
        private TaoTaoProjectDBEntities db = new TaoTaoProjectDBEntities();

        // 商品管理
        public ActionResult Index(string keyword = "")
        {
            var shopping = db.shopping.Include(s => s.category);
            return View(shopping.Where(p => p.title.Contains(keyword)).OrderByDescending(p=>p.id).ToList());
        }

        // 商品详情
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            shopping shopping = db.shopping.Find(id);
            if (shopping == null)
            {
                return HttpNotFound();
            }
            return View(shopping);
        }

        // 添加商品
        public ActionResult Create()
        {
            ViewBag.cid = new SelectList(db.category, "id", "catename");
            return View();
        }

        // 保存商品
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "id,title,cid,price,sale_price,number,detail,img,shop_number")] shopping shopping)
        {
            if (ModelState.IsValid)
            {
                db.shopping.Add(shopping);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cid = new SelectList(db.category, "id", "catename", shopping.cid);
            return View(shopping);
        }

        // 编辑商品
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            shopping shopping = db.shopping.Find(id);
            if (shopping == null)
            {
                return HttpNotFound();
            }
            ViewBag.cid = new SelectList(db.category, "id", "catename", shopping.cid);
            return View(shopping);
        }

        // 保存编辑
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "id,title,cid,price,sale_price,number,detail,img,shop_number")] shopping shopping)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shopping).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cid = new SelectList(db.category, "id", "catename", shopping.cid);
            return View(shopping);
        }

        //删除商品
        public ActionResult Delete(int id)
        {
            shopping shopping = db.shopping.Find(id);
            db.shopping.Remove(shopping);
            db.SaveChanges();
            return RedirectToAction("Index");
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
