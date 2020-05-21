using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DormitoryManagement.Models
{
    [MetadataType(typeof(StudentfosValidate))]//共享元数据
    public partial class Student
    {
    }
    public partial class StudentfosValidate
    {
        [Display(Name="姓名")]
        public string Name { get; set; }
        [Display(Name = "联系方式")]
        public string Contact { get; set; }
        [Display(Name ="状态")]
        public int State { get; set; }
    }

}