using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 上机考试系统.Models.DatabaseModel;

namespace 上机考试系统.Areas.Teacher.Controllers
{
    public class TeacherController : Controller
    {
        public string TEACHER;
        private DatabaseEntities db = new DatabaseEntities();
        // GET: Teacher/Teacher
        public ActionResult TeacherIndex(String teacherName)
        {
            ViewBag.teacherName = teacherName;
            return View();
        }

        public ActionResult BeforeTest()
        {
            return View(db.Exam.ToList());
        }

        [HttpPost]
        public ActionResult BeforeTest(Exam exam)
        {
            exam.Id = 2;
            exam.creator = "";
            db.Exam.Add(exam);
            db.SaveChanges();
            return View(db.Exam.ToList());
        }
    }
}