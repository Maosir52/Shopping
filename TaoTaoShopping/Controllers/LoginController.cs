using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaoTaoShopping.Models; //需要先引用模型类

namespace TaoTaoShopping.Controllers
{
    public class LoginController : Controller
    {
        //使用数据库上下文对象操作
        private TaoTaoProjectDBEntities db = new TaoTaoProjectDBEntities();
        // 登录
        public ActionResult Index()
        {
            return View();
        }

        //实现的是Post请求
        [HttpPost]
        public ActionResult Index(string username,string pwd)
        {
            //1、判断用户名和密码是否为空
            if (string.IsNullOrEmpty(username))
            {
                return Content("<script>alert('用户名不能为空！');window.history.back(-1);</script>");
            }
            if (string.IsNullOrEmpty(pwd))
            {
                return Content("<script>alert('密码不能为空！');window.history.back(-1);</script>");
            }
            admin info = db.admin.FirstOrDefault(p=>p.username == username && p.pwd == pwd);
            if(info == null)
            {
                return Content("<script>alert('用户名或密码错误！');window.history.back(-1);</script>");
            }
            else
            {
                //使用Session记住用户的关键信息
                Session["nickname"] = info.nickname;
                Session["id"] = info.id;
                return Redirect("/Admin/Index"); //代表登录成功
            }
            return View();
        }

        //退出系统
        public ActionResult Logout()
        {
            Session["nickname"] = null;
            Session["id"] = null;
            return Redirect("/Home/Index");
        }
    }
}