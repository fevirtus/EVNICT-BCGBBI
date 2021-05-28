using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BI_Project.Models.UI;
namespace BI_Project.Services.Roles
{
    public class BlockLangRoleListModel : BlockLanguageModel
    {
        public string LblNo { set; get; }
        public string LblDescription { set; get; }
        public string Lblname { set; get; }
        public string Department { get; set; }
        public string LblAction { set; get; }
        public string LblEdit { set; get; }
        public string LblDelete { set; get; }
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
            this.Department = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Department");
        }
    }
}