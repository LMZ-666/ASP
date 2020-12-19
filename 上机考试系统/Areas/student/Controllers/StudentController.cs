using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 上机考试系统.Models.DatabaseModel;

namespace 上机考试系统.Areas.student.Controllers
{
    
    public class StudentController : Controller
    {
        DatabaseEntities db = new DatabaseEntities();
        static int MyTestId;
        static String MyStudentName;
        // GET: student/Student

        public ActionResult InitStudent(String studentName,int TestId=0)
        {
            
            MyTestId = TestId;
            MyStudentName = studentName;
            return RedirectToAction("StudentIndex");
        }
        public ActionResult StudentIndex()
        {

            ViewBag.studentName = MyStudentName;
            return View();
        }
        public ActionResult StartTest()
        {

            Exam EX = db.Exam.Find(MyTestId);

            String str;
            if(EX!=null)
            {
                str = String.Format("本场考试的科目名称为：{0}，考试时间为：{1}，监考教师是：{2}", EX.name, EX.time, EX.creator);
            }
            else
            {
                str = "没有要进行的考试";
            }
            ViewBag.Message = str;
            return View(EX);
        }
    }
}