using DormitoryManagement.DAL;
using DormitoryManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DormitoryManagement.Controllers
{
    //班级
    public class ClassController : Controller
    {
        ClassDal Classdal = new ClassDal();
        StudentDal Studentdal = new StudentDal();
        ClassRecordDal ClassRecorddal = new ClassRecordDal();
        DataModelContainer db = new DataModelContainer();
        int pagesize = 5;//每页展示数据的条数
        // GET: Class
        public ActionResult Index()
        {
            IQueryable<Class> temp = from u in db.Class select u;
            int page = temp.Count() / pagesize;
            if (temp.Count() % pagesize != 0)
            {
                page = page + 1;
            }
            ViewBag.Number = page;
            return View();
        }
        //判断数据库是否已经有了这个班级
        public bool AddSelect(string Name)
        {
               return Classdal.determineClass(Name);
           
        }
        //添加班级
        [HttpPost]
        public bool Add(Class entity)
        {
            try
            {
                db.Class.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        class _Class
        {

            public string ClassName { get; set; }

            public int Id { get; set; }

            public int? Number { get; set; }

            public int State { get; set; }

        }
        //查询所有班级信息
        [HttpGet]
        public JsonResult FindAll(int page = 1)
        {
           JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(Classdal.FindAll(page), setting);
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
            var data = Classdal.Serach(page, Value);
            var ret = JsonConvert.SerializeObject(data.data, setting);
            var pages = data.count;
            int pg = 0;
            if (pages>pagesize)
            {
                pg= pages / pagesize;
                if (pages % pagesize != 0)
                {
                    pg = pg + 1;
                }
            }
            else
            {
                pg = 1;
            }
           var a= pg;
            return Json(new { ret = ret, pg = pg }, JsonRequestBehavior.AllowGet);
        }
        //修改状态
        public bool UpdateState(int id, int state)
        {
            Class temp = db.Class.Where(c => c.Id == id).FirstOrDefault();
            temp.State = state;
            var cout = db.SaveChanges();
            return cout > 0;
        }
        //查询所有班级给学生管理界面下拉框用
        public JsonResult FindALLClass()
        {
            return Json(Classdal.FindAllClass(), JsonRequestBehavior.AllowGet);
        }
        //查询班级已有学生
        public JsonResult FindStu(int ClassId)
        {
           
            return Json(Classdal.FindStu(ClassId), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 批量移除学生
        /// </summary>
        /// <param name="list">学生id</param>
        /// <param name="ClassId">班级id</param>
        /// <returns></returns>
        public bool OutStu(int[] list, int ClassId)
        {
            try
            {
                for (int i = 0; i < list.Length; i++)
                {
                    //得到学生的id
                    int StuId = list[i];
                    //得到学生的入住记录
                    List<ClassRecord> data = ClassRecorddal.FindByStuId(StuId);
                    data.FirstOrDefault().DepartureTime = DateTime.Now;
                    db.Entry(data.FirstOrDefault()).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return true;
            }catch(Exception e)
            {
                return false;
            }
            
        }
        class nscc
        {
             
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime? srCheckinTime { get; set; }
            public string Number { get; set; }
            public DateTime? crCheckinTime { get; set; }
            public string CLassName { get; set; }

        }
        //根据班级id或者学生姓名查询学生
        [HttpPost]
        public JsonResult SelectStuByNameorClass(int Classid, string StudentName)
        {


            if (Classid != 0 && StudentName == "")//根据班级查询改班级的所有学生
            {

                JsonSerializerSettings setting = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None
                };
                var ret = JsonConvert.SerializeObject(Studentdal.FindStuByClassId(Classid), setting);
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            else if (Classid == 0 && StudentName != "")//根据学生名查学生
            {

                JsonSerializerSettings setting = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None
                };
                var ret = JsonConvert.SerializeObject(Studentdal.FindStuByStuName(StudentName), setting);
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            else if (Classid != 0 && StudentName != "")//根据学生姓名和班级查询
            {

                JsonSerializerSettings setting = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None
                };
                var ret = JsonConvert.SerializeObject(Studentdal.FindStuByClassIdAndStuName(Classid, StudentName), setting);
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            else//查询所有班级的所有学生的寝室信息
            {
                JsonSerializerSettings setting = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None
                };
                var ret = JsonConvert.SerializeObject(Studentdal.FindALLByno(), setting);
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
        }
        //批量修改
        public bool IntoStu(int []list, int Classid)
        {
            try{
                for (int i = 0; i < list.Length; i++)
                {
                    int StuId = list[i];
                    List<ClassRecord> data = ClassRecorddal.FindByStuId(StuId);
                    if(data.FirstOrDefault().DepartureTime==null)
                    {
                        data.FirstOrDefault().DepartureTime = DateTime.Now;
                        db.Entry(data.FirstOrDefault()).State = EntityState.Modified;
                        db.SaveChanges();
                        ClassRecord cr = new ClassRecord();
                        cr.ClassId = Classid;
                        cr.CheckinTime = DateTime.Now;
                        cr.StudentId = StuId;
                        cr.DepartureTime = null;
                        db.ClassRecord.Add(cr);
                        db.SaveChanges();
                    }
                    else
                    {
                        ClassRecord cr = new ClassRecord();
                        cr.ClassId = Classid;
                        cr.CheckinTime = DateTime.Now;
                        cr.StudentId = StuId;
                        cr.DepartureTime = null;
                        db.ClassRecord.Add(cr);
                        db.SaveChanges();
                    }
                }
                return true;
            }catch(Exception e)
            {
                return false;
            }
            
            
        }
        //修改班级信息
        [HttpPost]
        public bool UpdateClassInfo(int ClassId, string Number)
        {
           bool pd= Classdal.UpadateClass(ClassId, Number);
            if (pd==true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //联想词功能
        [HttpGet]
        public JsonResult Lenovo(String Value)
        {
          var data= Classdal.Lenovo(Value);
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(data, setting);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }




    }
}