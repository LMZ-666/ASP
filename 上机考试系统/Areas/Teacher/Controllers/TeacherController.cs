using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Web;
using System.Web.Mvc;
using 上机考试系统.Models.DatabaseModel;

namespace 上机考试系统.Areas.Teacher.Controllers
{
    public class TeacherController : Controller
    {
        public static String TEACHER;
        public static int TEACHERID;
        public static int ExamId;
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

//------------------------------------------------------考前管理----------------------------------------------------------------

        public ActionResult BeforeTest()
        {
            return View(db.Exam.ToList());
        }

        [HttpPost]
        public ActionResult BeforeTest(Exam exam)
        {
            Exam exam1 = new Exam();
            exam1.Id = 0;
            foreach (var item in db.Exam.ToList())
            {
                if(exam1.Id<item.Id)
                {
                    exam1.Id = item.Id;
                }
            }
            exam.Id = exam1.Id + 1;
            ExamId = exam.Id;
            exam.creator = TEACHER;
            exam.creatorId = TEACHERID;
            exam.has_cleaned = "否";
            exam.has_saved = "否";
            exam.has_stopped = "否";
            exam.is_being = "否";
            exam.test_upload = "试卷未上传";
            exam.commmit_number = 0;
            db.Exam.Add(exam);
            db.SaveChanges();

            return View(db.Exam.ToList());
        }

        public ActionResult ExamEdit(int exam_Id)
        {
            ViewBag.exam_Id = exam_Id;
            Exam exam = db.Exam.Find(exam_Id);
            var exams = db.Exam.ToList();
            int has_exambeing = 0;
            foreach (var item in exams)
            {
                if(item.is_being == "是")
                {
                    has_exambeing = 1;
                }
            }
            ViewBag.has_exambeing = has_exambeing;
            var g = from t in db.student
                    where t.exam_Id == exam_Id
                    select t;
            var p = g.ToList();
            ViewBag.stuNum = p.Count();
            return View();
        }

        [HttpPost]
        public ActionResult ExamEdit_Mod(Exam exam)
        {
            Exam exam1 = db.Exam.Find(exam.Id);
            exam.Id = exam1.Id;
            exam.creator = exam1.creator;
            exam.creatorId = exam1.creatorId;
            exam.has_cleaned = exam1.has_cleaned;
            exam.has_saved = exam1.has_saved;
            exam.has_stopped = exam1.has_stopped;
            exam.is_being = exam1.is_being;
            exam.test_upload = exam1.test_upload;
            exam.commmit_number = exam1.commmit_number;
            exam.AnswerPath = exam1.AnswerPath;
            exam.PaperPath = exam1.PaperPath;
            db.Exam.Remove(exam1);
            db.SaveChanges();
            db.Exam.Add(exam);
            db.SaveChanges();
            return RedirectToAction("BeforeTest");
        }

        [HttpPost]
        public ActionResult ExamEdit_add(Student stu)
        {
            Exam exam = new Exam();
            foreach (var item in db.Exam.ToList())
            {
                if (item.is_being == "是")
                {
                    exam = item;
                    break;
                }
            }

            stu.pwd = "123456";
            db.student.Add(stu);
            db.SaveChanges();

            return RedirectToAction("ExamEdit", new { exam_Id=stu.exam_Id });
        }

        [HttpPost]
        public ActionResult ExamEdit_being(Exam exam)
        {
            Exam exam1 = db.Exam.Find(exam.Id);
            exam.Id = exam1.Id;
            exam.name = exam1.name;
            exam.time = exam1.time;
            exam.creator = exam1.creator;
            exam.creatorId = exam1.creatorId;
            exam.has_cleaned = exam1.has_cleaned;
            exam.has_saved = exam1.has_saved;
            exam.has_stopped = exam1.has_stopped;
            exam.is_being = "是";
            exam.test_upload = exam1.test_upload;
            exam.commmit_number = exam1.commmit_number;
            exam.PaperPath = exam1.PaperPath;
            exam.AnswerPath = exam1.AnswerPath;
            db.Exam.Remove(exam1);
            db.SaveChanges();
            db.Exam.Add(exam);
            db.SaveChanges();
            return RedirectToAction("BeforeTest");
        }

//------------------------------------------------------考中管理----------------------------------------------------------------

