using DormitoryManagement.Models;
using EntityFramework.Extensions;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DormitoryManagement.DAL;

namespace DormitoryManagement.Controllers
{
    public class StudentController : Controller
    {
        StudentDal Studal = new StudentDal();
        ClassDal Classdal = new ClassDal();
        //学生
        DataModelContainer db = new DataModelContainer();
        int pagesize = 5;//每页展示数据的条数
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        //查询所有学生信息
        public JsonResult FindAll(int page = 1)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(Studal.StudentFindAll(page), setting);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        //搜索
        [HttpPost]
        public JsonResult FindAll(string Value, int page = 1)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var data = Studal.Serach(page, Value);
            var ret = JsonConvert.SerializeObject(data.data, setting);
            var pages = data.count;
            int pg = 0;
            if (pages > pagesize)
            {
                pg = pages / pagesize;
                if (pages % pagesize != 0)
                {
                    pg = pg + 1;
                }
            }
            else
            {
                pg = 1;
            }
            return Json(new { ret = ret, pg = pg }, JsonRequestBehavior.AllowGet);
        }
        //添加学生
        [HttpPost]
        public bool Add(string Name,string Contact,int Classid)
        {    
            try
            {
               bool pd= Studal.AddStudent(Name, Contact, Classid);
                if(pd==true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
               
            }
            catch (Exception e)
            {
                return false;
            }

        } //判断数据库是否已经有了这个学生
        public bool AddSelect(string Name,string Contact)
        {
            try
            {
                bool pd= Studal.determineStu(Name, Contact);
                return pd;
            }
            catch (Exception e)
            {
                return false;
            }

        }
         
        //查询所有班级给学生修改班级的时候用
        public JsonResult FindAllClass()
        {
           
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(Classdal.FindAllClass(), setting);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        class nowClass
        {
            public string ClassName { get; set; }
            public int Id { get; set; }
        }
        //查询学生现在所在的班级
        [HttpPost]
        public JsonResult FindStuNowClass( int stuid)
        {
           JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(Studal.FindStuNowClass(stuid), setting);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        //修改学生所在班级
        [HttpPost]
        public bool UpdateClass(int BeforeClassName, int NewClassName,int StuId)
        {
            
             bool pd = Studal.UpdateClass(BeforeClassName, NewClassName, StuId);
             return pd;
           

        }
        //联想词功能
        [HttpGet]
        public JsonResult Lenovo(String Value)
        {
            var data = Studal.Lenovo(Value);
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(data, setting);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        //修改状态
        public bool UpdateState(int id, int state)
        {
            return Studal.UpdateState(id, state);
        }
        //修改学生信息
        [HttpPost]
        public bool UpdateStuInfo(int StuId,string Name,string Contact)
        {
            bool pd = Studal.UpdateStuInfo(StuId, Name, Contact);
            return pd;
        }
        //excel导入
        [HttpPost]
        public string UploadXls()
        {
            try
            {
                HttpPostedFileBase file = HttpContext.Request.Files["BulkXls"];
                if (file == null)
                {
                    return "请选择文件";
                }
                var ext = Path.GetExtension(file.FileName);
                var exts = ",.xls,.xlsx,";
                if (!exts.Contains("," + ext + ","))
                {
                    return "请上传.xls,.xlsx文件";
                }
                //上传文件
                var path = HttpContext.Request.MapPath("~/");
                var dir = "/Upload_Files/xls/";
                if (!Directory.Exists(path + dir))
                {
                    Directory.CreateDirectory(path + dir);
                }
                //保存文件
                var fullPath = path + dir + "hotels" + Path.GetExtension(file.FileName);
                file.SaveAs(fullPath);

                var filePath = Server.MapPath("~/Upload_Files/xls/" + "hotels" + Path.GetExtension(file.FileName));
                // 读取Excel文件到DataSet中
                string connStr = "";
                string fileType = System.IO.Path.GetExtension(filePath);
                if (!string.IsNullOrEmpty(fileType))
                {
                    //支持文件格式不一样，Excel有 97-2003及2007格式  分别为 .xls和.xlsx
                    if (fileType == ".xls")
                    { connStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\""; }
                    else
                    { connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\""; }
                    string sql_F = "Select * FROM [{0}]";

                    OleDbConnection conn = null;
                    OleDbDataAdapter da = null;

                    DataSet ds = new DataSet();
                    try
                    {
                        // 初始化连接，并打开
                        conn = new OleDbConnection(connStr);
                        conn.Open();
                        // 初始化适配器
                        da = new OleDbDataAdapter();
                        da.SelectCommand = new OleDbCommand(String.Format(sql_F, "Sheet1$"), conn);
                        DataSet dsItem = new DataSet();
                        da.Fill(dsItem, "t");
                        ds.Tables.Add(dsItem.Tables[0].Copy());
                        //这里的for是循环DataSet中的值并向数据库中插入
                        for (int i = 0; i < dsItem.Tables[0].Rows.Count; i++)
                        {

                           // var student = dsItem.Tables[0];//得到表明
                            ////比如我们导入的 Excel表中有 四列
                            //if (!_HotelRepository.IsExist(u => u.HotelName == hotelName))
                            //{
                                //实例化一个Student对象并赋值
                                Student model = new Student();
                                //这里是Excel表格中对应的列
                           string Name=model.Name = dsItem.Tables[0].Rows[i]["姓名"].ToString();//得到学生姓名
                            string Contact = model.Contact = dsItem.Tables[0].Rows[i]["联系方式"].ToString();//得到学生联系方式
                            string state = dsItem.Tables[0].Rows[i]["状态"].ToString();//得到学生状态
                            string Class = dsItem.Tables[0].Rows[i]["所在班级"].ToString();//得到学生状态
                            int State =0;//用于保存学生当前状态
                            if(state.Equals("在读"))
                            {
                                State = 0;
                            }
                            else if(state.Equals("已毕业"))
                            {
                                State = 1;
                            }
                            else
                            {
                                return "学生状态格式错误只能有在读和已毕业两种状态";
                            }
                                model.State = State;
                            //查询是否已有该学生
                           bool pdStu= Studal.determineStu(Name, Contact);
                            if(pdStu==true)
                            {
                                //如果已有该学生就修改学生的信息
                                db.Student.Where(s => s.Name ==Name ).Update(b => new Student { Name = Name, Contact = Contact,State= State });
                                //判断这个学生的班级
                                List<nowClass> ClassInfo = db.Database.SqlQuery<nowClass>("select c.ClassName,c.Id from Student s left join ClassRecord cr on s.Id =cr.StudentId left join Class c on c.Id=cr.ClassId  where cr.DepartureTime is null and s.Name='" + Name + "'").ToList();
                                if(ClassInfo.FirstOrDefault().ClassName==null)
                                {
                                    //如果没有就添加一个班级并给它添加一个学生
                                    Class cls = new Class();
                                    cls.ClassName = Class;
                                    cls.State = 0;
                                    db.Class.Add(cls);//添加一个班级
                                    db.SaveChanges();
                                    //班级添加学生
                                    ClassRecord cr = new ClassRecord();
                                    cr.ClassId = (from c in db.Class where c.ClassName == Class && c.State == 0 select c.Id).First();
                                    cr.CheckinTime = DateTime.Now;
                                    cr.StudentId = (from s in db.Student where s.Name == Name && s.State == 0 select s.Id).First();
                                    cr.DepartureTime = null;
                                    db.ClassRecord.Add(cr);
                                    db.SaveChanges();
                                }

                                string className = ClassInfo.FirstOrDefault().ClassName;//得到学生当前所在班级班级名
                                int BeforeClassName = ClassInfo.FirstOrDefault().Id;//得到学生当前所在班级班级Id
                                if (className != Class)//如果批量导入的学生班级与数据库内数据不一致就修改学生当前班级
                                {
                                    //判断数据库是否有该班级
                                    IQueryable<Class> clstemp = from u in db.Class where u.ClassName == Class  select u;
                                    if (clstemp.Count() > 0)//如果有这个班级就直接修改班级记录添加学生
                                    {
                                        Class temp = db.Class.Where(c => c.ClassName == Class).FirstOrDefault();
                                        temp.State = 0;  //把该班级设置为启用状态。                                    
                                        //修改学生班级记录改为离开该班级
                                        db.ClassRecord.Where(cr => cr.StudentId == db.Student.Where(s => s.Name == Name).FirstOrDefault().Id && cr.ClassId == BeforeClassName && cr.DepartureTime == null).Update(b => new ClassRecord { DepartureTime = DateTime.Now });
                                        //重新添加一条新的班级记录
                                        ClassRecord Cr = new ClassRecord();
                                        Cr.ClassId = (from c in db.Class where c.ClassName == Class && c.State==0 select c.Id).First();
                                        Cr.CheckinTime = DateTime.Now;
                                        Cr.StudentId = (from s in db.Student where s.Name == Name && s.State == 0 select s.Id).First();
                                        Cr.DepartureTime = null;
                                        db.ClassRecord.Add(Cr);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        //如果没有就添加一个班级并给它添加一个学生
                                        Class cls = new Class();
                                        cls.ClassName = Class;
                                        cls.State = 0;
                                        db.Class.Add(cls);//添加一个班级
                                        db.SaveChanges();
                                        //班级添加学生
                                        ClassRecord cr = new ClassRecord();
                                        cr.ClassId = (from c in db.Class where c.ClassName == Class && c.State == 0 select c.Id).First();
                                        cr.CheckinTime = DateTime.Now;
                                        cr.StudentId = (from s in db.Student where s.Name == Name && s.State==0 select s.Id).First();
                                        cr.DepartureTime = null;
                                        db.ClassRecord.Add(cr);
                                        db.SaveChanges();
                                    }
                                       
                                }
                            }
                            else//如果没有这个学生就添加学生
                            {
                                //执行插入
                                db.Student.Add(model);//添加学生
                                db.SaveChanges();
                                //判断数据库是否有该班级
                                IQueryable<Class> clstemp = from u in db.Class where u.ClassName == Class && u.State == 0 select u;
                                if (clstemp.Count() > 0)//如果有这个班级就直接修改班级信息
                                {
                                    ClassRecord Cr = new ClassRecord();
                                    Cr.ClassId = (from c in db.Class where c.ClassName == Class && c.State == 0 select c.Id).First();
                                    Cr.CheckinTime = DateTime.Now;
                                    var a = Name;
                                    Cr.StudentId =(from s in db.Student where s.Name == Name select s.Id).First();
                                    Cr.DepartureTime = null;
                                    db.ClassRecord.Add(Cr);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    //如果没有就添加一个班级并给它添加一个学生
                                    Class cls = new Class();
                                    cls.ClassName = Class;
                                    cls.State = 0;
                                    db.Class.Add(cls);//添加一个班级
                                    db.SaveChanges();
                                    //班级添加学生
                                    ClassRecord cr = new ClassRecord();
                                    cr.ClassId = db.Class.Where(c => c.ClassName == Class && c.State == 0).FirstOrDefault().Id;
                                    cr.CheckinTime = DateTime.Now;
                                    cr.StudentId = db.Student.Where(s => s.Name == Name && s.State == 0).FirstOrDefault().Id;
                                    cr.DepartureTime = null;
                                    db.ClassRecord.Add(cr);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        // 关闭连接
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                            da.Dispose();
                            conn.Dispose();
                        }
                    }
                }


            }
            catch (Exception ex)
            {

            }
            return "";
        }
    }
}