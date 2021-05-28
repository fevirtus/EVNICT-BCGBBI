using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Models.EntityModels
{
    public class EntityReportBIModel
    {
        public int Id { get; set; }
        public string ReportName { get; set; }
        public string ReportCode { get; set; }
        public string UrlLink { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public string DepartmentCode { get; set; }
        public int AreaId { get; set; }
        public string Cycle { get; set; }
        public int DepartmentId { get; set; }
        public bool IsDelete { get; set; }

        public string CycleName { get; set; }

        public string AreaName { get; set; }
    }
}