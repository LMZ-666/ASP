using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace 上机考试系统.Areas.Teacher.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher/Teacher
        public ActionResult TeacherIndex(String teacherName)
        {
            ViewBag.teacherName = teacherName;
            return View();
        }
    }
}