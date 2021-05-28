using BI_Project.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Services.Departments
{
    public class BlockDepartmentCreateLangModel : BlockLanguageModel
    {

        public string Name { get; set; }
        public string Filter01 { get; set; }
        public string BtnSubmit { get; set; }
        public string Title_Edit { get; set; }       

        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            this.Name = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Name");
            this.Filter01 = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Filter01");
            this.BtnSubmit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".BtnSubmit");
            this.Title_Edit = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".title_edit");          
        }
    }
}