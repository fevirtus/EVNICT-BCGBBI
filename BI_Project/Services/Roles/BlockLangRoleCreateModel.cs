using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.UI;
namespace BI_Project.Services.Roles
{
    public class BlockLangRoleCreateModel: BlockLanguageModel
    {
        public string LblDescription { set; get; }
        public string Lblname { set; get; }
        public string title_edit { set; get; }

        public string BtnSubmit { set; get; }
        
        public string LblMenus { set; get; }
        public string Departments { get; set; }
        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            this.LblDescription = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblDescription");
            this.Lblname = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Lblname");
            this.title_edit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".title_edit");
            this.BtnSubmit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".BtnSubmit");
            this.Departments = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Departments");
            this.LblMenus = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblMenus");
        }

    }
}