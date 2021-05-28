using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BI_Project.Models.UI;
namespace BI_Project.Services.Menus
{
    public class BlockCreateMenuLangModel : BlockLanguageModel
    {
        public string Lblparent { set; get; }
        public string Lblname { set; get; }
        public string title_edit { set; get; }

        public string BtnSubmit { set; get; }
        public string LblPath { set; get; }
        public string PathTableau { set; get; }
        public string Departments { get; set; }
        public string LblPriority { set; get; }
        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            this.Lblparent = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Lblparent");
            this.Lblname = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Lblname");
            this.title_edit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".title_edit");
            this.BtnSubmit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".BtnSubmit");
            this.LblPath = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblPath");
            this.LblPriority = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblPriority");
            this.PathTableau = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".PathTableau");
            this.Departments = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Departments");
        }
    }
}