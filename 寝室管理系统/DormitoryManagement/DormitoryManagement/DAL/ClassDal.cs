using DormitoryManagement.Models;
using EntityFramework.Extensions;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace DormitoryManagement.DAL
{
    public class ClassDal
    {
        DataModelContainer db = new DataModelContainer();//上下文
        int pagesize = 5;//每页展示数据的条数
        public class _Class
        {

            public string ClassName { get; set; }

            public int Id { get; set; }

            public int? Number { get; set; }

            public int State { get; set; }

        }
        //查询所有班级
        public object FindAllClass()
        {
            var data = (from u in db.Class select new { u.ClassName, u.Id }).ToList();
            return data;
        }
        /// <summary>
        /// 判断数据库是否已有该班级
        /// </summary>
        /// <param name="Name">班级名</param>
        /// <returns></returns>
        public bool determineClass(string Name)
        {
            IQueryable<Class> temp = from u in db.Class where u.ClassName == Name select u;
            int pd = temp.Count();
            return pd > 0;
        }
        /// <summary>
        /// 查询所有班级
        /// </summary>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public List<_Class> FindAll(int page)
        {
            var data = db.Database.SqlQuery<_Class>("select d.ClassName ,d.Id,count(t.ClassId) Number ,d.State  from  ClassRecord t right join Class d on d.id=t.ClassId and t.DepartureTime is null group by  d.State ,d.ClassName, d.Id").ToPagedList(page, pagesize).ToList(); ;
            return data;
        }
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="page">页码</param>
        ///  /// <param name="Value">搜索框内的值</param>
        /// <returns></returns>
        public class hd
        {
            public List<_Class> data { get; set; }
            public int count { get; set; }
        }

        public hd Serach(int page, string Value)
        {
            var data = new List<_Class>();
            var cout = new List<_Class>();
            //Contains函数判断该字符串是否包含该字段
            if (Value.Contains("停用"))
            {

                var classnumber = Value.Split(new string[] { "停用" }, StringSplitOptions.RemoveEmptyEntries);
                if (classnumber.Length == 0)
                {
                    data = db.Database.SqlQuery<_Class>("select d.ClassName ,d.Id,count(t.ClassId) Number ,d.State  from  ClassRecord t right join Class d on d.id=t.ClassId and t.DepartureTime is null where  State=1 group by  d.State ,d.ClassName, d.Id").ToPagedList(page, pagesize).ToList();
                    cout = db.Database.SqlQuery<_Class>("select d.ClassName ,d.Id,count(t.ClassId) Number ,d.State  from  ClassRecord t right join Class d on d.id=t.ClassId and t.DepartureTime is null where  State=1 group by  d.State ,d.ClassName, d.Id").ToList();
                }
                else
                {
                    for (int i = 0; i < classnumber.Length; i++)
                    {
                        data = db.Database.SqlQuery<_Class>("select d.ClassName ,d.Id,count(t.ClassId) Number ,d.State  from  ClassRecord t right join Class d on d.id=t.ClassId and t.DepartureTime is null where ClassName like'%" + classnumber[i] + "%' and State=1 group by  d.State ,d.ClassName, d.Id").ToPagedList(page, pagesize).ToList();

                    }
                }

                return new hd() { data = data, count = cout.Count() };
            }
            else if (Value.Contains("启用"))
            {
                var classnumber = Value.Split(new string[] { "启用" }, StringSplitOptions.RemoveEmptyEntries);
                if (classnumber.Length == 0)
                {
                    data = db.Database.SqlQuery<_Class>("select d.ClassName ,d.Id,count(t.ClassId) Number ,d.State  from  ClassRecord t right join Class d on d.id=t.ClassId and t.DepartureTime is null where  State=0 group by  d.State ,d.ClassName, d.Id").ToPagedList(page, pagesize).ToList();
                    cout = db.Database.SqlQuery<_Class>("select d.ClassName ,d.Id,count(t.ClassId) Number ,d.State  from  ClassRecord t right join Class d on d.id=t.ClassId and t.DepartureTime is null where  State=0 group by  d.State ,d.ClassName, d.Id").ToList();
                }
                else
                {
                    for (int i = 0; i < classnumber.Length; i++)
                    {
                        data = db.Database.SqlQuery<_Class>("select d.ClassName ,d.Id,count(t.ClassId) Number ,d.State  from  ClassRecord t right join Class d on d.id=t.ClassId and t.DepartureTime is null where ClassName like'%" + classnumber[i] + "%' and State=0 group by  d.State ,d.ClassName, d.Id").ToPagedList(page, pagesize).ToList();

                    }
                }

                return new hd() { data = data, count = cout.Count() };
            }
            else if (Value != "")
            {
                data = db.Database.SqlQuery<_Class>("select d.ClassName ,d.Id,count(t.ClassId) Number ,d.State  from  ClassRecord t right join Class d on d.id=t.ClassId and t.DepartureTime is null where ClassName like'%" + Value + "%' group by  d.State ,d.ClassName, d.Id").ToPagedList(page, pagesize).ToList();
                cout = db.Database.SqlQuery<_Class>("select d.ClassName ,d.Id,count(t.ClassId) Number ,d.State  from  ClassRecord t right join Class d on d.id=t.ClassId and t.DepartureTime is null where ClassName like'%" + Value + "%' group by  d.State ,d.ClassName, d.Id").ToList();
                return new hd() { data = data, count = cout.Count() };
            }
            else {
                data = db.Database.SqlQuery<_Class>("select d.ClassName ,d.Id,count(t.ClassId) Number ,d.State  from  ClassRecord t right join Class d on d.id=t.ClassId and t.DepartureTime is null group by  d.State ,d.ClassName, d.Id").ToPagedList(page, pagesize).ToList();
                cout = db.Database.SqlQuery<_Class>("select d.ClassName ,d.Id,count(t.ClassId) Number ,d.State  from  ClassRecord t right join Class d on d.id=t.ClassId and t.DepartureTime is null group by  d.State ,d.ClassName, d.Id").ToList();
                return new hd() { data = data, count = cout.Count() };
            }


        }
        /// <summary>
        /// 查询班级已有的学生
        /// </summary>
        /// <param name="ClassId">班级id</param>
        /// <returns></returns>
        public object FindStu(int ClassId)
        {
            var data = (from a in db.ClassRecord
                        join b in db.Student on a.StudentId equals b.Id
                        join c in db.Class on a.ClassId equals c.Id
                        where c.Id == ClassId && a.DepartureTime == null
                        select new { b.Id, b.Name, b.Contact });
            return data;
        }
        /// <summary>
        /// 修该班级信息
        /// </summary>
        /// <param name="ClassId">班级id</param>
        /// <param name="Number">班级号</param>
        /// <returns></returns>
        public bool UpadateClass(int ClassId, string Number)
        {
           var data= db.Class.Where(cr => cr.Id == ClassId).FirstOrDefault();
            data.ClassName = Number;
            int cout = db.SaveChanges();
            if (cout > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       /// <summary>
       /// 联想词功能
       /// </summary>
       /// <param name="Conditions">输入内容</param>
       /// <returns></returns>
        public object Lenovo(string Conditions)
        {
            var data =(from c in db.Class where c.ClassName.Contains(Conditions) select new { c.ClassName }).ToList().Take(5);
            return data;
        }
       
    }
}