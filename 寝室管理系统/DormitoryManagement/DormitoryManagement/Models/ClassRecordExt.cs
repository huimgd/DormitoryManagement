using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DormitoryManagement.Models
{
    public class ClassRecordExt
    {
    }
    //班级的伙伴类
    [MetadataType(typeof(ClassRecordfosValidate))]//使Class共享ClassfosValidate元数据
    public partial class ClassRecord
    {
    }
    public partial class ClassRecordfosValidate
    {
        [Display(Name ="入班时间")]
        public System.DateTime CheckinTime { get; set; }
        [Display(Name = "离班时间")]
        public System.DateTime DepartureTime { get; set; }
        [Display(Name ="班级信息")]
        public int ClassId { get; set; }
        [Display(Name ="学生信息")]
        public int StudentId { get; set; }
    }
}