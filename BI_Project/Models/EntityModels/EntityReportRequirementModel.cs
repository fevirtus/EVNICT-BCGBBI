using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Models.EntityModels
{
    public class EntityReportRequirementModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ConfirmExpired { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public int CreatorId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int Modifier { get; set; }

        //From table GB_ReportBI, GB_ReportConfirm
        public string ReportName { get; set; }
       
        public string DepartmentCode { get; set; }
       
        public bool ConfirmStatus { get; set; }
       
        public bool DataStatus { get; set; }

        public int ReportBIId { get; set; }
    }
}