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
            DatabaseEntities db = new DatabaseEntities();

            //得到从数据库中查询到的表对象
            student stu = db.student.Find(Student.Id);

            //根据查到的表对象进行判断(若为空，说明Id输入错误)
            if (stu != null & stu.pwd == Student.pwd)
                return RedirectToAction("Index");
            else
                return View();

        }
    }
}