using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaoTaoShopping.Filter
{
    //管理员过滤器
    public class AdminAuthen : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //1、获取用户登录信息 
            string user = filterContext.HttpContext.Session["id"]?.ToString();

            //判断用户信息
            if (user == null)
            {
                filterContext.Result = new RedirectResult("/Login/Index");
            }

        }
    }
}