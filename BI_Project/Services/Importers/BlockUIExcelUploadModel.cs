using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.EntityModels;
namespace BI_Project.Services.Importers
{
    public class BlockUIExcelUploadModel
    {
        
        

        public BlockUIExcelUploadModel()
        {
        
        }

        public int CurrentPage { set; get; }

        public int PerPage { set; get; }

        public int Month { set; get; }

        public int Year { set; get; }

        
    }
}