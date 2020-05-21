using DormitoryManagement.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DormitoryManagement.DAL
{
    public class FloorDal
    {
        DataModelContainer db = new DataModelContainer();
        LayerDal Layerdal = new LayerDal();
        DormitoryDal Dormitorydal = new DormitoryDal();
        int pagesize = 5;//每页数据条数

        public class Floor
        {
            public int Id { get; set; }
            public string FloorNumber { get; set; }
            public string LayerNumber { get; set; }
            public int DormitoryNumber { get; set; }
            public int State { get; set; }
        }
        /// <summary>
        /// 查询所有楼
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Floor> FindAll(int page)
        {
            var data = db.Database.SqlQuery<Floor>("select df.Id,df.FloorName FloorNumber,df.State,COUNT(d.DetailsLayerId) DormitoryNumber ,LayerNumber=stuff((select ','+rtrim(dl.LayerNumber) from DetailsLayer dl where dl.DetailsFloorId=df.Id order by df.Id for xml path('')),1,1,'') from DetailsFloor df left join DetailsLayer dl on dl.DetailsFloorId=df.Id left join Dormitory d  on d.DetailsLayerId=dl.Id  group by df.Id,df.State,df.FloorName").ToPagedList(page, pagesize).ToList() ;
            return data;
        } /// <summary>
          /// 搜索
          /// </summary>
          /// <param name="page">页码</param>
          ///  /// <param name="Value">搜索框内的值</param>
          /// <returns></returns>
        public class hd
        {
            public List<Floor> data { get; set; }
            public int count { get; set; }
        }

        public hd Serach(int page, string Value)
        {
            var data = new List<Floor>();
            var cout = new List<Floor>();
            string tiaojian1 = "";
            string tiaojian2 = "";
            string tj = "";
            string sql = "select df.Id,df.FloorName FloorNumber,df.State,COUNT(d.DetailsLayerId) DormitoryNumber ,LayerNumber=stuff((select ','+rtrim(dl.LayerNumber) from DetailsLayer dl where dl.DetailsFloorId=df.Id order by df.Id for xml path('')),1,1,'') from DetailsFloor df left join DetailsLayer dl on dl.DetailsFloorId=df.Id left join Dormitory d  on d.DetailsLayerId=dl.Id where 1=1 "+tj+" group by df.Id,df.State,df.FloorName";
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
                string ls = "";//用于零时存储循环里的条件数据
                for (i = 0; i < vs.Length; i++)
                {
                    if (vs[i] == "启用" || vs[i] == "停用")
                    {
                        continue;
                    }
                    ls = tiaojian2;
                    tiaojian2 = " AND  (charindex('" + vs[i] + "',df.FloorName)>0 OR charindex(df.FloorName,'" + vs[i] + "')>0  OR charindex(dl.LayerNumber,'" + vs[i] + "' )>0 OR charindex('" + vs[i] + "',dl.LayerNumber )>0)";
                    tiaojian2 = ls + tiaojian2;
                }
                tj = tiaojian2 + tiaojian1;
                sql = "select df.Id,df.FloorName FloorNumber,df.State,COUNT(d.DetailsLayerId) DormitoryNumber ,LayerNumber=stuff((select ','+rtrim(dl.LayerNumber) from DetailsLayer dl where dl.DetailsFloorId=df.Id order by df.Id for xml path('')),1,1,'') from DetailsFloor df left join DetailsLayer dl on dl.DetailsFloorId=df.Id left join Dormitory d  on d.DetailsLayerId=dl.Id where 1=1 " + tj + " group by df.Id,df.State,df.FloorName";
                data = db.Database.SqlQuery<Floor>(sql).ToPagedList(page, pagesize).ToList();
                cout = db.Database.SqlQuery<Floor>(sql).ToList();

                return new hd() { data = data, count = cout.Count() };
            }
            else
            {
                data = db.Database.SqlQuery<Floor>(sql).ToPagedList(page, pagesize).ToList();
                cout = db.Database.SqlQuery<Floor>(sql).ToList();
                return new hd() { data = data, count = cout.Count() };
            }
        }
        /// <summary>
        /// 查询所有可用的楼
        /// </summary>
        /// <returns></returns>
        public object FindAll()
        {
            var data = (from u in db.DetailsFloor select new { u.FloorName, u.Id,u.State }).ToList();
            return data;

        }
        /// <summary>
        /// 根据楼好判断楼是否已存在
        /// </summary>
        /// <param name="FloorName"></param>
        /// <returns></returns>
        public bool determineFloor(string FloorName)
        {
            List<Floor> Floordata = db.Database.SqlQuery<Floor>("select df.Id ,df.FloorName FloorNumber,df.State,COUNT(dl.Id) LayerNumber ,COUNT(d.DetailsLayerId) DormitoryNumber from DetailsFloor df left join DetailsLayer dl on dl.DetailsFloorId=df.Id left join Dormitory d on d.DetailsLayerId=dl.Id where df.FloorName='" + FloorName + "' group by df.FloorName,df.State,df.Id ").ToList();
            if (Floordata.Count()>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据楼号查询楼的相关信息
        /// </summary>
        /// <param name="FloorName">楼名</param>
        /// <returns></returns>
        public List<Floor> FindFloorByName(string FloorName)
        {
            List<Floor> Floordata = db.Database.SqlQuery<Floor>("select df.Id ,df.FloorName FloorNumber,df.State,COUNT(dl.Id) LayerNumber ,COUNT(d.DetailsLayerId) DormitoryNumber from DetailsFloor df left join DetailsLayer dl on dl.DetailsFloorId=df.Id left join Dormitory d on d.DetailsLayerId=dl.Id where df.FloorName='" + FloorName + "' group by df.FloorName,df.State,df.Id ").ToList();
            return Floordata;
        }
        /// <summary>
        /// 根据id修改楼的状态
        /// </summary>
        /// <param name="FloorID"></param>
        /// <returns></returns>
        public bool UpdateStateById(int FloorID)
        {
            DetailsFloor temp = db.DetailsFloor.Where(c => c.Id == FloorID).FirstOrDefault();
            temp.State = 0;//将楼改为启用状态
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
        /// 根据id和当前状态修改状态
        /// </summary>
        /// <param name="id">楼ID</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public bool UpdateStateById(int id, int state)
        {
            DetailsFloor temp = db.DetailsFloor.Where(c => c.Id == id).FirstOrDefault();
            temp.State = state;
            var cout = db.SaveChanges();
            if(cout>0)//楼状态修改成功
            {
                //获取该楼内所有层
                List<DetailsLayer> layers =Layerdal.FindLayerByDF(id);
                for(int i=0;i<layers.ToArray().Length; i++)
                {
                    //获取层id
                    var layerId=layers[i].Id;
                    //跟据层查询到与该层相关的寝室
                  List<DormitoryInfo> dormitoryInfos=Dormitorydal.FindDormitoryByLayer(layerId);
                    for(int j=0;j< dormitoryInfos.ToArray().Length;j++)
                    {
                        int Did = dormitoryInfos[j].Id;
                        //移除该寝室的所有学生
                        Dormitorydal.ResetDormitoryByid(Did);
                        //修改寝室状态
                        Dormitorydal.UpdateState(Did,state);
                    }
                }
            }
            return cout > 0;
        }
            /// <summary>
            /// 添加楼
            /// </summary>
            /// <param name="FloorName">楼号</param>
            /// <param name="FState">状态</param>
            /// <returns></returns>
            public bool AddFloor(string FloorName,int FState)
        {
            DetailsFloor detailsFloor = new DetailsFloor();
            detailsFloor.FloorName = FloorName;
            detailsFloor.State = FState;
            db.DetailsFloor.Add(detailsFloor);
            int  pd = db.SaveChanges();
            if(pd>0)
            {
                return true;

            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据楼号判断该楼状态
        /// </summary>
        /// <param name="Number">楼号</param>
        /// <returns></returns>
        public bool FindStateByNumber(string Number)
        {
            List<Floor> data = db.Database.SqlQuery<Floor>("select * from DetailsFloor df where df.FloorName='"+Number+"'").ToList();
           int state= data.FirstOrDefault().State;
            if(state==0)
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
            var data = (from c in db.DetailsFloor where c.FloorName.Contains(Conditions) select new { c.FloorName }).ToList().Take(5);
            return data;
        }

    }
}