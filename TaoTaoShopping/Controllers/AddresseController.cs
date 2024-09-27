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
    [UserAuthen]
    public class AddresseController : Controller
    {
        private TaoTaoProjectDBEntities db = new TaoTaoProjectDBEntities();

        // 地址列表
        public ActionResult Index()
        {
            int id = 0;
            if (Session["user_id"] != null)
            {
                id = int.Parse(Session["user_id"].ToString());
            }
            var address = db.address.Include(a => a.user).Where(p=>p.uid == id);
            return View(address.ToList());
        }

        //添加地址
        public ActionResult Create()
        {
            ViewBag.uid = new SelectList(db.user, "id", "username");
            return View();
        }

        // 保存添加
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,address1,name,phone,mark,createtime,uid")] address address)
        {
            if (ModelState.IsValid)
            {
                db.address.Add(address);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.uid = new SelectList(db.user, "id", "username", address.uid);
            return View(address);
        }

        // 编辑
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            address address = db.address.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            ViewBag.uid = new SelectList(db.user, "id", "username", address.uid);
            return View(address);
        }

        // 保存编辑
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,address1,name,phone,mark,createtime,uid")] address address)
        {
            if (ModelState.IsValid)
            {
                db.Entry(address).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.uid = new SelectList(db.user, "id", "username", address.uid);
            return View(address);
        }

        //删除
        public ActionResult Delete(int id)
        {
            address address = db.address.Find(id);
            db.address.Remove(address);
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
