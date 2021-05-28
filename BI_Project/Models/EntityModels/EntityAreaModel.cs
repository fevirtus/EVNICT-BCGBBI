using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Models.EntityModels
{
    public class EntityAreaModel
    {
        public int Id { get; set; }
        public string AreaName { get; set; }
        public string Description { get; set; }
        public int ParentId { get; set; }
        public bool? IsDelete { get; set; }

    }
}