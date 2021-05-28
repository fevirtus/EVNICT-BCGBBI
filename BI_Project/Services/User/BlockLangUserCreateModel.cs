using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BI_Project.Models.UI;
namespace BI_Project.Services.User
{
    public class BlockLangUserCreateModel : BlockLanguageModel
    {
        public string UserName { get; set; }
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}", ErrorMessage = "Password minimum 8 characters and contain at least one uppercase, one lowercase, one number.")]
        public string Password { get; set; }
        
        public string ConfirmPassword { set; get; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string IsAdmin { get; set; }
        
        public string title_edit { set; get; }

        public string Departments { get; set; }

        public string Roles { set; get; }

        public string Menus { set; get; }
        public string BtnSubmit { set; get; }
        public string FullName { get; set; }
        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            this.title_edit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".title_edit");
            this.BtnSubmit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".BtnSubmit");

            this.Email = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Email");
            this.Phone = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Phone");
            this.Roles = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Roles");
            this.Menus = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Menus");    
            this.Departments = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Departments");


            this.UserName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".UserName");
            this.Password = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Password");
            this.ConfirmPassword = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".ConfirmPassword");
            this.IsAdmin = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".IsAdmin");
            this.FullName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".FullName");
        }
    }
}