using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Models.EntityModels
{
    public class EntityReportConfirmModel
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public int ReportRequirementId { get; set; }
        public string Description { get; set; }
        public bool ConfirmStatus { get; set; }
        public bool DataStatus { get; set; }
        public DateTime Created { get; set; }
        public int CreatorId { get; set; }
        public string ReportPathFile { get; set; }
        public int DepartmentId { get; set; }
        public int Cycle { get; set; }
        public int Year { get; set; }
        public DateTime ConfirmDate { get; set; }

        //adding attribute
        public string ReportRequirementName { get; set; }
        public string ReportBIName { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

    }
}