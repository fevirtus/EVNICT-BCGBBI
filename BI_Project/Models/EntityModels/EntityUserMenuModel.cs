using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Models.EntityModels
{
    public class EntityUserMenuModel
    {
        public int MenuId { get; set; }
        public int UserId { get; set; }
        public bool IsDefaultPage { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}