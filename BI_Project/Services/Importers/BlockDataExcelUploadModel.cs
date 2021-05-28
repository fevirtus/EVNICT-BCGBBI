using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.EntityModels;
namespace BI_Project.Services.Importers
{
    public class BlockDataExcelUploadModel 
    {
        
        

        public BlockDataExcelUploadModel():base()
        {
        
        }

        public List<EntityUploadHistoryModel> ListHistory { set; get; }
        public string PermissionID { set; get; }


        public int NumberRecords { set; get; }

        public int NumberPages { set; get; }

        

        public string FolderUpload { set; get; }

        public string HelpDoc { set; get; }

        public string Note { set; get; }
        public int CurrentPage { set; get; }

        public int PerPage { set; get; }

        public int Month { set; get; }

        public int Year { set; get; }

        public bool Export { get; set; }
        public bool ExportBaseMonth { set; get; }
        public string Url { get; set; }
        public bool ExportOff { get; set; }
    }
}