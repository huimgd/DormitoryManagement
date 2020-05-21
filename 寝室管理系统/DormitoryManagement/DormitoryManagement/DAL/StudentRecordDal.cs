using DormitoryManagement.Models;
using EntityFramework.Extensions;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DormitoryManagement.DAL
{
    public class StudentRecordDal
    {
        DataModelContainer db = new DataModelContainer();
        int pagesize = 5;
        /// <summary>
        /// 根据学生id修该他当前所在寝室的离开时间
        /// </summary>
        /// <param name="StuId">学生id</param>
        public void UpdateRecordByStuId(int StuId)
        {
            int jilu = db.StudentRecord.Where(dsr => dsr.StudentId == StuId && dsr.DepartureTime == null).Update(b => new StudentRecord { DepartureTime = DateTime.Now });
            db.SaveChanges();
        }
        /// <summary>
        /// 根据学生id和寝室id添加一条新的入住记录
        /// </summary>
        /// <param name="DId">寝室id</param>
        /// <param name="StuId">学生id</param>
        /// <returns></returns>
        public bool AddRecordByDIdAndStuId(int DId,int StuId)
        {
            StudentRecord sr = new StudentRecord();
            sr.DormitoryInfoId = DId;
            sr.CheckinTime = DateTime.Now;
            sr.StudentId = StuId;
            sr.DepartureTime = null;
            db.StudentRecord.Add(sr);
            int cout=db.SaveChanges();
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
        /// 查询所有入住记录
        /// </summary>
        /// <returns></returns>
        public object FindAll()
        {
            var data = (from a in db.StudentRecord.OrderByDescending(p => p.Id)
                        join b in db.Student on a.StudentId equals b.Id
                        join c in db.Dormitory on a.DormitoryInfoId equals c.Id
                        join d in db.ClassRecord on b.Id equals d.StudentId 
                        join e in db.Class on d.ClassId equals e.Id
                        where d.DepartureTime==null
                        select new { b.Name, b.Contact, c.Number,
                            a.CheckinTime, a.DepartureTime ,e.ClassName}).ToList();
            return data;
        }
      public  class srmodel{
            public string Name { get;  set; }
            public string  Contact { get; set; }
            public string  Number { get; set; }
            public string ClassName { get; set; }
            public DateTime CheckinTime { get; set; }
            public DateTime? DepartureTime { get; set; }
        }
        /// <summary>
        ///搜索
        /// </summary>
        public class hd
        {
            public List<srmodel> data { get; set; }
            public int cout { get; set; }
        }
        public hd Serach( string Value,int page)
        {
            var data = new List<srmodel>();
            var cout = new List<srmodel>();
            string tiaojian1 = "";
            string tiaojian2 = "";
            string tj = "";
            if (Value != "")
            {

                int i = 0;
                var vs = Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (Value.Contains("未搬离"))
                {
                    tiaojian1 = " and sr.DepartureTime is null";
                }
                if (Value.Contains("已搬离"))
                {
                    tiaojian1 = " and sr.DepartureTime is not null";
                }
                string ls = "";//用于零时存储循环里的条件数据
                for (i = 0; i < vs.Length; i++)
                {
                    if (vs[i] == "已搬离" || vs[i] == "未搬离")
                    {
                        continue;
                    }
                    ls = tiaojian2;
                    tiaojian2 = " AND  (charindex('" + vs[i] + "',s.Name)>0 OR charindex(s.Name,'" + vs[i] + "')>0  OR charindex(s.Contact,'" + vs[i] + "' )>0 OR charindex('" + vs[i] + "',s.Contact )>0  OR   charindex(d.Number,'" + vs[i] + "')>0 OR charindex('" + vs[i] + "',d.Number)>0   OR   charindex(c.ClassName,'" + vs[i] + "')>0 OR charindex('" + vs[i] + "',c.ClassName)>0  )";
                    tiaojian2 = ls + tiaojian2;
                }
                tj = tiaojian2 + tiaojian1;
                string sql = "select s.Name,s.Contact,d.Number,sr.CheckinTime,sr.DepartureTime,c.ClassName from  StudentRecord sr left join Student s on sr.StudentId =s.Id left join ClassRecord cr on cr.StudentId =s.Id and cr.DepartureTime is null and cr.CheckinTime is not null left join Class c on cr.ClassId =c.Id left join Dormitory d on sr.DormitoryInfoId=d.Id where 1=1 " + tj + "  group by sr.DepartureTime,s.Name,s.Contact,d.Number,sr.CheckinTime,c.ClassName";
                data = db.Database.SqlQuery<srmodel>(sql).ToPagedList(page, pagesize).ToList();
                cout = db.Database.SqlQuery<srmodel>(sql).ToList();
                return new hd() { data = data,cout= cout.Count()};
            }
            else
            {
                string sql = "select s.Name,s.Contact,d.Number,sr.CheckinTime,sr.DepartureTime,c.ClassName from  StudentRecord sr left join Student s on sr.StudentId =s.Id left join ClassRecord cr on cr.StudentId =s.Id and cr.DepartureTime is null and cr.CheckinTime is not null left join Class c on cr.ClassId =c.Id left join Dormitory d on sr.DormitoryInfoId=d.Id  group by sr.DepartureTime,s.Name,s.Contact,d.Number,sr.CheckinTime,c.ClassName";
                data = db.Database.SqlQuery<srmodel>(sql).ToPagedList(page, pagesize).ToList();
                cout = db.Database.SqlQuery<srmodel>(sql).ToList();
                return new hd() { data = data, cout = cout.Count() };
            }
        }
        /// <summary>
        /// 根据寝室id查询该寝室所有的入住记录
        /// </summary>
        /// <param name="Did">寝室id</param>
        /// <returns></returns>
        public List<StudentRecord> FindRecordByDId(int Did)
        {
            List<StudentRecord> temp = db.Database.SqlQuery<StudentRecord>("select * from StudentRecord sr where sr.DormitoryInfoId=" + Did + "").ToList();
            return temp;
        }
        /// <summary>
        /// 根据寝室id查询当前寝室的学生入住记录
        /// </summary>
        /// <param name="Did">寝室id</param>
        /// <returns></returns>
        public List<StudentRecord> FindStuByDId(int Did)
        {
            List<StudentRecord> temp = db.Database.SqlQuery<StudentRecord>("select * from StudentRecord sr where sr.DormitoryInfoId="+ Did + " and sr.DepartureTime is null").ToList();
            return temp;
        }
        /// <summary>
        /// 根据学生id查询学生入住记录
        /// </summary>
        /// <param name="Stuid">学生id</param>
        /// <returns></returns>
        public List<StudentRecord> FindByStuId(int Stuid)
        {
            List<StudentRecord> data = db.Database.SqlQuery<StudentRecord>("select * from StudentRecord sr join Student s on sr.StudentId=s.Id where s.Id="+ Stuid + " order by sr.id desc ").ToList();
            return data;
        }
        /// <summary>
        /// 联想词功能
        /// </summary>
        /// <param name="Conditions">输入内容</param>
        /// <returns></returns>
        public object Lenovo(string Conditions)
        {
            var data = (from s in db.Student where s.Name.Contains(Conditions) select new { s.Name }).ToList().Take(5);
            return data;
        }
    }
}