        public ActionResult TestCondition()
        {
            Exam exam = new Exam();
            foreach (var item in db.Exam.ToList())
            {
                if(item.is_being == "是")
                {
                    exam = item;
                    break;
                }
            }

            if (exam.is_being != "是")
                ViewBag.has_exambeing = 0;
            else
                ViewBag.has_exambeing = 1;

            ViewBag.examName = exam.name;
            var g = from t in db.student
                    where t.exam_Id == exam.Id
                    select t;
            var p = g.ToList();
            ViewBag.student_all = p.Count;

            g = from t in db.student
                    where t.ip_address != null && t.exam_Id == exam.Id
                    select t;
            p = g.ToList();
            ViewBag.student_login = p.Count;
            ViewBag.student_notlogin = ViewBag.student_all - ViewBag.student_login;
            ViewBag.commit_num = exam.commmit_number;
            ViewBag.notcommit_num = ViewBag.student_login - ViewBag.commit_num;
            return View(); 
        }

        public ActionResult StudentInfo()
        {
            Exam exam = new Exam();
            foreach (var item in db.Exam.ToList())
            {
                if (item.is_being == "是")
                {
                    exam = item;
                    break;
                }
            }

            if (exam.is_being != "是")
                ViewBag.has_exambeing = 0;
            else
                ViewBag.has_exambeing = 1;
            return View();
        }

        [HttpPost]
        public ActionResult StudentInfo(Student stu)
        {
            Exam exam = new Exam();
            foreach (var item in db.Exam.ToList())
            {
                if (item.is_being == "是")
                {
                    exam = item;
                    break;
                }
            }
            
            if (stu.Id != 0 && stu.name != null && stu.stuClass != 0)
            {
                var g = from t in db.student
                        where t.Id == stu.Id && t.exam_Id == exam.Id && t.name == stu.name && t.stuClass == stu.stuClass
                        select t;
                return View(g.ToList());
            }
            else if(stu.Id == 0 && stu.name != null && stu.stuClass != 0)
            {
                var g = from t in db.student
                        where t.exam_Id == exam.Id && t.name == stu.name && t.stuClass == stu.stuClass
                        select t;
                return View(g.ToList());
            }
            else if(stu.Id != 0 && stu.name == null && stu.stuClass != 0)
            {
                var g = from t in db.student
                        where t.Id == stu.Id && t.exam_Id == exam.Id && t.stuClass == stu.stuClass
                        select t;
                return View(g.ToList());
            }
            else if (stu.Id != 0 && stu.name != null && stu.stuClass == 0)
            {
                var g = from t in db.student
                        where t.Id == stu.Id && t.exam_Id == exam.Id
                        select t;
                return View(g.ToList());
            }
            else if (stu.Id == 0 && stu.name == null && stu.stuClass != 0)
            {
                var g = from t in db.student
                        where t.exam_Id == exam.Id && t.stuClass == stu.stuClass
                        select t;
                return View(g.ToList());
            }
            else if (stu.Id != 0 && stu.name == null && stu.stuClass == 0)
            {
                var g = from t in db.student
                        where t.Id == stu.Id && t.exam_Id == exam.Id
                        select t;
                return View(g.ToList());
            }
            else if (stu.Id == 0 && stu.name != null && stu.stuClass == 0)
            {
                var g = from t in db.student
                        where t.name == stu.name && t.exam_Id == exam.Id
                        select t;
                return View(g.ToList());
            }
            else
            {
                var g = from t in db.student
                        where t.exam_Id == exam.Id
                        select t;
                return View(g.ToList());
            }
        }

        [HttpPost]
        public ActionResult StudentInfo_add(Student stu)
        {
            Exam exam = new Exam();
            foreach (var item in db.Exam.ToList())
            {
                if (item.is_being == "是")
                {
                    exam = item;
                    break;
                }
            }

            stu.pwd = "123456";
            stu.exam_Id = exam.Id;
            db.student.Add(stu);
            db.SaveChanges();

            return Content("<script >alert('添加学生成功');window.open('" + Url.Content("StudentInfo") + "', '_self')</script >", "text/html");
        }

