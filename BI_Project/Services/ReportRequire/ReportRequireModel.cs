using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static BI_Project.Helpers.UIMenuTreeHelper;

namespace BI_Project.Services.ReportRequire
{
    public class ReportRequireModel
    {

        public ReportRequireModel() { }

        public int Stt { get; set; }

        public int Id { get; set; }

        public String Title { get; set; }

        public DateTime ConfirmExpired { get; set; }

        public String Description { get; set; }

        public int Status { get; set; }

        public DateTime Created { get; set; }

        public int CreatorId { get; set; }       

        public DateTime LastUpdated { get; set; }

        public int Modifier { get; set; }

        public String FullName { get; set; }

        public String TrangThai { get; set; }

        public String strReportId { get; set; }

        public String Cycle { get; set; }
    }
}