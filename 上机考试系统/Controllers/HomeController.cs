using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
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
        public ActionResult LoginStudent(Student Student)
        {
            //DatabaseEntities db = new DatabaseEntities();

            //得到从数据库中查询到的表对象
            Student stu = db.student.Find(Student.Id);

            //根据查到的表对象进行判断(若为空，说明Id输入错误)
            if (stu != null)                                                       //判断有没有这个ID号的学生
                if (stu.pwd == Student.pwd)                                         //判断学生的账号密码是否相同
                {
                    
                    IPHostEntry host = Dns.GetHostEntry("");                    //获取该学生登录的IP地址
                    var ipAdresses = host.AddressList;
                    Student.ip_address = ipAdresses[5].ToString();
                    var g = from t in db.student          //寻找数据库中和该学生登录IP一样的学生信息，用于验证，防止替考 
                            where t.ip_address == Student.ip_address&&t.ip_address!=null
                            select t;
                    List<Student> p = g.ToList();
                    if (stu.ip_address != null)             //若地址不为空，则该ID学生已经进行IP绑定
                    {

                        if (stu.ip_address == Student.ip_address)  //判断该学生IP与数据库中的IP是否一致，一致则允许登录
                        {
                            return RedirectToAction("InitStudent", "Student", new { area = "student", studentId = stu.Id});
                        }
                        else                             //不一致表明该学生用了其他IP地址登录，有可能是替考，则不允许登录
                        {
                            return Content("<script >alert('登录的IP地址非法');window.open('" + Url.Content("/Home/Login") + "', '_self')</script >", "text/html");
                        }
                    }
                    else                               //若地址为空，则该学生是第一次登录考试系统，要进行IP地址绑定
                    {
                        
                        if (p.Count != 0)      //上面Linq查找学生信息的列表，如果不为空，则表明之前有人已经注册过该IP，不允许注册
                        {
                            return Content("<script >alert('登录的IP地址非法');window.open('" + Url.Content("/Home/Login") + "', '_self')</script >", "text/html");
                        }
                        else                 //若为空，则对其进行IP地址绑定，跳转到学生界面
                        {
                            Student ST = new Student();
                            ST.Id = stu.Id;
                            ST.name = stu.name;
                            ST.pwd = stu.pwd;
                            ST.ip_address = Student.ip_address;
                            ST.stuClass = stu.stuClass;
                            ST.exam_Id = stu.exam_Id;
                            ST.SavePath = stu.SavePath;
                            db.student.Remove(stu);
                            db.student.Add(ST);
                            db.SaveChanges();
                            return RedirectToAction("InitStudent", "Student", new { area = "Student",StudentId=stu.Id});              
                        }
                    }
                }
                else
                    return Content("<script >alert('账号或密码错误');window.open('" + Url.Content("/Home/Login") + "', '_self')</script >", "text/html");  //账号密码错误
            else
                return Content("<script >alert('账号或密码错误');window.open('" + Url.Content("/Home/Login") + "', '_self')</script >", "text/html");   //找不到该学生ID
            
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