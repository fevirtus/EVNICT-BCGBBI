using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Services.ReportRequire
{
    public class ReportRequireCreateModel
    {

        public ReportRequireCreateModel() { }
                

        public int Id { get; set; }

        public String Title { get; set; }

        public String ConfirmExpired { get; set; }

        public String Description { get; set; }

        public int CreatorId { get; set; }

    }
}