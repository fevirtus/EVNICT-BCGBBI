using BI_Project.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Services.Menus
{
    public class BlockMenuLeftLangModel : BlockLanguageModel
    {
        public string ManagingUsers { set; get; }
        public string ListofUser { set; get; }
        public string CreateNewUser { set; get; }
        public string ManagingRoles { set; get; }
        public string ListofRole { set; get; }
        public string CreateNewRole { set; get; }
        public string ManagingMenus { set; get; }
        public string ListofMenu { set; get; }
        public string CreateNewMenu { set; get; }
        public string ManaginDepartments { set; get; }
        public string ListofDepartments { set; get; }
        public string CreateNewDepartments { set; get; }

        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);
            this.ManagingUsers = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".ManagingUsers");
            this.ListofUser = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".ListofUser");
            this.CreateNewUser = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".CreateNewUser");
            this.ManagingRoles = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".ManagingRoles");
            this.ListofRole = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".ListofRole");
            this.CreateNewRole = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".CreateNewRole");
            this.ManagingMenus = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".ManagingMenus");
            this.ListofMenu = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".ListofMenu");
            this.CreateNewMenu = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".CreateNewMenu");
            this.ManaginDepartments = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".ManaginDepartments");
            this.ListofDepartments = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".ListofDepartments");
            this.CreateNewDepartments = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".CreateNewDepartments");

        }

    }
}