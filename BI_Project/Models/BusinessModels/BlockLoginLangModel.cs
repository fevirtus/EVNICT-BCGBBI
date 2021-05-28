using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BI_Project.Models.UI;
namespace BI_Project.Models.BusinessModels
{
    public class BlockLoginLangModel: BlockLanguageModel
    {
        public string LblHelpTitle { set; get; }

        public string LblUserName { set; get; }

        public string LblPassword { set; get; }
        public string LblDepartment { set; get; }

        public string BtnSubmit { set; get; }

        public string LblForgetPassword { set; get; }

        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            this.LblHelpTitle = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblHelpTitle");
            this.LblUserName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblUserName");
            this.LblPassword = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblPassword");
            this.BtnSubmit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".BtnSubmit");
            this.LblForgetPassword = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblForgetPassword");
            this.LblDepartment = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".LblDepartment");
        }
    }
}