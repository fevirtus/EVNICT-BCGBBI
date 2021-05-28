using BI_Project.Models.UI;

namespace bicen.Services.Importers
{
    public class BlockLangRowUploadModel : BlockLanguageModel
    {
        public string Year { set; get; }
        public string Month { set; get; }
        public string Search { set; get; }

        public string File { set; get; }

        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            this.Year = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Year");
            this.Month = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Month");
            this.Search = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Search");
            this.File = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".File");
        }
    }
}