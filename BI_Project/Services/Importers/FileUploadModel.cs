using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.EntityModels;
namespace BI_Project.Services.Importers
{
    public class FileUploadModel 
    {

        public HttpPostedFileBase FileObj { get; set; }
        public int currentyear { get; set; }
        public int currentMonth { set; get; }
        public bool ExportOff { set; get; }
        public int PermissionId { set; get; }

    }
}