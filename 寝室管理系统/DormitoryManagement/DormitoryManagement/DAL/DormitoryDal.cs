using DormitoryManagement.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DormitoryManagement.DAL
{
    public class DormitoryDal
    {
        DataModelContainer db = new DataModelContainer();
        StudentRecordDal Recorddal = new StudentRecordDal();
        int pagesize = 5;//每页数据条数
        public class Temp
        {
            public string Number { get; set; }//寝室编号
            public int Id { get; set; }
            public int? DormitoryInfoId { get; set; }//寝室Id
            public int? person { get; set; }//已有人数
            public int Capacity { get; set; }//可容纳人数
            public string StudentId { get; set; }//学生
            public int State { get; set; }//状态
            public string FloorName { get; set; }//楼号
            public string LayerNumber { get; set; }//层号
            public int FloorState { get; set; }//寝室所在楼的状态
        }
        /// <summary>
        /// 查询是否已经有该寝室
        /// </summary>
        /// <param name="Number">寝室编号</param>
        /// <param name="Floor">楼号</param>
        /// <param name="Layer">层号</param>
        /// <returns></returns>
        public bool determineDormitory(string Number, int Floor, int Layer)
        {
            var temp = (from d in db.Dormitory
                        join dl in db.DetailsLayer on d.DetailsLayerId equals dl.Id
                        join df in db.DetailsFloor on dl.DetailsFloorId equals df.Id
                        where d.Number == Number && dl.Id == Layer && df.Id == Floor
                        select new { d.Number }).ToList();
            int pd = temp.Count();
            if (pd > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 查询所有寝室
        /// </summary>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public List<Temp> FindALL(int page)
        {
            var data = db.Database.SqlQuery<Temp>("select df.FloorName, dl.LayerNumber, d.Number, d.State, d.Capacity, d.Id, t.DormitoryInfoId, count(StudentId) person, StudentId = stuff((select ',' + rtrim(s.Name) from StudentRecord sr inner join Student s on s.id = sr.StudentId where t.DormitoryInfoId = sr.DormitoryInfoId and sr.DepartureTime is null order by sr.StudentId for xml path('')),1,1,'')  from StudentRecord t right join Dormitory d on d.id = t.DormitoryInfoId  and t.DepartureTime is null left join DetailsLayer dl on d.DetailsLayerId = dl.Id left join DetailsFloor df on dl.DetailsFloorId = df.Id group by DormitoryInfoId ,d.Number ,d.State,d.Id,d.Capacity,dl.LayerNumber,df.FloorName").ToPagedList(page, pagesize).ToList();
            return data;
        }
        /// <summary>
        /// 查询寝室已有学生
        /// </summary>
        /// <param name="DormitoryId">寝室id</param>
        /// <returns></returns>
        public object FindStu(int DormitoryId)
        {
            var data = (from a in db.StudentRecord
                        join b in db.Student on a.StudentId equals b.Id
                        join c in db.Dormitory on a.DormitoryInfoId equals c.Id
                        where c.Id == DormitoryId && a.DepartureTime == null
                        select new { b.Id, b.Name, b.Contact });
            return data;
        }
        /// <summary>
        /// 根据学生姓名查询学生信息
        /// </summary>
        /// <param name="Name">学生名</param>
        /// <returns></returns>
        public bool SelectStu(string Name)
        {
            IQueryable<Student> temp = from u in db.Student where u.Name == Name select u;
            int pd = temp.Count();
            if (pd > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据寝室编号，楼号，层号判断该寝室是否已存在
        /// </summary>
        /// <param name="DNumber">寝室编号</param>
        /// <param name="FloorName">楼号</param>
        /// <param name="LayerNumber">层号</param>
        /// <returns></returns>
        public bool determineDormitoryByDfandDlandDNumber(string DNumber, string FloorName, string LayerNumber)
        {
            List<Temp> DormitoryInfodata = db.Database.SqlQuery<Temp>("select df.FloorName, dl.LayerNumber, d.Number, d.State, d.Capacity, d.Id, t.DormitoryInfoId, count(StudentId) person, StudentId = stuff((select ',' + rtrim(s.Name) from StudentRecord sr inner join Student s on s.id = sr.StudentId where t.DormitoryInfoId = sr.DormitoryInfoId and sr.DepartureTime is null order by sr.StudentId for xml path('')),1,1,'')  from StudentRecord t right join Dormitory d on d.id = t.DormitoryInfoId  and t.DepartureTime is null left join DetailsLayer dl on d.DetailsLayerId = dl.Id left join DetailsFloor df on dl.DetailsFloorId = df.Id where d.Number='" + DNumber + "'and df.FloorName='" + FloorName + "' and dl.LayerNumber='" + LayerNumber + "' group by DormitoryInfoId ,d.Number ,d.State,d.Id,d.Capacity,dl.LayerNumber,df.FloorName").ToList();
            if (DormitoryInfodata.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 更具楼号、层号、寝室号获取该楼该层该寝室的相关信息
        /// </summary>
        /// <param name="DNumber">寝室编号</param>
        /// <param name="FloorName">楼号</param>
        /// <param name="LayerNumber">层号</param>
        /// <returns></returns>
        public List<Temp> FindByDfandDlandDNumber(string DNumber, string FloorName, string LayerNumber)
        {
            List<Temp> DormitoryInfodata = db.Database.SqlQuery<Temp>("select df.FloorName, dl.LayerNumber, d.Number, d.State, d.Capacity, d.Id, t.DormitoryInfoId, count(StudentId) person, StudentId = stuff((select ',' + rtrim(s.Name) from StudentRecord sr inner join Student s on s.id = sr.StudentId where t.DormitoryInfoId = sr.DormitoryInfoId and sr.DepartureTime is null order by sr.StudentId for xml path('')),1,1,'')  from StudentRecord t right join Dormitory d on d.id = t.DormitoryInfoId  and t.DepartureTime is null left join DetailsLayer dl on d.DetailsLayerId = dl.Id left join DetailsFloor df on dl.DetailsFloorId = df.Id where d.Number='" + DNumber + "'and df.FloorName='" + FloorName + "' and dl.LayerNumber='" + LayerNumber + "' group by DormitoryInfoId ,d.Number ,d.State,d.Id,d.Capacity,dl.LayerNumber,df.FloorName").ToList();
            return DormitoryInfodata;
        }
        /// <summary>
        ///修改寝室状态
        /// </summary>
        /// <param name="id">寝室id</param>
        /// <param name="state">目标状态</param>
        /// <returns></returns>
        public bool UpdateState(int id, int state)
        {
            DormitoryInfo temp = db.Dormitory.Where(c => c.Id == id).FirstOrDefault();
            temp.State = state;
            var cout = db.SaveChanges();
            return cout > 0;
        }
        /// <summary>
        /// 根据寝室id修改寝室的状态和可容纳人数
        /// </summary>
        /// <param name="Dormitoryid">寝室id</param>
        /// <param name="DCapacity">可容纳人数</param>
        /// <returns></returns>
        public bool UpdateStateAndCapacity(int Dormitoryid, int DCapacity)
        {
            DormitoryInfo info = db.Dormitory.Where(d => d.Id == Dormitoryid).FirstOrDefault();
            info.State = 0;//将寝室状态改为启用状态
            info.Capacity = DCapacity;//把寝室可容纳人数改为现在导入的值
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
        /// 根据寝室id和学生id判断改其实是否已有这个学生
        /// </summary>
        /// <param name="did">寝室id</param>
        /// <param name="Name">学生姓名</param>
        /// <returns></returns>
        public bool DetermineStuByDNumberAndStuName(int did, string Name)
        {
            var data = (from a in db.StudentRecord
                        join b in db.Student on a.StudentId equals b.Id
                        join c in db.Dormitory on a.DormitoryInfoId equals c.Id
                        where c.Id == did && a.DepartureTime == null && b.Name == Name
                        select new { b.Name, b.Contact }).ToList();
            if (data.Count() > 0)
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
            var data = (from s in db.Dormitory where s.Number.Contains(Conditions) select new { s.Number}).ToList().Take(5);
            return data;
        }
        /// <summary>
        /// 添加寝室
        /// </summary>
        /// <param name="DNumber">寝室编号</param>
        /// <param name="DCapacity">寝室可容纳人数</param>
        /// <returns></returns>
        public bool AddDormitory(string DNumber, int DCapacity, int LayerId)
        {
            DormitoryInfo dormitoryInfo = new DormitoryInfo();
            dormitoryInfo.Number = DNumber;
            dormitoryInfo.State = 0;
            dormitoryInfo.Capacity = DCapacity;
            dormitoryInfo.Already = 0;
            dormitoryInfo.CreationTime = DateTime.Now;
            dormitoryInfo.DetailsLayerId = LayerId;
            db.Dormitory.Add(dormitoryInfo);
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
        ///根据层id查询该层的寝室
        /// </summary>
        /// <param name="LayerId">层id</param>
        /// <returns></returns>
        public List<DormitoryInfo> FindDormitoryByLayer(int LayerId)
        {
            List<DormitoryInfo> data = db.Database.SqlQuery<DormitoryInfo>(" select* from Dormitory d where d.DetailsLayerId = " + LayerId + "").ToList();
            return data;
        }
        /// <summary>
        ///清空寝室现有所有学生 
        /// </summary>
        /// <param name="id">寝室id</param>
        /// <returns></returns>
        public bool ResetDormitoryByid(int id)
        {
            //得到在当前该寝室的学生入住记录
            List<StudentRecord> temp = Recorddal.FindStuByDId(id);
            for (int i = 0; i < temp.ToArray().Length; i++)
            {
                //修改其离开时间为当前时间
                temp[i].DepartureTime = DateTime.Now;
                db.Entry(temp[i]).State = EntityState.Modified;
            }
            int cout = db.SaveChanges();
            return cout > 0;
        }
        /// <summary>
        /// 根据id判断寝室是否满了
        /// </summary>
        /// <param name="id">寝室id</param>
        /// <returns></returns>
        public bool Determine_Capactiy(int id)
        {
            var data = db.Database.SqlQuery<Temp>("  select d.Number, d.State, d.Capacity, d.Id, count(StudentId) person, StudentId = stuff((select ',' + rtrim(s.Name) from StudentRecord sr inner join Student s on s.id = sr.StudentId where t.DormitoryInfoId = sr.DormitoryInfoId and sr.DepartureTime is null order by sr.StudentId for xml path('')),1,1,'')  from StudentRecord t right join Dormitory d on d.id = t.DormitoryInfoId  and t.DepartureTime is null where d.Id=" + id + "  group by DormitoryInfoId ,d.Number ,d.State,d.Id,d.Capacity").ToList();
            int Capacity = data.FirstOrDefault().Capacity;
            int? person = data.FirstOrDefault().person;
            if (person == null)
            {
                return true;
            }
            else
            {
                return Capacity > person;
            }
        }
        public class hd
        {
            public List<Temp> data { get; set; }
            public int count { get; set; }
        }
        //搜索
        public hd Serach(int page, string Value)
        {
            var data = new List<Temp>();
            var cout = new List<Temp>();
            string tiaojian1 = "";
            string tiaojian2 = "";
            string sname = "";
            string sql = "select df.FloorName,df.State FloorState, dl.LayerNumber, d.Number, d.State, d.Capacity, d.Id, t.DormitoryInfoId, count(StudentId) person, StudentId = stuff((select ',' + rtrim(s.Name) from StudentRecord sr inner join Student s on s.id = sr.StudentId where t.DormitoryInfoId = sr.DormitoryInfoId and sr.DepartureTime is null order by sr.StudentId for xml path('')),1,1,'')  from StudentRecord t right join Dormitory d on d.id = t.DormitoryInfoId  and t.DepartureTime is null left join DetailsLayer dl on d.DetailsLayerId = dl.Id left join DetailsFloor df on dl.DetailsFloorId = df.Id  left join Student s on t.StudentId=s.Id " + sname+ "  group by DormitoryInfoId ,d.Number ,d.State,d.Id,d.Capacity,dl.LayerNumber,df.FloorName,df.State having 1=1  ";
            if (Value != "")
            {

                int i = 0;
                var vs = Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (Value.Contains("启用"))
                {
                    tiaojian1 = " and s.State=0";
                }
                if (Value.Contains("停用"))
                {
                    tiaojian1 = " and s.State=1";
                }
               if(Value.Contains("已满"))
                {
                    tiaojian1 = " and COUNT(StudentId)=d.Capacity";
                }
                string temp = "";//用于存储拼接好的sql
                string ls = "";//用于零时存储循环里的条件数据
                for (i = 0; i < vs.Length; i++)
                {
                    if (vs[i] == "启用" || vs[i] == "停用" || vs[i] == "已满")
                    {
                        continue;
                    }
                    ls = tiaojian2;
                    sname = "where charindex('"+ vs[i] + "', s.Name) > 0";
                    tiaojian2 = " and (charindex('" + vs[i] + "',FloorName)>0 OR charindex(FloorName,'" + vs[i] + "')>0  OR charindex(Number,'" + vs[i] + "' )>0 OR charindex('" + vs[i] + "',Number )>0  OR   charindex(LayerNumber,'" + vs[i] + "')>0 OR charindex('" + vs[i] + "',LayerNumber)>0)";
                    tiaojian2 = ls + tiaojian2;
                }
                temp = temp = sql.Replace(sql, sql + tiaojian2 + tiaojian1);
                var a = temp;
                data = db.Database.SqlQuery<Temp>(temp).ToPagedList(page, pagesize).ToList();
                cout = db.Database.SqlQuery<Temp>(temp).ToList();

                return new hd() { data = data, count = cout.Count() };

            }
            else
            {
                data = db.Database.SqlQuery<Temp>(sql).ToPagedList(page, pagesize).ToList();
                cout = db.Database.SqlQuery<Temp>(sql).ToList();
             
                return new hd() { data = data, count = cout.Count() };
            }
        }
    }
}