using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Models.EntityModels
{
    public class EntityRoleModel
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public List<EntityUserRoleModel> LstUserRole { get; set; }

        public List<EntityMenuModel> LstMenus { get; set; }

        public int DepartId { get; set; }
        public string DepartmentName { get; set; }
    }
}