using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.EntityModels;
namespace BI_Project.Services.GBTask
{
    public class BlockDataGBTaskCreateModel : BI_Project.Models.EntityModels.EntityGBTaskModel
    {
        
        public List<EntityRoleModel> ListAllRoles { set; get; }

        public string StrAllowedMenus { set; get; }
        public BlockDataGBTaskCreateModel():base()
        {
            ListAllRoles = new List<EntityRoleModel>();
        }

        

    }
}