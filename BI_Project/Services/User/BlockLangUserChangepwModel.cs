using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BI_Project.Models.UI;
namespace BI_Project.Services.User
{
    public class BlockLangUserChangepwModel : BlockLanguageModel
    {

        

        public string Password { get; set; }

        public string OldPassword { get; set; }
        
        public string ConfirmPassword { set; get; }


        public string BtnSubmit { set; get; }
        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            this.OldPassword = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".OldPassword");
            this.BtnSubmit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".BtnSubmit");

            

     
            this.Password = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Password");
            this.ConfirmPassword = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".ConfirmPassword");

        }
    }
}