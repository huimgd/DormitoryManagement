using System;
using System.ComponentModel.DataAnnotations;

namespace DormitoryManagement.Models
{
    public class StudentRecordExt
    {
    }
    [MetadataType(typeof(StudentRecordfosValidate))]//共享元数据
    public partial class StudentRecord
    {
    }
    public partial class StudentRecordfosValidate
    {
        [Display(Name = "入住时间")]
        [DisplayFormat(DataFormatString = "0:yyyy/MM/dd")]
        public DateTime CheckinTime { get; set; }
        [Display(Name = "离开时间")]
        [DisplayFormat(DataFormatString = "0:yyyy/MM/dd")]
        public DateTime DepartureTime { get; set; }
        [Display(Name = "学生信息")]
        public int StudentId { get; set; }
        [Display(Name = "寝室信息")]
        public int DormitoryInfoId { get; set; }
    }
}