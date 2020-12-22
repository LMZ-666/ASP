﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using 上机考试系统.Models.DatabaseModel;

namespace 上机考试系统.Areas.student.Controllers
{
    
    public class StudentController : Controller
    {
        DatabaseEntities db = new DatabaseEntities();
        static int MyId;
        public static int MyTestId;
        static String MyStudentName;
        static String TestTime;
        static String TestName;
        static String SavePath;
        // GET: student/Student

        public ActionResult InitStudent(int StudentId)
        {
            Student ST = db.student.Find(StudentId);
            MyId = StudentId;
            MyTestId = (int)ST.exam_Id;
            MyStudentName = ST.name;
            return RedirectToAction("StudentIndex");
        }
        [HttpPost]
        public ActionResult SubmitAnswer()
        {
            
            String PaperAnswer = Request.Form["PaperAnswer"];
            byte[] data = Encoding.UTF8.GetBytes(PaperAnswer);
            var filePath = Server.MapPath(string.Format("~/Areas/{0}", "PaperAnswer"));
            var fileName = string.Format("{2}_{3}_{0}_{1}.txt", MyId.ToString(), MyStudentName, TestName, TestTime);
            SavePath = Path.Combine(filePath, fileName);
            Exam EX = db.Exam.Find(MyTestId);
            EX.commmit_number += 1;
            EX.AnswerPath = SavePath;
            db.SaveChanges();
            FileStream fs = new FileStream(SavePath, FileMode.Create, FileAccess.ReadWrite);
            fs.Write(data, 0, data.Length);
            fs.Close();
            return RedirectToAction("CheckedAnswer");


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
                TestName = EX.name;
                TestTime = EX.time;
                
                String paperpath = EX.PaperPath;
                FileStream fs = new FileStream(paperpath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                ViewBag.Paper = sr.ReadToEnd();
                
                str = String.Format("本场考试的科目名称为：{0}，考试时间为：{1}，监考教师是：{2}", EX.name, EX.time, EX.creator);
            }
            else
            {
                str = "没有要进行的考试";
            }
            ViewBag.Message = str;
            return View(EX);
        }
        public ActionResult CheckedAnswer()
        {
            try
            {
                FileStream fs = new FileStream(SavePath, FileMode.Open, FileAccess.Read);
                StreamReader Reader = new StreamReader(fs);
                ViewBag.PaperAnswer = Reader.ReadToEnd();
                ViewBag.SubmitResult = "答案已经提交成功";
                fs.Close();
            }
            catch (Exception)
            {

                ViewBag.PaperAnswer = "";
                ViewBag.SubmitResult = "找不到已提交答案，请重新提交";
            }
            return View();
        }
        public ActionResult CheckedIPAddress()
        {
            Student stu = db.student.Find(MyId);
            ViewBag.IPAddress = stu.ip_address;
            return View();
        }
    }
}