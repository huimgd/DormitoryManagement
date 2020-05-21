using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DormitoryManagement.Models
{
    //班级的伙伴类
    [MetadataType(typeof(ClassfosValidate))]//使Class共享ClassfosValidate元数据
    public partial class Class
    {
    }
    public partial class ClassfosValidate
    {
        [Required(ErrorMessage = "*必填")]
        [StringLength(5, ErrorMessage = "*超出规定长度(来自伙伴类)")]
        [Display(Name="班级名")]
        public string ClassName { get; set; }
        [Display(Name = "状态")]
        public int State { get; set; }
    }
}