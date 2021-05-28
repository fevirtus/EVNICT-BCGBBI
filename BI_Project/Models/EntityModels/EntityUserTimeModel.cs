using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bicen.Models.EntityModels
{
    public class EntityUserTimeModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public Guid PKID { get; set; }
        public DateTime TimeLogin { get; set; }
        public DateTime TimeLogout { get; set; }

        public DateTime TimeClickDashboard { get; set; }
        public string Dashboard { get; set; }

        public DateTime Date { get; set; }
    }
}