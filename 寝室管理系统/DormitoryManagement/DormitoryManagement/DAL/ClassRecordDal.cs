using DormitoryManagement.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DormitoryManagement.DAL
{
    public class ClassRecordDal
    {
        DataModelContainer db = new DataModelContainer();
        /// <summary>
        /// 查询所有入班记录
        /// </summary>
        /// <returns></returns>
        public object FindAll()
        {
            var data = (from a in db.ClassRecord.OrderByDescending(p => p.Id) join b in db.Student on a.StudentId equals b.Id join c in db.Class on a.ClassId equals c.Id select new { b.Name, b.Contact, c.ClassName, a.CheckinTime, a.DepartureTime }).ToList();
            return data;
        }
        public class crmodel
        {
            public string Name { get; set; }
            public string Contact { get; set; }
            public string ClassName { get; set; }
            public DateTime CheckinTime { get; set; }
            public DateTime? DepartureTime { get; set; }
        }
        /// <summary>
        ///搜索
        /// </summary>
        public class hd
        {
            public List<crmodel> data { get; set; }
            public int cout { get; set; }
        }
        public hd Serach(string Value,int page,int pagesize)
        {
            var data = new List<crmodel>();
            var cout = new List<crmodel>();
            string tiaojian1 = "";
            string tiaojian2 = "";
            string tj = "";
            if (Value != "")
            {

                int i = 0;
                var vs = Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (Value.Contains("未更改"))
                {
                    tiaojian1 = " and cr.DepartureTime is null";
                }
                if (Value.Contains("已更改"))
                {
                    tiaojian1 = " and cr.DepartureTime is not null";
                }
                string ls = "";//用于零时存储循环里的条件数据
                for (i = 0; i < vs.Length; i++)
                {
                    if (vs[i] == "已搬离" || vs[i] == "未搬离")
                    {
                        continue;
                    }
                    ls = tiaojian2;
                    tiaojian2 = " AND  (charindex('" + vs[i] + "',s.Name)>0 OR charindex(s.Name,'" + vs[i] + "')>0  OR charindex(s.Contact,'" + vs[i] + "' )>0 OR charindex('" + vs[i] + "',s.Contact )>0  OR   charindex(c.ClassName,'" + vs[i] + "')>0 OR charindex('" + vs[i] + "',c.ClassName)>0  )";
                    tiaojian2 = ls + tiaojian2;
                }
                tj = tiaojian2 + tiaojian1;
                string sql = "select s.Name,s.Contact,c.ClassName ,cr.CheckinTime,cr.DepartureTime from ClassRecord cr left join Student s on cr.StudentId=s.Id left join Class c on cr.ClassId=c.Id  where 1=1 "+tj+" group by cr.DepartureTime,s.Name,s.Contact,c.ClassName ,cr.CheckinTime";
                data = db.Database.SqlQuery<crmodel>(sql).ToPagedList(page, pagesize).ToList();
                cout = db.Database.SqlQuery<crmodel>(sql).ToList();

                return new hd() { data = data ,cout=cout.Count()};
            }
            else
            {
                string sql = "select s.Name,s.Contact,c.ClassName ,cr.CheckinTime,cr.DepartureTime from ClassRecord cr left join Student s on cr.StudentId=s.Id left join Class c on cr.ClassId=c.Id  group by cr.DepartureTime,s.Name,s.Contact,c.ClassName ,cr.CheckinTime";
                data = db.Database.SqlQuery<crmodel>(sql).ToPagedList(page, pagesize).ToList();
                cout = db.Database.SqlQuery<crmodel>(sql).ToList();
                return new hd() { data = data,cout = cout.Count() };
            }
        }
        /// <summary>
        /// 根据学生id查询学生入班记录
        /// </summary>
        /// <param name="Stuid">学生id</param>
        /// <returns></returns>
        public List<ClassRecord> FindByStuId(int Stuid)
        {
            List<ClassRecord> data = db.Database.SqlQuery<ClassRecord>("select * from ClassRecord cr join Student s on cr.StudentId=s.Id where s.Id="+ Stuid + " order by cr.id desc ").ToList();
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