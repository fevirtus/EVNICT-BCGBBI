using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.EntityModels;
namespace BI_Project.Services.Importers
{
    public class BlockCategoryModel 
    {
        
        

        public BlockCategoryModel():base()
        {
        
        }

        public List<EntityProfilesModel> ListHistory { set; get; }
        public string PROFILEID { get; set; }
        public string DESCR { get; set; }
        public string PARENT_PROFILEID { set; get; }
        public string PROFILE_TYPE { set; get; }
        public int DISPORDER { get; set; }

        public string PermissionID { set; get; }
        public int NumberPages { set; get; }
        public string HelpDoc { set; get; }
        public int CurrentPage { set; get; }
        public int PerPage { set; get; }
    }
}