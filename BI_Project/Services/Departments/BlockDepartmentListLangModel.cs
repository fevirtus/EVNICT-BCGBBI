using BI_Project.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Services.Departments
{
    public class BlockDepartmentListLangModel : BlockLanguageModel
    {
        public string LblNo { set; get; }
        public string Name { get; set; }
        public string LblAction { set; get; }

        public string LblEdit { set; get; }
        public string LblDelete { set; get; }

        public string LBLConfirmDelete { set; get; }

        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            this.LblNo = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblNo");
            this.Name = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Name");
            this.LblEdit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblEdit");
            this.LblDelete = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblDelete");
            this.LblAction = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblAction");
            this.LBLConfirmDelete = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblAction");
        }
    }
}