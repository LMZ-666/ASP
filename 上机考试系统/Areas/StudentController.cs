using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace 上机考试系统.Areas.student.Controllers
{
    public class StudentController : Controller
    {
        // GET: student/Student
        public ActionResult StudentIndex(String studentName)
        {
            ViewBag.studentName = studentName;
            return View();
        }
    }
}