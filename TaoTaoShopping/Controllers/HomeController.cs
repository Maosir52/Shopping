using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaoTaoShopping.Filter;
using TaoTaoShopping.Models;

namespace TaoTaoShopping.Controllers
{
    public class HomeController : Controller
    {
        //引入数据库上下文对象
        private TaoTaoProjectDBEntities db = new TaoTaoProjectDBEntities();

        //商城的首页
        public ActionResult Index()
        {
            //查找最新的10条商品回来
            var list = db.shopping.OrderByDescending(p=>p.id).Take(12).ToList();
            return View(list);
        }

        //后台的个人中心功能
        [UserAuthen]
        public ActionResult BackIndex()
        {
            return View();
        }

        //商城的列表页
        public ActionResult List(int? cid,string keyword="",int page=1)
        {
            int pageSize = 16; //16个数据一页
            IEnumerable<shopping> list = null;
            //存在模糊搜索
            if (!string.IsNullOrEmpty(keyword))
            {
                 list = db.shopping.Where(p => p.title.Contains(keyword)).OrderByDescending(p => p.id).ToList();
            }
            else
            {
                if (cid == null)
                {
                    return Content("<script>alert('未找到商品！');window.history.back(-1);</script>");
                }
                list = db.shopping.Where(p => p.cid == cid).OrderByDescending(p => p.id).ToList();
            }
            //这里是实现分页的核心代码
            //（1）先获取当前表的总条数，才知道有多少页
            int recordCount = list.Count();
            //(2)分页取回来展示 skip的意思是：跳过前面的多少条，取10条的意思
            list = list.Skip((page - 1) * pageSize).Take(pageSize);
            //(3)获取当前可以分多少页的页码数回去
            ViewBag.pageNum = Math.Ceiling((Convert.ToDecimal(recordCount)) / (Convert.ToDecimal(pageSize)));
            return View(list);
        }

        //商城的详情页
        public ActionResult Detail(int? id)
        {
            if(id == null)
            {
                return Content("<script>alert('未找到商品！');window.history.back(-1);</script>");
            }
            shopping info = db.shopping.FirstOrDefault(p=>p.id == id);
            if (info == null)
            {
                return Content("<script>alert('未找到商品！');window.history.back(-1);</script>");
            }

            //获取评论列表展示
            ViewBag.list = db.comment.Where(p=>p.shop_id == id).ToList();
            return View(info);
        }

        //登录
        public ActionResult Login()
        {
            return View();
        }

        //注册
        public ActionResult Register()
        {
            return View();
        }

        //商品支付页面【商品详情页跳转过来的支付】
        [UserAuthen]
        public ActionResult Pay2(int shopId,int shopNum)
        {
            //购物车信息
            int id = 0;
            if (Session["user_id"] != null)
            {
                id = int.Parse(Session["user_id"].ToString());
            }
            if (id == 0)
            {
                return Content("<script>alert('请先登录，登录才能购买！');window.location.href='/Home/Login';</script>");
            }
            //查询购物车列表返回
            shopping s = db.shopping.FirstOrDefault(p => p.id == shopId);
            if (s.number < shopNum)
            {
                return Content("<script>alert('" + s.title + "商品库存不足，无法购买！');window.history.back(-1);</script>");
            }
            //商品数量
            ViewBag.shopNum = shopNum;
            //我的地址
            ViewBag.address = db.address.Where(p => p.uid == id).OrderByDescending(p => p.id).ToList();
            return View(s);
        }

