using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 上机考试系统.Models.DatabaseModel;

namespace 上机考试系统.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(student Student)
        {
            Database db = new Database();

            //得到从数据库中查询到的表对象
            student stu = db.student.Find(id);

            //根据所写的sql语句，表中的行数应该是1，只要不为0，就说明查询到了该用户，重定向
            if (stu != null)
                return RedirectToAction("Index");
            else
                return View();

        }
    }
}