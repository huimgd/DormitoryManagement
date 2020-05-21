using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DormitoryManagement.Models
{
    [MetadataType(typeof(DormitoryInfofosValidate))]//使Class共享ClassfosValidate元数据
    public partial class DormitoryInfo
    {
    }
    public partial class DormitoryInfofosValidate
    {   [Display(Name ="可用性")]
        public int State { get; set; }
        [Display(Name ="编号")]
        public string Number { get; set; }
        [Display(Name ="可容纳人数")]
        public int Capacity { get; set; }
        [Display(Name ="已有人数")]
        public int Already { get; set; }
    }

}