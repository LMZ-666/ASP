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
        private DatabaseEntities db = new DatabaseEntities();
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
        public ActionResult LoginStudent(student Student)
        {
            //DatabaseEntities db = new DatabaseEntities();

            //得到从数据库中查询到的表对象
            student stu = db.student.Find(Student.Id);

            //根据查到的表对象进行判断(若为空，说明Id输入错误)
            if (stu != null)
                if(stu.pwd == Student.pwd)
                    return RedirectToAction("Index");
                else
                    return Content("<script >alert('账号或密码错误');window.open('" + Url.Content("/Home/Login") + "', '_self')</script >", "text/html");
            else
                return Content("<script >alert('账号或密码错误');window.open('" + Url.Content("/Home/Login") + "', '_self')</script >", "text/html");
        }

        [HttpPost]
        public ActionResult LoginTeacher(teacher Teacher)
        {
            //DatabaseEntities db = new DatabaseEntities();

            //得到从数据库中查询到的表对象
            teacher tch = db.teacher.Find(Teacher.Id);

            //根据查到的表对象进行判断(若为空，说明Id输入错误)
            if (tch != null)
                if (tch.pwd == Teacher.pwd)
                    return RedirectToAction("Initial", "Teacher", new { area = "Teacher", teacherName = tch.name, teacherId = tch.Id });
                else
                    return Content("<script >alert('账号或密码错误');window.open('" + Url.Content("/Home/Login") + "', '_self')</script >", "text/html");
            else
                return Content("<script >alert('账号或密码错误');window.open('" + Url.Content("/Home/Login") + "', '_self')</script >", "text/html");
        }

    }
}