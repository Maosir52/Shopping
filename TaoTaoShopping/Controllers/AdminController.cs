using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaoTaoShopping.Filter;

namespace TaoTaoShopping.Controllers
{
    [AdminAuthen]
    public class AdminController : Controller
    {
        // 后台页面的显示
        public ActionResult Index()
        {
            return View();
        }
    }
}