using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BI_Project.Models.UI;
namespace BI_Project.Services.User
{
    public class BlockLangUserListModel : BlockLanguageModel
    {
        public string LblNo { set; get; }
        public string LblDescription { set; get; }
        public string Lblname { set; get; }
        
        public string Email { set; get; }

        public string Phone { set; get; }

        public string Roles { set; get; }

        public string Menus { set; get; }

        public string DepartName  { get; set; }

        public string LblAction { set; get; }

        public string LblEdit { set; get; }
        public string LblDelete { set; get; }
        public string FullName { set; get; }

        public string LBLConfirmDelete { set; get; }
        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);
            this.LblNo = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblNo");
            this.LblAction = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblAction");
            this.LblDescription = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblDescription");
            this.Lblname = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Lblname");
            this.LblEdit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblEdit");
            this.LblDelete = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblDelete");
            this.LBLConfirmDelete = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LBLConfirmDelete");

            this.Email = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Email");
            this.Phone = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Phone");
            this.Roles = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Roles");
            this.Menus = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Menus");
            this.DepartName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".DepartName");
            this.FullName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".FullName");
        }
    }
}