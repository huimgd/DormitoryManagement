using DormitoryManagement.Models;
using EntityFramework.Extensions;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DormitoryManagement.DAL
{
    //学生信息
   public class Stu
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Contact { get; set; }
        public string ClassName { get; set; }
        public int State { get; set; }
    }
    //学生当前所在班级
    public class nowClass
    {
        public string ClassName { get; set; }
        public int Id { get; set; }
       
    }
    //保存查询出的学生信息
    public class nscc
    {
        public int Id { get; set; }//学生id
        public string Name { get; set; }//学生姓名
        public DateTime? srCheckinTime { get; set; }//入住记录——进入时间
        public string Number { get; set; }//寝室编号
        public DateTime? crCheckinTime { get; set; }//入班记录--进入时间
        public string CLassName { get; set; }//班级名

    }
    public class StudentDal
    {
        DataModelContainer db = new DataModelContainer();//上下文
        int pagesize = 5;//每页展示数据的条数
        /// <summary>
        /// 查询所有学生
        /// </summary>
        /// <param name="page">页数</param>
        /// <returns></returns>
        public List<Stu> StudentFindAll(int page = 1)
        {
            var data = db.Database.SqlQuery<Stu>("select s.Id,s.Contact,s.Name,s.State,c.ClassName from Student s left join ClassRecord cr on s.Id =cr.StudentId left join Class c on c.Id=cr.ClassId  where cr.DepartureTime is null ").ToPagedList(page, pagesize).ToList();
            return data;
        }
        /// <summary>
        ///添加学生
        /// </summary>
        /// <param name="Name">学生姓名</param>
        /// <param name="Contact">学生联系方式</param>
        /// <param name="Classid">所在班级id</param>
        /// <returns></returns>
        public bool AddStudent(string Name, string Contact, int Classid)
        {
            Student stu = new Student();
            stu.Name = Name;
            stu.Contact = Contact;
            stu.State = 0;
            db.Student.Add(stu);
            db.SaveChanges();//给学生表添加数据
            var stuid = db.Student.AsEnumerable().Last().Id;
            ClassRecord cr = new ClassRecord();//给班级表添加数据
            cr.ClassId = Classid;
            cr.CheckinTime = DateTime.Now;
            cr.DepartureTime = null;
            cr.StudentId = stuid;
            db.ClassRecord.Add(cr);
          int cout= db.SaveChanges();
            if(cout>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        ///修改学生状态
        /// </summary>
        /// <param name="id">学生id</param>
        /// <param name="state">目标状态</param>
        /// <returns></returns>
        public bool UpdateState(int id, int state)
        {
            Student temp = db.Student.Where(c => c.Id == id).FirstOrDefault();
            temp.State = state;
            var cout = db.SaveChanges();
            return cout > 0;
        }
        /// <summary>
        /// 根据学生姓名和联系方式判断学生是否已存在
        /// </summary>
        /// <param name="Name">学生姓名</param>
        /// <param name="Contact">学生联系方式</param>
        /// <returns></returns>
        public bool determineStu(string Name, string Contact)
        {
            IQueryable<Student> temp = from u in db.Student where u.Name == Name && u.Contact == Contact && u.State == 0 select u;
            int pd= temp.Count();
            if(pd>0)
            {
                return true;
            }else
            {
                return false;
            }
            
        }
        /// <summary>
        /// 根据学生姓名判断是否已有该学生
        /// </summary>
        /// <param name="Name">学生名</param>
        /// <returns></returns>
        public List<Stu> determineStuByName(string Name)
        {
            List<Stu> studentdata = db.Database.SqlQuery<Stu>("select s.Id,s.Contact,s.Name,s.State,c.ClassName from Student s left join ClassRecord cr on s.Id =cr.StudentId left join Class c on c.Id=cr.ClassId  where cr.DepartureTime is null and  s.Name='" + Name + "' and s.State=0 ").ToList();
            return studentdata;
        }
        /// <summary>
        /// 查询学生当前所在班级
        /// </summary>
        /// <param name="stuid">学生id</param>
        /// <returns></returns>
        public List<nowClass> FindStuNowClass(int stuid)
        {
            var data = db.Database.SqlQuery<nowClass>("select c.ClassName,c.Id from Student s left join ClassRecord cr on s.Id =cr.StudentId left join Class c on c.Id=cr.ClassId  where cr.DepartureTime is null and s.id=" + stuid + "").ToList();
            return data;
        }
        /// <summary>
        /// 修改学生所在班级
        /// </summary>
        /// <param name="BeforeClassName">以前班级的编号</param>
        /// <param name="NewClassName">现在班级的编号</param>
        /// <param name="StuId">学生id</param>
        /// <returns></returns>
        public bool UpdateClass(int BeforeClassName, int NewClassName, int StuId)
        {
            try
            {
                //使用Extensions插件进行修改不用多查询一遍，直接返回受影响行数
                //修改之前的入住记录给它添加离开时间
                var Before = db.ClassRecord.Where(cr => cr.StudentId == StuId && cr.ClassId == BeforeClassName && cr.DepartureTime == null).FirstOrDefault();
                Before.DepartureTime = DateTime.Now;
               int cout=db.SaveChanges();
                if (cout > 0)
                {
                    //添加新的入班记录
                    ClassRecord cr = new ClassRecord();
                    cr.ClassId = NewClassName;
                    cr.CheckinTime = DateTime.Now;
                    cr.StudentId = StuId;
                    cr.DepartureTime = null;
                    db.ClassRecord.Add(cr);
                    db.SaveChanges();
                }
                return true;
            }
            catch( Exception e)
            {
                return false;
            }
            

        }
        /// <summary>
        /// 修改学生信息
        /// </summary>
        /// <param name="StuId">学生id</param>
        /// <param name="Name">学生姓名</param>
        /// <param name="Contact">联系方式</param>
        /// <returns></returns>
        public bool UpdateStuInfo(int StuId, string Name, string Contact)
        {
           var data = db.Student.Where(cr => cr.Id == StuId).FirstOrDefault();
            data.Name = Name;
            data.Contact = Contact;
            int cout = db.SaveChanges();
            if(cout>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据班级id查询出班级里所有的学生
        /// </summary>
        /// <param name="classId">班级id</param>
        /// <returns></returns>
        public List<nscc> FindStuByClassId(int Classid)
        {
            var data = db.Database.SqlQuery<nscc>("select  s.Id Id,s.Name Name,sr.CheckinTime srCheckinTime,d.Number Number,cr.CheckinTime crCheckinTime,c.ClassName from Student s left join StudentRecord sr on s.Id=sr.StudentId and sr.DepartureTime is null left join Dormitory d on sr.DormitoryInfoId=d.Id  left join ClassRecord cr on cr.StudentId =s.Id left join Class c on c.Id=cr.ClassId  where sr.DepartureTime is null and cr.DepartureTime is null  and c.Id = " + Classid + " group by sr.CheckinTime,cr.CheckinTime,c.ClassName, s.Name,d.Number,s.Id").ToList();
            return data;
        }
        /// <summary>
        /// 根据学生姓名查询学生信息
        /// </summary>
        /// <param name="StudentName">学生姓名</param>
        /// <returns></returns>
        public List<nscc> FindStuByStuName(string StudentName)
        {
            var data = db.Database.SqlQuery<nscc>("select s.Id Id,s.Name Name,sr.CheckinTime srCheckinTime,d.Number Number,cr.CheckinTime crCheckinTime,c.ClassName from Student s left join StudentRecord  sr on s.Id=sr.StudentId and sr.DepartureTime is null left join Dormitory d on sr.DormitoryInfoId=d.Id  left join ClassRecord cr on cr.StudentId =s.Id and cr.DepartureTime is null left join Class c on c.Id=cr.ClassId  where sr.DepartureTime is null and cr.DepartureTime is null and s.Name like'%" + StudentName + "%' group by sr.CheckinTime,cr.CheckinTime,c.ClassName, s.Name,d.Number,s.Id ").ToList();
            return data;
        }
        /// <summary>
        /// 根据班级id和学生姓名查询学生
        /// </summary>
        /// <param name="Classid">班级id</param>
        /// <param name="StudentName">学生姓名</param>
        /// <returns></returns>
        public List<nscc> FindStuByClassIdAndStuName(int Classid, string StudentName)
        {
            var data = db.Database.SqlQuery<nscc>("select s.Id Id,s.Name Name,sr.CheckinTime srCheckinTime,d.Number Number,cr.CheckinTime crCheckinTime,c.ClassName from Student s left join StudentRecord sr on s.Id=sr.StudentId and sr.DepartureTime is null left join Dormitory d on sr.DormitoryInfoId=d.Id  left join ClassRecord cr on cr.StudentId =s.Id and cr.DepartureTime is null left join Class c on c.Id=cr.ClassId  where sr.DepartureTime is null and cr.DepartureTime is null and s.Name like'%" + StudentName + "%' and c.Id = " + Classid + " group by sr.CheckinTime,cr.CheckinTime,c.ClassName, s.Name,d.Number,s.Id ").ToList();
            return data;
        }
        /// <summary>
        /// 查询所有班级的所以学生
        /// </summary>
        /// <returns></returns>
        public List<nscc> FindALLByno()
        {
            var data = db.Database.SqlQuery<nscc>("select s.Id Id,s.Name Name,sr.CheckinTime srCheckinTime,d.Number Number,cr.CheckinTime crCheckinTime,c.ClassName from Student s left join StudentRecord sr on s.Id=sr.StudentId and sr.DepartureTime is null left join Dormitory d on sr.DormitoryInfoId=d.Id  left join ClassRecord cr on cr.StudentId =s.Id and cr.DepartureTime is null left join Class c on c.Id=cr.ClassId  where  cr.DepartureTime is null  group by sr.CheckinTime,cr.CheckinTime,c.ClassName, s.Name,d.Number,s.Id ").ToList();
            return data;
        }
        /// <summary>
        /// 联想词功能
        /// </summary>
        /// <param name="Conditions">输入内容</param>
        /// <returns></returns>
        public object Lenovo(string Conditions)
        {
            var data = (from s in db.Student where s.Name.Contains(Conditions)select new { s.Name}).ToList().Take(5);
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
            public List<Stu> data { get; set; }
            public int count { get; set; }
        }

        public hd Serach(int page, string Value)
        {
            var data = new List<Stu>();
            var cout = new List<Stu>();
            string tiaojian1 = "";
            string tiaojian2 = "";
            string sql = "select s.Id,s.Contact,s.Name,s.State,c.ClassName from Student s left join ClassRecord cr on s.Id =cr.StudentId left join Class c on c.Id=cr.ClassId  where cr.DepartureTime is null";
            if (Value != "")
            {
               
                int i = 0;
                    var vs = Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (Value.Contains("在读"))
                {
                    tiaojian1 = " and s.State=0";
                }
                if (Value.Contains("已毕业"))
                {
                    tiaojian1 = " and s.State=1";
                }
                string temp = "";//用于保存拼接完整后的sql语句
                    string ls = "";//用于零时存储循环里的条件数据
                    for (i = 0; i < vs.Length; i++)
                    {
                    if (vs[i] == "在读" || vs[i] == "已毕业")
                    {
                        continue;
                    }
                        ls = tiaojian2;
                        tiaojian2 = " AND  (charindex('" + vs[i] + "',s.Name)>0 OR charindex(s.Name,'" + vs[i] + "')>0  OR charindex(s.Contact,'" + vs[i] + "' )>0 OR charindex('" + vs[i] + "',s.Contact )>0  OR   charindex(c.ClassName,'" + vs[i] + "')>0 OR charindex('" + vs[i] + "',c.ClassName)>0)";
                        tiaojian2 = ls + tiaojian2;
                    }
                    var s = tiaojian2;
                    temp = sql.Replace(sql, sql + tiaojian2+tiaojian1);
                    var a = temp;
                    data = db.Database.SqlQuery<Stu>(temp).ToPagedList(page, pagesize).ToList();
                    cout = db.Database.SqlQuery<Stu>(temp).ToList();
                
                return new hd() { data = data, count = cout.Count() };
            }
            else
            {
                data = db.Database.SqlQuery<Stu>(sql).ToPagedList(page, pagesize).ToList();
                cout = db.Database.SqlQuery<Stu>(sql).ToList();
                return new hd() { data = data, count = cout.Count() };
            }
        }
    }
}