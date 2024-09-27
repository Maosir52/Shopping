using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaoTaoShopping.Controllers
{
    public class UploadController : Controller
    {
        // 上传方法,用于异步上传功能的实现
        [HttpPost]
        public ActionResult file(HttpPostedFileBase pic)
        {
            try
            {
                if (pic != null)
                {
                    if (pic.ContentLength == 0)
                    {
                        return Content("209"); //获取上传的图片
                    }
                    else
                    {
                        //判断文件的后缀名，是否符合条件
                        string backFix = Path.GetExtension(pic.FileName);
                        if (backFix != ".gif" && backFix != ".png" && backFix != ".jpg" && backFix != ".jpeg")
                        {
                            return Content("210");
                        }
                        string fileName = DateTime.Now.ToString("MMddHHmmss") + backFix;
                        string strPath = Server.MapPath("~/Content/pic/" + fileName);
                        pic.SaveAs(strPath);
                        //返回路径
                        return Content("/Content/pic/" + fileName);
                    }
                }
                else
                {
                    return Content("300"); //图片不能为空
                }
            }
            catch (Exception ex)
            {
                return Content("400"); //上传失败
            }
        }
    }
}