        public ActionResult RemoveBinding()
        {
            Exam exam = new Exam();
            foreach (var item in db.Exam.ToList())
            {
                if (item.is_being == "是")
                {
                    exam = item;
                    break;
                }
            }

            if (exam.is_being != "是")
                ViewBag.has_exambeing = 0;
            else
                ViewBag.has_exambeing = 1;
            return View();
        }

        [HttpPost]
        public ActionResult RemoveBinding(Student stu)
        {
            Exam exam = new Exam();
            foreach (var item in db.Exam.ToList())
            {
                if (item.is_being == "是")
                {
                    exam = item;
                    break;
                }
            }
            if (stu.ip_address == null)
            {
                if (stu.Id != 0 && stu.name != null && stu.stuClass != 0)
                {
                    var g = from t in db.student
                            where t.Id == stu.Id && t.exam_Id == exam.Id && t.name == stu.name && t.stuClass == stu.stuClass && t.ip_address != null
                            select t;
                    return View(g.ToList());
                }
                else if (stu.Id == 0 && stu.name != null && stu.stuClass != 0)
                {
                    var g = from t in db.student
                            where t.exam_Id == exam.Id && t.name == stu.name && t.stuClass == stu.stuClass && t.ip_address != null
                            select t;
                    return View(g.ToList());
                }
                else if (stu.Id != 0 && stu.name == null && stu.stuClass != 0)
                {
                    var g = from t in db.student
                            where t.Id == stu.Id && t.exam_Id == exam.Id && t.stuClass == stu.stuClass && t.ip_address != null
                            select t;
                    return View(g.ToList());
                }
                else if (stu.Id != 0 && stu.name != null && stu.stuClass == 0)
                {
                    var g = from t in db.student
                            where t.Id == stu.Id && t.exam_Id == exam.Id && t.ip_address != null
                            select t;
                    return View(g.ToList());
                }
                else if (stu.Id == 0 && stu.name == null && stu.stuClass != 0)
                {
                    var g = from t in db.student
                            where t.exam_Id == exam.Id && t.stuClass == stu.stuClass && t.ip_address != null
                            select t;
                    return View(g.ToList());
                }
                else if (stu.Id != 0 && stu.name == null && stu.stuClass == 0)
                {
                    var g = from t in db.student
                            where t.Id == stu.Id && t.exam_Id == exam.Id && t.ip_address != null
                            select t;
                    return View(g.ToList());
                }
                else if (stu.Id == 0 && stu.name != null && stu.stuClass == 0)
                {
                    var g = from t in db.student
                            where t.name == stu.name && t.exam_Id == exam.Id && t.ip_address != null
                            select t;
                    return View(g.ToList());
                }
                else
                {
                    var g = from t in db.student
                            where t.exam_Id == exam.Id && t.ip_address != null
                            select t;
                    return View(g.ToList());
                }
            }
            else
            {
                if (stu.ip_address != null)
                {
                    var g = from t in db.student
                            where t.ip_address == stu.ip_address && t.exam_Id == exam.Id
                            select t;
                    return View(g.ToList());
                }
                else
                {
                    var g = from t in db.student
                            where t.exam_Id == exam.Id && t.ip_address != null
                            select t;
                    return View(g.ToList());
                }
            }
        }

        [HttpPost]
        public ActionResult RemoveBinding_IP(int Id)
        {
            Student student1 = db.student.Find(Id);
            Student stu = new Student();
            stu.Id = student1.Id;
            stu.name = student1.name;
            stu.stuClass = student1.stuClass;
            stu.pwd = student1.pwd;
            stu.exam_Id = student1.exam_Id;
            stu.SavePath = student1.SavePath;
            stu.ip_address = null;
            db.student.Remove(student1);
            db.SaveChanges();
            db.student.Add(stu);
            db.SaveChanges();

            return RedirectToAction("RemoveBinding");
        }

        public ActionResult ExamInform()
        {
            Exam exam = new Exam();
            foreach (var item in db.Exam.ToList())
            {
                if (item.is_being == "是")
                {
                    exam = item;
                    break;
                }
            }

            if (exam.is_being != "是")
                ViewBag.has_exambeing = 0;
            else
                ViewBag.has_exambeing = 1;

            var g = from t in db.ExamNotice
                    where t.exam_Id == exam.Id
                    select t;

            return View(g.ToList());
        }

