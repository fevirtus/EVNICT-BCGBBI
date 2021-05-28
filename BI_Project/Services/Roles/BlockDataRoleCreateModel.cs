using BI_Project.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Services.Roles
{
    public class BlockDataRoleCreateModel
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int DeptID { get; set; }

        public string StrAllowedMenus { set; get; }
        public string StrAllowedReportBI { set; get; }
        public List<int>  ListRoleMenus { set; get; }

        public List<int> ListRoleReportBI { set; get; }

        //public int DepartId { get; set; }


        public BlockDataRoleCreateModel()
        {
            ListRoleMenus = new List<int>();
            ListRoleReportBI = new List<int>();
        }
        
    }
}