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
        public String TEACHER;
        public int TEACHERID;
        private DatabaseEntities db = new DatabaseEntities();
        // GET: Teacher/Teacher
        public ActionResult Initial(String teacherName, int teacherId)
        {
            TEACHER = teacherName;
            TEACHERID = teacherId;
            return RedirectToAction("TeacherIndex"); 
        }

        public ActionResult TeacherIndex()
        {
            ViewBag.teacherName = TEACHER;
            return View();
        }

        public ActionResult BeforeTest()
        {
            return View(db.Exam.ToList());
        }

        [HttpPost]
        public ActionResult BeforeTest(Exam exam)
        {
            exam.Id = 3;
            exam.creator = "黄亚博";
            exam.creatorId = 1;
            db.Exam.Add(exam);
            db.SaveChanges();
            return View(db.Exam.ToList());
        }
    }
}