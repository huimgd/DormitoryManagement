using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DormitoryManagement.DAL
{
    public class LayerDal
    {
        DataModelContainer db = new DataModelContainer();
        int pagesize = 2;//每页数据条数
        public class Layer
        {
            public int Id { get; set; }
            public string LayerNumber { get; set; }
        }
        /// <summary>
        /// 根据楼Id查询该楼的层
        /// </summary>
        /// <param name="FloorId">楼ID</param>
        /// <returns></returns>
        public List<DetailsLayer> FindLayerByDF(int FloorId)
        {
            List<DetailsLayer> data = (from dl in db.DetailsLayer where dl.DetailsFloorId == FloorId select dl).ToList();
            return data;
        }
        /// <summary>
        /// 根据楼号和层号判断是否已存在该层
        /// </summary>
        /// <param name="FloorName">楼号</param>
        /// <param name="LayerNumber">层号</param>
        /// <returns></returns>
        public bool determineLayerByFloorNameandLayerNumber(string FloorName,string LayerNumber)
        {
            List<Layer> Layerdata = db.Database.SqlQuery<Layer>("select dl.LayerNumber,dl.Id from DetailsLayer dl join DetailsFloor df on dl.DetailsFloorId=df.Id where df.FloorName='" + FloorName + "'and dl.LayerNumber='" + LayerNumber + "'").ToList();
            if(Layerdata.Count()>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 添加层
        /// </summary>
        /// <param name="LayerNumber">层号</param>
        /// <param name="FloorId">该层所在楼的id</param>
        /// <returns></returns>
        public bool AddLayer(string LayerNumber,int FloorId)
        {
            DetailsLayer dl = new DetailsLayer();
            dl.LayerNumber = LayerNumber;
            dl.DetailsFloorId = FloorId;
           db.DetailsLayer.Add(dl);
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
        /// 根据楼号和层号，返回该曾的id
        /// </summary>
        /// <param name="FloorName">楼编号</param>
        /// <param name="LayerNumber">层编号</param>
        /// <returns></returns>
        public List<Layer> FindLayerId( string FloorName,string LayerNumber)
        {
            List<Layer> Layer = db.Database.SqlQuery<Layer>("select dl.LayerNumber,dl.Id from DetailsFloor df join DetailsLayer dl on dl.DetailsFloorId=df.Id  where df.FloorName='" + FloorName + "' and dl.LayerNumber=" + LayerNumber + "").ToList();
            return Layer;
        }
    }
}