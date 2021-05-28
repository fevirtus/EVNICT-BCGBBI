using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Models.EntityModels
{
    public class EntityGBTaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ReportRequirementId { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public bool DataStatus { get; set; }

        public DateTime Created { get; set; }
        public int CreatorId { get; set; }

        public string ResultFile { get; set; }
        public string Comment { get; set; }

        public DateTime Deadline { get; set; }

        public string DepartmentCode { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }
    }
}