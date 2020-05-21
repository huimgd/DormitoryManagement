using DormitoryManagement.DAL;
using DormitoryManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Controllers
{
    public class DormitoryController : Controller
    {
        //寝室
        // GET: Dormitory
        DormitoryDal Dormitorydal = new DormitoryDal();
        ClassDal Classdal = new ClassDal();
        FloorDal Floordal = new FloorDal();
        LayerDal Layerdal = new LayerDal();
        StudentDal Studentdal = new StudentDal();
        StudentRecordDal StudentRecorddal = new StudentRecordDal();
        DataModelContainer db = new DataModelContainer();
        int pagesize = 5;//每页数据条数
        public ActionResult Index()
        {
            IQueryable<DormitoryInfo> temp = from u in db.Dormitory select u;
            int page = temp.Count() / pagesize;
            if (temp.Count() % pagesize != 0)
            {
                page = page + 1;
            }
            ViewData["Message"] = page;
            return View();
        }
        //判断这栋楼的这一层是否已经有了这个寝室
        public bool AddSelect(string Number, int Floor, int Layer)
        {
            try
            {
               bool pd= Dormitorydal.determineDormitory(Number, Floor, Layer);
                return pd;
            }
            catch (Exception)
            {
                return false;
            }

        }  //添加寝室
        [HttpPost]
        public bool Add(DormitoryInfo entity)
        {
            try
            {
                entity.State = 0;
                entity.Already = 0;
                entity.CreationTime = DateTime.Now;
                db.Dormitory.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        
        [HttpGet]
        //查询所有寝室
        public JsonResult FindAll(int page = 1)
        {

           
            JsonSerializerSettings setting = new JsonSerializerSettings()//把数据转换为json类型
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            
            var ret = JsonConvert.SerializeObject(Dormitorydal.FindALL(page), setting);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //搜索
        public JsonResult FindAll(string Value,int page = 1)
        {


            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var data = Dormitorydal.Serach(page, Value);
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

        //修改状态
        public bool UpdateState(int id, int state)
        {
            return Dormitorydal.UpdateState(id,state);
        }
        class Stu
        {
            public string Name { get; set; }
            public string Contact { get; set; }
        }
        //查询寝室已有学生
        public JsonResult FindStu(int DormitoryId)
        {
            return Json(Dormitorydal.FindStu(DormitoryId), JsonRequestBehavior.AllowGet);
        }
        
        //判断数据库是否已经有了这个学生
        public bool StuAddSelect(string Name)
        {
            try
            {
             bool pd= Dormitorydal.SelectStu(Name);
                return pd;
            }

            catch (Exception)
            {
                return false;
            }

        }
        public bool Determine_Capactiy(int id)
        {
           return Dormitorydal.Determine_Capactiy(id);
        }
        //查询所有班级
        public JsonResult FindALLClass()
        {
            return Json(Classdal.FindAllClass(), JsonRequestBehavior.AllowGet);
        }
        //保存查询出的学生信息
        class nscc
        {
            public int Id { get; set; }//学生id
            public string Name { get; set; }//学生姓名
            public DateTime? srCheckinTime { get; set; }//入住记录——进入时间
            public string Number { get; set; }//寝室编号
            public DateTime? crCheckinTime { get; set; }//入班记录--进入时间
            public string CLassName { get; set; }//班级名

        }
        //查询所有的楼添加寝室用
        public JsonResult FindAllFloor()
        {
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(Floordal.FindAll(), setting);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        //查询该楼号的层
        public JsonResult FindAllLayer(int FloorId)
        {
          JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(Layerdal.FindLayerByDF(FloorId), setting);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        //联想词功能
        [HttpGet]
        public JsonResult Lenovo(String Value)
        {
            var data = Dormitorydal.Lenovo(Value);
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var ret = JsonConvert.SerializeObject(data, setting);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        //批量修改
        public bool IntoStu(int[] list, int Dormitoryid)
        {
            try
            {
                for (int i = 0; i < list.Length; i++)
                {
                    int StuId = list[i];
                    List<StudentRecord> data = StudentRecorddal.FindByStuId(StuId);
                    if (data.FirstOrDefault().DepartureTime==null)
                    {
                        data.FirstOrDefault().DepartureTime = DateTime.Now;
                        db.Entry(data.FirstOrDefault()).State = EntityState.Modified;
                        db.SaveChanges();
                        StudentRecord sr = new StudentRecord();
                        sr.DormitoryInfoId = Dormitoryid;
                        sr.CheckinTime = DateTime.Now;
                        sr.StudentId = StuId;
                        sr.DepartureTime = null;
                        db.StudentRecord.Add(sr);
                        db.SaveChanges();
                    }
                    else
                    {
                        StudentRecord sr = new StudentRecord();
                        sr.DormitoryInfoId = Dormitoryid;
                        sr.CheckinTime = DateTime.Now;
                        sr.StudentId = StuId;
                        sr.DepartureTime = null;
                        db.StudentRecord.Add(sr);
                        db.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }
        /// <summary>
        /// 根据寝室所在楼名判断改寝室是否可呗手动启用
        /// </summary>
        /// <param name="FloorNumber">楼号</param>
        /// <returns></returns>
        public bool FloorState(string FloorNumber)
        {
           return  Floordal.FindStateByNumber(FloorNumber);
        }
        /// <summary>
        /// 批量从寝室移除学生
        /// </summary>
        /// <param name="list">移除学是的id数组</param>
        /// <param name="Dormitoryid">寝室id</param>
        /// <returns></returns>
        public bool OutStu(int[] list, int Dormitoryid)
        {
            try
            {
                for (int i = 0; i < list.Length; i++)
                {
                    //得到学生的id
                    int StuId = list[i];
                    //得到学生的入住记录
                    List<StudentRecord> data = StudentRecorddal.FindByStuId(StuId);
                    data.FirstOrDefault().DepartureTime = DateTime.Now;
                    db.Entry(data.FirstOrDefault()).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
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
                var ret = JsonConvert.SerializeObject(Studentdal.FindStuByClassIdAndStuName(Classid,StudentName), setting);
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

        class Floor
        {
            public int Id { get; set; }
            public string FloorNumber { get; set; }
            public int LayerNumber { get; set; }
            public int DormitoryNumber { get; set; }
            public int State { get; set; }
        }
        class Layer
        {
            public int Id { get; set; }
            public string LayerNumber { get; set; }
        }
        
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

        }
        //excel导入
        [HttpPost]
        public string[] UploadXls()
        {
            string[] mistake=new string[10];//用于保存返回给前台的错误
            try
            {
                HttpPostedFileBase file = HttpContext.Request.Files["BulkXls"];
                if (file == null)
                {
                    mistake[0]="请选择文件";
                }
                var ext = Path.GetExtension(file.FileName);
                var exts = ",.xls,.xlsx,";
                if (!exts.Contains("," + ext + ","))
                {
                    mistake[0] = "请上传.xls,.xlsx文件";
                }
                //上传文件
                var path = HttpContext.Request.MapPath("~/");
                var dir = "/Upload_Files/xls/";
                if (!Directory.Exists(path + dir))
                {
                    Directory.CreateDirectory(path + dir);
                }
                //保存文件
                var fullPath = path + dir + "hotels" + Path.GetExtension(file.FileName);
                file.SaveAs(fullPath);

                var filePath = Server.MapPath("~/Upload_Files/xls/" + "hotels" + Path.GetExtension(file.FileName));
                // 读取Excel文件到DataSet中
                string connStr = "";
                string fileType = System.IO.Path.GetExtension(filePath);
                if (!string.IsNullOrEmpty(fileType))
                {
                    //支持文件格式不一样，Excel有 97-2003及2007格式  分别为 .xls和.xlsx
                    if (fileType == ".xls")
                    { connStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\""; }
                    else
                    { connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\""; }
                    string sql_F = "Select * FROM [{0}]";

                    OleDbConnection conn = null;
                    OleDbDataAdapter da = null;

                    DataSet ds = new DataSet();
                    try
                    {
                        // 初始化连接，并打开
                        conn = new OleDbConnection(connStr);
                        conn.Open();
                        // 初始化适配器
                        da = new OleDbDataAdapter();
                        da.SelectCommand = new OleDbCommand(String.Format(sql_F, "Sheet1$"), conn);
                        DataSet dsItem = new DataSet();
                        da.Fill(dsItem, "t");
                        ds.Tables.Add(dsItem.Tables[0].Copy());
                        
                        //这里的for是循环DataSet中的值并向数据库中插入
                        for (int i = 0; i < dsItem.Tables[0].Rows.Count; i++)
                        {

                            // var student = dsItem.Tables[0];//得到表明
                            ////比如我们导入的 Excel表中有 四列
                            //if (!_HotelRepository.IsExist(u => u.HotelName == hotelName))
                            //{
                            //实例化一个楼对象
                            DetailsFloor floor = new DetailsFloor();
                            //实例化一个层对象
                            DetailsLayer layer = new DetailsLayer();
                            //实例化一个寝室对象
                            DormitoryInfo dormitory = new DormitoryInfo();
                            //实例化一个入住记录对象
                            StudentRecord studentRecord = new StudentRecord();
                            //这里是Excel表格中对应的列
                            string FloorName = floor.FloorName = dsItem.Tables[0].Rows[i]["楼"].ToString();//得到楼姓名
                            string LayerNumber = layer.LayerNumber = dsItem.Tables[0].Rows[i]["层"].ToString();//得到层
                            string DNumber = dormitory.Number = dsItem.Tables[0].Rows[i]["寝室编号"].ToString();//得到寝室编号
                            string Dstate = dsItem.Tables[0].Rows[i]["寝室状态"].ToString();//得到寝室状态
                            string Fstate = dsItem.Tables[0].Rows[i]["楼状态"].ToString();//得到楼状态
                            int DCapacity = dormitory.Capacity = Convert.ToInt32(dsItem.Tables[0].Rows[i]["可容纳人数"]);//得到寝室可容纳人数
                            string Stu = dsItem.Tables[0].Rows[i]["已有学生"].ToString();//得到楼状态
                            int DState = 0;//用于保存寝室当前状态
                            int FState = 0;//用于保存楼当前状态
                            //寝室状态
                            if (Dstate.Equals("启用"))
                            {
                                DState = 0;
                            }
                            else if (Dstate.Equals("停用"))
                            {
                                DState = 1;
                            }
                            else
                            {
                                mistake[0] = "寝室状态格式错误只能有在读和已毕业两种状态";
                            }
                            dormitory.State = DState;//得到寝室状态
                            //楼状态
                            if (Fstate.Equals("启用"))
                            {
                                FState = 0;
                            }
                            else if (Fstate.Equals("停用"))
                            {
                                FState = 1;
                            }
                            else
                            {
                                mistake[0]="楼状态格式错误只能有在读和已毕业两种状态";
                            }
                            floor.State = FState;//得到楼状态
                            //查询楼，判断该楼是否存在
                           bool pdFloor=Floordal.determineFloor(FloorName);
                            //更具楼号查出楼的相关信息
                            var Floordata = Floordal.FindFloorByName(FloorName);
                            //已有该楼
                            if (pdFloor==true)
                            {
                                //楼的id
                                    int FloorID = Floordata.FirstOrDefault().Id;
                                    //修改楼状态
                                    Floordal.UpdateStateById(FloorID);
                                    //判断层
                                   bool pdLayer= Layerdal.determineLayerByFloorNameandLayerNumber(FloorName, LayerNumber);
                                        //如果有这层
                                    if (pdLayer==true)
                                    {
                                        //判断寝室 
                                       bool PdDor= Dormitorydal.determineDormitoryByDfandDlandDNumber(DNumber, FloorName, LayerNumber);
                                            //如果已经有了这个寝室
                                        if (PdDor==true)
                                        {
                                            //获取寝室相关信息
                                            var DormitoryInfodata = Dormitorydal.FindByDfandDlandDNumber(DNumber, FloorName, LayerNumber);
                                        
                                               int Dormitoryid = DormitoryInfodata.FirstOrDefault().Id;
                                                //修改寝室状态和可容纳人数
                                                Dormitorydal.UpdateStateAndCapacity(Dormitoryid, DCapacity);
                                                //截取出每个学生的名字
                                                string[] xx = Stu.Split(new string[] { "、" }, StringSplitOptions.None);
                                                foreach (string item in xx)
                                                {  //判断改寝室可容纳人数
                                                  //如果寝室还能容纳人
                                                    if (DormitoryInfodata.FirstOrDefault().Capacity >= DormitoryInfodata.FirstOrDefault().person+xx.Length)
                                                    {

                                                        //判断是否有这个学生
                                                     var studentdata=Studentdal.determineStuByName(item);
                                                        //如果有该学生
                                                        if (studentdata.Count() > 0)
                                                        {
                                                            int did = DormitoryInfodata.FirstOrDefault().Id;
                                                            //查询寝室是否已有该学生
                                                         bool pdStuByDNameAndStuName = Dormitorydal.DetermineStuByDNumberAndStuName(did, item);
                                                            //如果已有这个学生
                                                            if (pdStuByDNameAndStuName==true)
                                                            {
                                                                continue;
                                                            }
                                                            else//如果寝室没有这个学生
                                                            {
                                                                //得到学生Id
                                                                int StuId = studentdata.FirstOrDefault().Id;
                                                                //修改学生之前的入住记录
                                                                StudentRecorddal.UpdateRecordByStuId(StuId);
                                                                //给该学生添加入住记录
                                                                StudentRecorddal.AddRecordByDIdAndStuId(did, StuId);
                                                            }
                                                           
                                                        }
                                                        else//如果学生表没有这个学生
                                                        {
                                                            mistake[1] = "没有‘" + item + "’的学生信息";
                                                        }

                                                    }
                                                    else//如果寝室已满
                                                    {
                                                         mistake[2] =  "‘" + DormitoryInfodata.FirstOrDefault().Number + "’寝室已满";
                                                    }
                                                }

                                        }
                                    }
                                    else//如果该楼没有这层
                                    {
                                        int FloorId = Floordata.FirstOrDefault().Id;
                                        //添加层
                                    bool pdAddLayer=Layerdal.AddLayer(LayerNumber, FloorId);
                                        if (pdAddLayer==true)
                                        {
                                          //查询层id
                                          var Layer=  Layerdal.FindLayerId(FloorName, LayerNumber);
                                            int LayerId = Layer.FirstOrDefault().Id;
                                            //添加寝室
                                         bool pdAddDormitory= Dormitorydal.AddDormitory(DNumber, DCapacity,LayerId);
                                            //查询层的id
                                            if (pdAddDormitory==true)//添加寝室成功
                                            {
                                                
                                                //根据寝室编号、楼号、层号查询该寝室
                                                var DormitoryInfodata = Dormitorydal.FindByDfandDlandDNumber(DNumber, FloorName, LayerNumber);
                                                string[] xx = Stu.Split(new string[] { "、" }, StringSplitOptions.None);
                                                foreach (string item in xx)
                                                {
                                                    //如果寝室还能容纳人
                                                    if (DormitoryInfodata.FirstOrDefault().Capacity >= DormitoryInfodata.FirstOrDefault().person + xx.Length)
                                                    {

                                                        //判断是否有这个学生
                                                        var studentdata = Studentdal.determineStuByName(item);
                                                        //如果有该学生
                                                        if (studentdata.Count() > 0)
                                                        {
                                                            int did = DormitoryInfodata.FirstOrDefault().Id;
                                                            //查询寝室是否已有该学生
                                                            bool pdStuByDNameAndStuName = Dormitorydal.DetermineStuByDNumberAndStuName(did, item);
                                                            //如果已有这个学生
                                                            if (pdStuByDNameAndStuName == true)
                                                            {
                                                                continue;
                                                            }
                                                            else//如果寝室没有这个学生
                                                            {
                                                                //得到学生Id
                                                                int StuId = studentdata.FirstOrDefault().Id;
                                                                //修改学生之前的入住记录
                                                                StudentRecorddal.UpdateRecordByStuId(StuId);
                                                                //给该学生添加入住记录
                                                                StudentRecorddal.AddRecordByDIdAndStuId(did, StuId);
                                                            }
                                                        }
                                                        else//如果没有这个学生
                                                        {
                                                            mistake[1] = "没有‘" + item + "’的学生信息";
                                                        }

                                                    }
                                                    else//如果寝室已满
                                                    {
                                                        mistake[2] = "‘" + DormitoryInfodata.FirstOrDefault().Number + "’寝室已满";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                mistake[3] = "" + Floordata.FirstOrDefault().FloorNumber + "添加" + LayerNumber + "层添加"+ DNumber + "寝室失败";
                                            }

                                        }
                                        else
                                        {
                                            mistake[4] = "" +Floordata.FirstOrDefault().FloorNumber+ "添加"+ LayerNumber + "层失败";
                                        }
                                       
                                    }
                            }
                            else//没有该楼
                            {
                                //添加楼
                              bool pdAddFloor=  Floordal.AddFloor(FloorName, FState);
                                if (pdAddFloor==true)//添加楼成功
                                {
                                    //得到刚添加楼的id
                                  int FloorId=db.DetailsFloor.Select(s => s.Id).Max();
                                    //添加层
                                    bool pdAddLayer = Layerdal.AddLayer(LayerNumber, FloorId);
                                    if (pdAddLayer == true)
                                    {
                                        //查询层id
                                        var Layer = Layerdal.FindLayerId(FloorName, LayerNumber);
                                        int LayerId = Layer.FirstOrDefault().Id;
                                        //添加寝室
                                        bool pdAddDormitory = Dormitorydal.AddDormitory(DNumber, DCapacity, LayerId);
                                        if (pdAddDormitory == true)//添加寝室成功
                                        {
                                            //根据寝室编号、楼号、层号查询该寝室
                                            var DormitoryInfodata = Dormitorydal.FindByDfandDlandDNumber(DNumber, FloorName, LayerNumber);
                                            string[] xx = Stu.Split(new string[] { "、" }, StringSplitOptions.None);
                                            foreach (string item in xx)
                                            {
                                                //如果寝室还能容纳人
                                                if (DormitoryInfodata.FirstOrDefault().Capacity >= DormitoryInfodata.FirstOrDefault().person + xx.Length)
                                                {

                                                    //判断是否有这个学生
                                                    var studentdata = Studentdal.determineStuByName(item);
                                                    //如果有该学生
                                                    if (studentdata.Count() > 0)
                                                    {
                                                        int did = DormitoryInfodata.FirstOrDefault().Id;
                                                        //查询寝室是否已有该学生
                                                        bool pdStuByDNameAndStuName = Dormitorydal.DetermineStuByDNumberAndStuName(did, item);
                                                        //如果已有这个学生
                                                        if (pdStuByDNameAndStuName == true)
                                                        {
                                                            continue;
                                                        }
                                                        else//如果寝室没有这个学生
                                                        {
                                                            //得到学生Id
                                                            int StuId = studentdata.FirstOrDefault().Id;
                                                            //修改学生之前的入住记录
                                                            StudentRecorddal.UpdateRecordByStuId(StuId);
                                                            //给该学生添加入住记录
                                                            StudentRecorddal.AddRecordByDIdAndStuId(did, StuId);
                                                        }
                                                    }
                                                    else//如果没有这个学生
                                                    {
                                                        mistake[1] = "没有‘" + item + "’的学生信息";
                                                    }
                                                }
                                                else//如果寝室已满
                                                {
                                                    mistake[2] = "‘" + DormitoryInfodata.FirstOrDefault().Number + "’寝室已满";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            mistake[3] = "no:" + Floordata.FirstOrDefault().FloorNumber + "添加" + LayerNumber + "层添加" + DNumber + "寝室失败";
                                        }

                                    }
                                    else
                                    {
                                        mistake[4] = "no:" + Floordata.FirstOrDefault().FloorNumber + "添加" + LayerNumber + "层失败";
                                    }
                                }
                                else//添加楼失败
                                {
                                    mistake[5] = "no:添加" + FloorName + "楼失败";
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        // 关闭连接
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                            da.Dispose();
                            conn.Dispose();
                        }
                    }
                }


            }
            catch (Exception ex)
            {

            }
            return mistake;
        }

    }
}