        //执行支付结算【购物车过来的操作】
        [UserAuthen]
        [HttpPost]
        public ActionResult Pay2(int address_id, string payWay, string mark, int shopId, int shopNum)
        {
            int id = 0;
            if (Session["user_id"] != null)
            {
                id = int.Parse(Session["user_id"].ToString());
            }
            //订单详情
            shopping s = db.shopping.FirstOrDefault(p => p.id == shopId);
            var sum = s.sale_price * shopNum;
            //查询用户的地址回来
            address a = db.address.FirstOrDefault(p => p.id == address_id);
            order o = new order()
            {
                uid = id,
                order_num = "o" + DateTime.Now.ToString("yyMMddSShh"),
                sum_price = sum,
                mark = mark,
                createtime = DateTime.Now,
                is_pay = 1,//支付成功了
                state = 0,//默认未发货
                pay_way = payWay, //支付方式
                address_id = address_id, //订单id
                address = a.address1,
                phone = a.phone,
                name = a.name, //用货人的信息
            };
            db.order.Add(o);
            order_detail d = new order_detail()
            {
                order_id = o.id, //订单id
                count = shopNum,
                price = s.sale_price,
                sum_price = s.sale_price * shopNum,
                shop_id = s.id,
                title = s.title
            };

            //实现商品数量的扣减
            s.number = (int)s.number - (int)shopNum;
            db.Entry(s).State = EntityState.Modified;
            db.order_detail.Add(d); //添加订单详情

            if (db.SaveChanges() > 0)
            {
                return Content("200");
            }
            else
            {
                return Content("201");
            }
        }

        //商品支付页面【购物车跳转的支付】
        [UserAuthen]
        public ActionResult Pay()
        {
            //购物车信息
            int id = 0;
            if (Session["user_id"] != null)
            {
                id = int.Parse(Session["user_id"].ToString());
            }
            if (id == 0)
            {
                return Content("<script>alert('请先登录，登录才能查看购物车！');window.location.href='/Home/Login';</script>");
            }
            //查询购物车列表返回
            var list = db.shop_car.Include(p => p.user).Include(p => p.shopping).Where(p => p.uid == id).OrderByDescending(p => p.id).ToList();

            //查询购物车中全部的商品回来，判断数量是否足够扣减
            foreach(shop_car item in list)
            {
                shopping s = db.shopping.FirstOrDefault(p => p.id == item.shopid);
                if(s.number < item.shopNum)
                {
                    return Content("<script>alert('"+s.title+"商品库存不足，无法购买，请先从购物车中移除该商品！');window.history.back(-1);</script>");
                }
            }

            //我的地址
            ViewBag.address = db.address.Where(p => p.uid == id).OrderByDescending(p => p.id).ToList();
            return View(list);
        }

        //执行支付结算
        [UserAuthen]
        [HttpPost]
        public ActionResult Pay(int address_id,string payWay,string mark)
        {
            int id = 0;
            if (Session["user_id"] != null)
            {
                id = int.Parse(Session["user_id"].ToString());
            }
            //订单详情
            IEnumerable<shop_car> list = db.shop_car.Include(p => p.user).Include(p => p.shopping).Where(p => p.uid == id).OrderByDescending(p => p.id).ToList();
            var sum = db.shop_car.Where(p => p.uid == id).Sum(p=>p.shopping.sale_price);
            //查询用户的地址回来
            address a = db.address.FirstOrDefault(p=>p.id == address_id);
            order o = new order()
            {
                uid = id,
                order_num = "o" + DateTime.Now.ToString("yyMMddSShh"),
                sum_price = sum,
                mark = mark,
                createtime = DateTime.Now,
                is_pay = 1,//支付成功了
                state = 0,//默认未发货
                pay_way = payWay, //支付方式
                address_id=address_id, //订单id
                address = a.address1,
                phone = a.phone,
                name = a.name, //用货人的信息
            };
            db.order.Add(o);
            foreach(shop_car item in list)
            {
                order_detail d = new order_detail()
                {
                    order_id = o.id, //订单id
                    count = item.shopNum,
                    price = item.shopping.sale_price,
                    sum_price = item.shopping.sale_price * item.shopNum,
                    shop_id = item.shopid,
                    title = item.shopping.title
                };
               
                //实现商品数量的扣减
                shopping s = db.shopping.FirstOrDefault(p=>p.id == item.shopid);
                if(s != null)
                {
                    s.number = (int)s.number - (int)item.shopNum;
                    db.Entry(s).State = EntityState.Modified;
                }
                db.order_detail.Add(d);
                //清空购物车的信息
                db.shop_car.Remove(item);
            }
            
            if (db.SaveChanges() > 0)
            {
                return Content("200");
            }
            else
            {
                return Content("201");
            }
        }

