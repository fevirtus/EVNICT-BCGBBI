using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Models.EntityModels
{
    public class EntityMenuModel
    {
        private int _level;
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int ParentId { get; set; }
        public string MenuLevel { get; set; }
        public bool Status { get; set; }
        public bool Selected { get; set; }
        public string FilterCommand { set; get; }

        public string FilterValue { set; get; }
        public string LevelTree { set; get; }
        public int Priority { set; get; }

        public string TableauUrl { set; get; }
        public int DeptID { get; set; }
        public int Level {
            

            get
            {
                string[] separatingChars = { "@@@"};
                string[] lstLevel = (LevelTree + "@@@abc").Split(separatingChars,System.StringSplitOptions.RemoveEmptyEntries);
                
                _level = lstLevel.Length - 1;
                return _level;
            }
        }
        public string IconCSS { set; get; }
        public bool HasChild { set; get; }
        public EntityMenuModel()
        {
            MenuLevel = "";
            Status = false;
            FilterCommand = "";
            FilterValue = "";
            Path = "";
        }

        public  EntityMenuModel Clone()
        {
            EntityMenuModel output = new EntityMenuModel();
            output.FilterCommand = this.FilterCommand;
            output.DeptID = this.DeptID;
            output.FilterValue = this.FilterValue;
            output.LevelTree = this.LevelTree;
            output.MenuId = this.MenuId;
            output.MenuLevel = this.MenuLevel;
            output.Name = this.Name;
            output.ParentId = this.ParentId;
            output.Path = this.Path;
            output.Priority = this.Priority;
            output.Status = this.Status;
            output.IconCSS = this.IconCSS;
            return output;
        }
    }
}