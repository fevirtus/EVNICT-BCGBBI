using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BI_Project.Models.UI;
namespace BI_Project.Models.BusinessModels
{
    public class BlockWelcomeLangModel : BlockLanguageModel
    {
        public string Lblwelcome { set; get; }

        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            this.Lblwelcome = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Lblwelcome.#cdata-section");
        }
    }
}