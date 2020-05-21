using DormitoryManagement.Models;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DormitoryManagement.DAL;

namespace DormitoryManagement.Controllers
{
    public class FloorController : Controller
    {
        FloorDal Floordal = new FloorDal();
        ClassDal Classdal = new ClassDal();
        DataModelContainer db = new DataModelContainer();
        int pagesize = 5;//每页展示数据条数
        // GET: Floor
        public ActionResult Index()
        {
            IQueryable<DetailsFloor> temp = from u in db.DetailsFloor select u;
            int page = temp.Count() / pagesize;
            if (temp.Count() % pagesize != 0)
            {
                page = page + 1;
            }
            ViewData["Message"] = page;
            return View();
        }
        class Floor
        {
            public string FloorNumber { get; set; }
            public string LayerNumber { get; set; }
            public int DormitoryNumber { get; set; }
            public int State { get; set; }
        }
        //查询所有的楼
        public JsonResult FindAllFloor(int page=1)
        {
           JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(Floordal.FindAll(page), setting);
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
            var data = Floordal.Serach(page, Value);
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
            var a = pg;
            return Json(new { ret = ret, pg = pg }, JsonRequestBehavior.AllowGet);
        }
        //联想词功能
        [HttpGet]
        public JsonResult Lenovo(String Value)
        {
            var data = Floordal.Lenovo(Value);
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
           
            return Floordal.UpdateStateById(id,state);
        }
        //判断数据库是否已经有了这个班级
        public bool AddSelect(string Number)
        {
            try
            {
                
                return Classdal.determineClass(Number);
            }
            catch (Exception)
            {
                return false;
            }

        }
        //添加楼和层
        public bool Add(string FloorNumber,int [] Layer)
        {
            try
            {
                DetailsFloor df = new DetailsFloor();
                df.FloorName = FloorNumber;
                db.DetailsFloor.Add(df);
                db.SaveChanges();
                var DF = db.DetailsFloor.Select(s => s.Id).Max();
                int cout = 0;//用于保存受影响行数
                DetailsLayer dl = new DetailsLayer();
                for(int i=0;i<Layer.Length;i++)
                {
                    dl.LayerNumber = Layer[i].ToString();
                    dl.DetailsFloorId = DF;
                    db.DetailsLayer.Add(dl);
                    cout = db.SaveChanges();
                }
                if (cout > 0)
                    return true;
                else
                    return false;
            }catch(Exception e)
            {
                return false;
            }
            
        }
    }
}