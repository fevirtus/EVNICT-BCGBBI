using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.EntityModels;

namespace BI_Project.Services.BCGB
{
    public class BlockDataBCGBDetailModel : EntityReportRequirementModel
    {
        public List<EntityRoleModel> ListAllRoles { set; get; }

        public string StrAllowedMenus { set; get; }
        public BlockDataBCGBDetailModel():base()
        {
            ListAllRoles = new List<EntityRoleModel>();
        }

    }
}