        [HttpPost]
        public ActionResult ExamInform(ExamNotice examNotice)
        {
            Exam exam = new Exam();
            foreach (var item in db.Exam.ToList())
            {
                if (item.is_being == "是")
                {
                    exam = item;
                    break;
                }
            }

            string tempTimeStr =DateTime.Now.Month.ToString().PadLeft(2, '0')+ "/" + DateTime.Now.Day.ToString().PadLeft(2, '0') + " " + DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Second.ToString().PadLeft(2, '0');

            examNotice.Id = db.ExamNotice.ToList().Count + 1;
            examNotice.sender = TEACHER;
            examNotice.time = tempTimeStr;
            examNotice.exam_Id = exam.Id;
            db.ExamNotice.Add(examNotice);
            db.SaveChanges();

            var g = from t in db.ExamNotice
                    where t.exam_Id == exam.Id
                    select t;

            return View(g.ToList());
        }

//------------------------------------------------------考后管理----------------------------------------------------------------
        public ActionResult AfterTest()
        {
            return View(db.Exam.ToList());
        }

        public ActionResult AfterTest_End(int exam_Id)
        {
            Exam exam1 = db.Exam.Find(exam_Id);
            Exam exam = new Exam();
            exam.Id = exam1.Id;
            exam.name = exam1.name;
            exam.time = exam1.time;
            exam.creator = exam1.creator;
            exam.creatorId = exam1.creatorId;
            exam.has_cleaned = exam1.has_cleaned;
            exam.has_saved = exam1.has_saved;
            exam.has_stopped = "是";
            exam.is_being = "否";
            exam.test_upload = exam1.test_upload;
            exam.commmit_number = exam1.commmit_number;
            exam.AnswerPath = exam1.AnswerPath;
            exam.PaperPath = exam1.PaperPath;
            db.Exam.Remove(exam1);
            db.SaveChanges();
            db.Exam.Add(exam);
            db.SaveChanges();
            var g = db.ExamNotice.ToList();
            foreach(var item in g)
            {
                db.ExamNotice.Remove(item);
            }
            db.SaveChanges();
            return RedirectToAction("AfterTest");
        }

        public ActionResult AfterTest_Download(int exam_Id)
        {
            var g = from t in db.student
                    where t.exam_Id == exam_Id
                    select t;
            foreach(var m in g.ToList())
            {
                if (m.SavePath != null)
                {
                    FileStream fs1 = new FileStream(m.SavePath, FileMode.Open, FileAccess.Read);
                    var fileName = m.SavePath.Replace(Server.MapPath(string.Format("~/Areas/{0}", "PaperAnswer")), "");
                    FileStream fs2 = new FileStream(string.Format(@"C:\Users\LMZ\Desktop\Answer\{0}", fileName), FileMode.Create, FileAccess.Write);
                    int num;
                    byte[] buffer = new byte[1024];
                    do
                    {
                        num = fs1.Read(buffer, 0, buffer.Length);
                        fs2.Write(buffer, 0, num);
                    } while (num > 0);
                    fs1.Close();
                    fs2.Close();
                }
            }
            return RedirectToAction("AfterTest");
        }

        public ActionResult AfterTest_Delete(int exam_Id)
        {
            Exam exam1 = db.Exam.Find(exam_Id);
            db.Exam.Remove(exam1);
            db.SaveChanges();
            return RedirectToAction("AfterTest");
        }

        [HttpPost]
        public ActionResult SubmitPaper(int Id)
        {
            HttpPostedFileBase FileData = Request.Files["testuploadfile"];
            Exam EX = db.Exam.Find(Id);
            var fileName = string.Format("{0}_{1}_{2}", EX.time, EX.name, EX.creator);
            var filePath = Server.MapPath(string.Format("~/Areas/{0}", "Paper"));
            EX.PaperPath = Path.Combine(filePath, fileName);
            db.SaveChanges();
            FileData.SaveAs(EX.PaperPath);
            return Json("上传成功！");
        }
    }
}