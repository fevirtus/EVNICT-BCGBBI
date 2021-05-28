using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Models.EntityModels
{
    public class EntityGBRPTEvaluateModel
    {
        public int Id { get; set; }
        public int ReportRequirementId { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int Total { get; set; }
        public int ReportUnConfirmNum { get; set; }
        public int DataCorrectNum { get; set; }
        public int DataInCorrectNum { get; set; }
        public int TaskDoneNum { get; set; }
        public int TaskProcessNum { get; set; }

        
        //&amp;
        public string DepartmentCode { 
            get
                ; 
            set; 
        }
        public string DepartmentCode_Evaluate { get; set; }
        public DateTime Created { get; set; }
        public int CreatorId { get; set; }





    }
}