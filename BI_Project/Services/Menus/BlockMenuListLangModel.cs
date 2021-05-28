using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BI_Project.Models.UI;
namespace BI_Project.Services.Menus
{
    public class BlockMenuListLangModel : BlockLanguageModel
    {
        public string LblNo { set; get; }
        public string Lblparent { set; get; }
        public string Lblname { set; get; }
        public string title_edit { set; get; }

        public string BtnSubmit { set; get; }
        public string LblPath { set; get; }

        public string LblPriority { set; get; }

        public string LblAction { set; get; }

        public string LblEdit { set; get; }
        public string LblDelete { set; get; }

        public string LBLConfirmDelete { set; get; }
        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);
            this.LblNo = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblNo");
            this.LblAction = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblAction");
            this.Lblparent = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Lblparent");
            this.Lblname = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Lblname");
            this.title_edit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".title_edit");
            this.BtnSubmit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".BtnSubmit");
            this.LblPath = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblPath");
            this.LblPriority = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblPriority");
            this.LblEdit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblEdit");
            this.LblDelete = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblDelete");
            this.LBLConfirmDelete = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LBLConfirmDelete");

        }
    }
}