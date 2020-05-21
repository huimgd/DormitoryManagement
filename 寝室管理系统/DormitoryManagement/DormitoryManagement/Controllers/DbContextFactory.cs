using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DormitoryManagement.Controllers
{
    public class DbContextFactory
    {
        public static DbContext GetCurrentDbContext()
        {
            //一次请求公用一个实例。上下文可以切换
            return new DataModelContainer();
        }
    }
}