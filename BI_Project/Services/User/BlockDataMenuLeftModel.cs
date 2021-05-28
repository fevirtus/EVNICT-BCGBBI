using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models;
using BI_Project.Models.EntityModels;
using BI_Project.Models.UI;

namespace BI_Project.Services.User
{
    public enum MenuTypes { HasAllLinkAndText, HasOnlyText, No2Show}
    public class BlockDataMenuLeftModel
    {
        public List<EntityMenuModel> LstAllOfMenus { set; get; }

        public List<EntityMenuModel> LstAllowedMenus { set; get; }

        public List<EntityReportBIModel> LstAllOfReport { set; get; }

        public List<EntityReportBIModel> LstAllowedReport { set; get; }

        public string StrAllowedMenuIds { set; get; }

        public string StrAllowedLeveltrees { set; get; }

        public EntityUserModel EntityUserModel { set; get; }

        public BlockDataMenuLeftModel()
        {
            LstAllOfMenus = new List<EntityMenuModel>();
            LstAllowedMenus = new List<EntityMenuModel>();


            LstAllOfReport = new List<EntityReportBIModel>();
            LstAllowedReport = new List<EntityReportBIModel>();
            this.StrAllowedLeveltrees = "";
            StrAllowedLeveltrees = "";
        }

        public bool CheckMenuHasChild(EntityMenuModel menu)
        {
            bool found = false;

            if (EntityUserModel.IsAdmin == true) return true;
            foreach(EntityMenuModel child in LstAllowedMenus)
            {
                if (child.LevelTree.Contains(menu.LevelTree + "@@@"))
                {
                    found = true;
                    break;
                }
                else continue;
            }

            return found;

        }

        public MenuTypes Check2Show(EntityMenuModel menu)
        {
            MenuTypes output= MenuTypes.HasAllLinkAndText;

            //CHECK WHETHER THE USER HAS PERMISSION TO ACCESS THIS MENU
            if (this.EntityUserModel.LstSelectedMenu.Contains(menu.MenuId))
                return output;

            //CHECK WHETHER THE MENU HAS CHILD MENU, THAT BE ACCESSED BY THE USER
            if (CheckMenuHasChild(menu) == true) output = MenuTypes.HasOnlyText;
            else output = MenuTypes.No2Show;
            return output;
        }

    }
}