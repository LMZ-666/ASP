using System;
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
            Student ST = db.student.Find(StudentId);   //获取登录界面传入的Id的学生信息
            MyId = StudentId;                           //将获取到的学生信息传入控制器静态变量
            MyTestId = (int)ST.exam_Id;
            MyStudentName = ST.name;
            return RedirectToAction("StudentIndex");   //重定向到学生界面
        }
        [HttpPost]                                          //表单POST请求
        public ActionResult SubmitAnswer()
        {         
            String PaperAnswer = Request.Form["PaperAnswer"];   //获取答卷文本框内的信息
            byte[] data = Encoding.UTF8.GetBytes(PaperAnswer);   //将获取到的文本框信息用UTF-8解码成字节数组
            var filePath = Server.MapPath(string.Format("~/Areas/{0}", "PaperAnswer"));    //初始化答案文件路径
            var fileName = string.Format("{2}_{3}_{0}_{1}.txt", MyId.ToString(), MyStudentName, TestName, TestTime); //初始化答案文件名称
            Exam EX = db.Exam.Find(MyTestId);               //获取该场考试信息
            Student ST = db.student.Find(MyId);             //获取参加考试的这个学生的信息
            ST.SavePath = Path.Combine(filePath, fileName);    //将初始化的答案文件路径和名称合并，并保存到该学生SavePath的那一列
            SavePath = ST.SavePath;
            db.SaveChanges();
            FileStream fs = new FileStream(ST.SavePath, FileMode.Create, FileAccess.ReadWrite); //将字节数组作为流读入合并后的路径
            fs.Write(data, 0, data.Length);
            fs.Close();
            return RedirectToAction("CheckedAnswer");     //重定向到已提交答案界面

        }
        public ActionResult StudentIndex()
        {

            ViewBag.studentName = MyStudentName;
            return View();
        }
        public ActionResult StartTest()
        {

            Exam EX = db.Exam.Find(MyTestId);    //MyTestId为上面设置的静态变量，用于保存上面StudentInit传入的参数，寻找该考试要参与的考试
            String str;
            if(EX!=null)                               
            {
                TestName = EX.name;             //保存该场考试的信息
                TestTime = EX.time;
                String paperpath = EX.PaperPath;   //保存该场考试试卷路径
                FileStream fs = new FileStream(paperpath, FileMode.Open, FileAccess.Read);   //显示试卷信息，文件流读取考试试卷的路径
                StreamReader sr = new StreamReader(fs);
                ViewBag.Paper = sr.ReadToEnd();
                str = String.Format("本场考试的科目名称为：{0}，考试时间为：{1}，监考教师是：{2}", EX.name, EX.time, EX.creator);//用于界面显示大致的考试信息
            }
            else
            {
                str = "没有要进行的考试";
            }
            ViewBag.Message = str;   //用于视图界面显示该场考试的大致信息
            return View(EX);
        }
        public ActionResult CheckedAnswer()
        {
            try
            {
                FileStream fs = new FileStream(SavePath, FileMode.Open, FileAccess.Read);  //读取合并后的路径文件
                StreamReader Reader = new StreamReader(fs);                           //用StreamReader读取文本
                ViewBag.PaperAnswer = Reader.ReadToEnd();                           //显示读取到的答案
                ViewBag.SubmitResult = "答案已经提交成功";                          //显示答案提交结果
                fs.Close();
            }
            catch (Exception)                                            //错误处理结果统一为找不到答案
            {

                ViewBag.PaperAnswer = "";
                ViewBag.SubmitResult = "找不到已提交答案，请重新提交";
            }
            return View();
        }
        public ActionResult CheckedIPAddress()
        {
            Student stu = db.student.Find(MyId);   //获取参与这场考试的学生的信息
            ViewBag.IPAddress = stu.ip_address;   //获取该学生绑定的IPAddress
            return View();
        }
    }
}