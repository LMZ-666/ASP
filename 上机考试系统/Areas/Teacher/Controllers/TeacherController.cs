﻿using System;
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
        public static String TEACHER;   // 用于存储登陆的教师名称
        public static int TEACHERID;    // 用于存储登陆的教师ID
        public static int ExamId;
        private DatabaseEntities db = new DatabaseEntities();   
        // GET: Teacher/Teacher
        public ActionResult Initial(String teacherName, int teacherId)  // 初次登陆时初始化静态变量
        {
            TEACHER = teacherName;
            TEACHERID = teacherId;

            return RedirectToAction("TeacherIndex");    // 跳转到教师首页函数
        }

        public ActionResult TeacherIndex()  // 教师首页
        {
            ViewBag.teacherName = TEACHER;  // 将教师名称传参到视图显示
            return View();
        }

//------------------------------------------------------考前管理----------------------------------------------------------------

        public ActionResult BeforeTest()    // 考前操作首页
        {
            return View(db.Exam.ToList());  // 将考试信息传入视图显示
        }

        [HttpPost]
        public ActionResult BeforeTest(Exam exam)   // 考前操作的添加考试
        {
            Exam exam1 = new Exam();
            exam1.Id = 0;
            foreach (var item in db.Exam.ToList())  // 获取exam_Id最大的考试
            {
                if(exam1.Id<item.Id)
                {
                    exam1.Id = item.Id;
                }
            }
            exam.Id = exam1.Id + 1;         // 新建的考试的ID为已存在的考试ID的最大值加一（为了避免主键ID重复导致出错）
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

        public ActionResult ExamEdit(int exam_Id)   // 编辑考试页面
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
            ViewBag.has_exambeing = has_exambeing;  // 本参数用于表示现在是否有考试正在进行，传入视图用于判断是否可以开启考试
            var g = from t in db.student
                    where t.exam_Id == exam_Id
                    select t;
            var p = g.ToList();
            ViewBag.stuNum = p.Count(); // 本参数用于表示参加本次考试的考生人数，传入视图用于显示
            return View();
        }

        [HttpPost]
        public ActionResult ExamEdit_Mod(Exam exam) // 编辑考试中的修改考试信息
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
        public ActionResult SubmitPaper(int Id) // 考前操作的编辑考试页面的上传试卷
        {
            HttpPostedFileBase FileData = Request.Files["testuploadfile"];          //获取选择的文件
            Exam EX = db.Exam.Find(Id);                                             //通过考试ID获取该场考试的信息
            var fileName = string.Format("{0}_{1}_{2}", EX.time, EX.name, EX.creator);  //初始化试卷文件名
            var filePath = Server.MapPath(string.Format("~/Areas/{0}", "Paper"));       //初始化试卷路径（不包括文件名）
            EX.PaperPath = Path.Combine(filePath, fileName);                            //将试卷路径保存到数据库
            db.SaveChanges();
            FileData.SaveAs(EX.PaperPath);                                           //将选择的文件保存到试卷路径
            return Json("上传成功！");
        }

        [HttpPost]
        public ActionResult ExamEdit_add(Student stu)   // 编辑考试中的添加考生
        {
            stu.pwd = "123456";
            db.student.Add(stu);
            db.SaveChanges();

            return RedirectToAction("ExamEdit", new { exam_Id=stu.exam_Id });
        }

        [HttpPost]
        public ActionResult ExamEdit_being(Exam exam)   // 编辑考试中的开启本次考试
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

        public ActionResult TestCondition() // 考中管理的考试概况页面
        {
            Exam exam = new Exam();
            foreach (var item in db.Exam.ToList())  // 获取正在进行考试的ID
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

        public ActionResult StudentInfo()   // 考中管理的学生信息页面
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
        public ActionResult StudentInfo(Student stu)    // 考中管理的学生信息页面的查询学生
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
            // 为了实现查询时当输入框为空是视为无条件查找，所有分成若干中参数为NULL的情况
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
        public ActionResult StudentInfo_add(Student stu)    //考中管理的学生信息页面的添加考生
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

        public ActionResult RemoveBinding() // 考中管理的解除绑定页面
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
        public ActionResult RemoveBinding(Student stu)  // 考中管理的解除绑定页面的按基本信息查询和按IP地址查询函数
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
            // 为了实现查询时当输入框为空是视为无条件查找，所有分成若干种参数为NULL的情况
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
        public ActionResult RemoveBinding_IP(int Id)    //考中管理的解除绑定页面的解除IP地址绑定函数
        {
            // 只是简单的将一位学生的IP地址信息置为NULL
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

        public ActionResult ExamInform()    // 考中管理的通知管理页面
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
        public ActionResult ExamInform(ExamNotice examNotice)   // 考中管理的通知管理页面的发布通告函数
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
            // 获取发布通知的时间
            string tempTimeStr =DateTime.Now.Month.ToString().PadLeft(2, '0')+ "/" + DateTime.Now.Day.ToString().PadLeft(2, '0') + " " + DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Second.ToString().PadLeft(2, '0');

            examNotice.Id = db.ExamNotice.ToList().Count + 1;
            examNotice.sender = TEACHER;
            examNotice.time = tempTimeStr;
            examNotice.exam_Id = exam.Id;
            db.ExamNotice.Add(examNotice);
            db.SaveChanges();
            // 获取本次考试的考试通知列表
            var g = from t in db.ExamNotice
                    where t.exam_Id == exam.Id
                    select t;

            return View(g.ToList());
        }

//------------------------------------------------------考后管理----------------------------------------------------------------
        public ActionResult AfterTest() // 考后管理页面
        {
            return View(db.Exam.ToList());
        }

        public ActionResult AfterTest_End(int exam_Id) // 考后管理页面的结束考试
        {
            // 只是简单的将exam的is_being参数变为‘否’，将has_stopped参数变为‘是’
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
            // 删除本次考试的所有考中通告
            var g = db.ExamNotice.ToList();
            foreach(var item in g)
            {
                db.ExamNotice.Remove(item);
            }
            db.SaveChanges();
            return RedirectToAction("AfterTest");
        }

        public ActionResult AfterTest_Download(int exam_Id) // 考后管理页面的下载学生答卷
        {
            var g = from t in db.student                                // 获取参加该场考试的所有学生
                    where t.exam_Id == exam_Id
                    select t;                                               
            foreach(var m in g.ToList())
            {
                if (m.SavePath != null)                              // 判断参加这场考试的每个学生是否提交了答案
                {
                    FileStream fs1 = new FileStream(m.SavePath, FileMode.Open, FileAccess.Read);            // 若提交了答案就会有答案路径，流读取答案路径的文件
                    var fileName = m.SavePath.Replace(Server.MapPath(string.Format("~/Areas/{0}", "PaperAnswer")), "");             // 初始化文件名
                    FileStream fs2 = new FileStream(string.Format(@"C:\Users\User\Desktop\data\{0}", fileName), FileMode.Create, FileAccess.Write);   // 将要打包的答案输入到对应的路径，这里的路径时桌面的data
                    int num;                                                                           // 用于判断流是否读完
                    byte[] buffer = new byte[1024];                       // 字节数组用于限制上传速度，这里设置了每次下1024bit(1kb)
                    do
                    {
                        num = fs1.Read(buffer, 0, buffer.Length);        // 获取每次读取的长度
                        fs2.Write(buffer, 0, num);                      // 将读到的长度的字节数组全部输入到对应的路径
                    } while (num > 0);
                    fs1.Close();
                    fs2.Close();                                     // 流的关闭
                }
            }
            return RedirectToAction("AfterTest");
        }

        public ActionResult AfterTest_Delete(int exam_Id)   // 考后管理页面的删除考试
        {
            Exam exam1 = db.Exam.Find(exam_Id);
            db.Exam.Remove(exam1);
            db.SaveChanges();
            return RedirectToAction("AfterTest");
        }
    }
}