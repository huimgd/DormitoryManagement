using DormitoryManagement.DAL;
using DormitoryManagement.Models;
using Newtonsoft.Json;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DormitoryManagement.Controllers
{
    public class StudentRecordController : Controller
    {
        ClassDal Classdal = new ClassDal();
        StudentRecordDal RecordDal = new StudentRecordDal();
        DataModelContainer db = new DataModelContainer();
        int pagesize = 5;
        // GET: StudentRecord
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult FindAll()
        {
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(RecordDal.FindAll(), setting);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        //查询所有班级
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
        //联想词
        [HttpGet]
        public JsonResult Lenovo(string Value)
        {
            var data = RecordDal.Lenovo(Value);
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(data, setting);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        //搜索
        [HttpPost]
        public JsonResult FindAll(string Value,int page=1)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var data = RecordDal.Serach(Value,page);
            var pages = data.cout;
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
            var ret = JsonConvert.SerializeObject(data.data, setting);
            return Json(new { ret = ret, pg = pg}, JsonRequestBehavior.AllowGet);
        }

    }
}