        //我的购物车
        [UserAuthen]
        public ActionResult Car()
        {
            int id = 0;
            if (Session["user_id"] != null)
            {
                id = int.Parse(Session["user_id"].ToString());
            }
            if(id == 0)
            {
                return Content("<script>alert('请先登录，登录才能查看购物车！');window.location.href='/Home/Login';</script>");
            }
            //查询购物车列表返回
            var list = db.shop_car.Include(p => p.user).Include(p => p.shopping).Where(p=>p.uid == id).OrderByDescending(p=>p.id).ToList();
            return View(list);
        }
        //删除购物车中的商品
        [UserAuthen]
        [HttpPost]
        public ActionResult delCar(int shopId)
        {
            int id = 0;
            if (Session["user_id"] != null)
            {
                id = int.Parse(Session["user_id"].ToString());
            }
            shop_car info = db.shop_car.FirstOrDefault(p => p.shopid == shopId && p.uid == id);
            db.shop_car.Remove(info);
            if (db.SaveChanges() > 0)
            {
                return Content("200");
            }
            else
            {
                return Content("201");
            }
        }

        //实现添加购物车功能
        [UserAuthen]
        [HttpPost]
        public ActionResult addCar(int shopId,int shopNum)
        {
            int id = 0;
            if (Session["user_id"] != null)
            {
                id = int.Parse(Session["user_id"].ToString());
            }
            //先判断购物车是否存在该商品，存在则数量改变，不存在则添加该商品
            shop_car info = db.shop_car.FirstOrDefault(p=>p.shopid == shopId && p.uid == id);
            if(info != null)
            {
                info.shopNum += shopNum;
                db.Entry(info).State = EntityState.Modified;
            }
            else
            {
                shop_car c = new shop_car()
                {
                    shopid = shopId,
                    shopNum = shopNum,
                    createtime = DateTime.Now,
                    uid = id
                };
                db.shop_car.Add(c);
            }
            
            if(db.SaveChanges() > 0)
            {
                return Content("200");
            }
            else
            {
                return Content("201");
            }
        }

        //实现登录
        [HttpPost]
        public ActionResult Login(user u)
        {
            //用户名是否存在
            user info = db.user.FirstOrDefault(p => p.username == u.username);
            if (info != null)
            {
                if(info.pwd != u.pwd)
                {
                    return Content("<script>alert('用户名或密码错误！');window.history.back(-1);</script>");
                }
                else
                {
                    Session["nick"] = info.nickname;
                    Session["img"] = info.img;
                    Session["user_id"] = info.id;
                    return Redirect("/Home/Index");
                }                
            }
            else
            {
                return Content("<script>alert('用户名或密码错误！');window.history.back(-1);</script>");
            }            
        }

        //退出系统
        public ActionResult Logout()
        {
            Session["nick"] = null;
            Session["user_id"] = null;
            return Redirect("/Home/Index");
        }

        //实现注册
        [HttpPost]
        public ActionResult Register(user u)
        {
            //用户名是否存在
            user info = db.user.FirstOrDefault(p=>p.username == u.username);
            if(info != null)
            {
                return Content("<script>alert('用户名已存在！');window.history.back(-1);</script>");
            }
            u.img = "/assets/img/head.png";
            db.user.Add(u);
            db.SaveChanges();
            return Content("<script>alert('注册成功！');window.history.back(-1);</script>");
        }
    }
}