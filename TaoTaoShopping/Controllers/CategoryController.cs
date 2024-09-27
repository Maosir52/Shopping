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
    public class CategoryController : Controller
    {
        private TaoTaoProjectDBEntities db = new TaoTaoProjectDBEntities();

        // 分类首页
        public ActionResult Index(string keyword="")
        {
            return View(db.category.Where(p => p.catename.Contains(keyword)).ToList());
        }

        // 添加分类
        public ActionResult Create()
        {
            return View();
        }

        // 保存添加分类
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,catename")] category category)
        {
            if (ModelState.IsValid)
            {
                db.category.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // 编辑分类
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category category = db.category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // 保存编辑分类
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,catename")] category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        //删除分类
        public ActionResult Delete(int id)
        {
            category category = db.category.Find(id);
            db.category.Remove(category);
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
