using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BI_Project.Models.UI;
namespace BI_Project.Services.Importers
{
    public class BlockLangExcelUpload : BlockLanguageModel
    {
        public string Username { set; get; }
        public string Fullname { set; get; }
        public string Uploadedtime { set; get; }
        public string Uploadmonth { set; get; }
        public string Uploadyear { set; get; }
        public string Dowload    { set; get; }
        public string Page_next { set; get; }

        public string Page_pre { set; get; }
        public string Btnupload { set; get; }

        public string Filehoder { set; get; }
        public string Helpdoc { set; get; }

        public string PerPage { set; get; }
        public string Year { set; get; }
        public string Month { set; get; }
        
        public string Search { set; get; }

        public string NumberInsertedRow { set; get; }
        public string YearLable { get; set; }
        public string YearSelect { get; set; }
        public string MonthSelect { get; set; }

        public string Export { get; set; }
        public string ExportOff { get; set; }

        public override void SetLanguage(object languageObject)
        {
            base.SetLanguage(languageObject);

            this.Username = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Username");
            this.Fullname = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Fullname");
            this.Uploadedtime = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Uploadedtime");
            this.Uploadmonth = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Uploadmonth");
            this.Uploadyear = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Uploadyear");
            this.Helpdoc = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Helpdoc");

            this.Dowload = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Dowload");
            this.Btnupload = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Btnupload");
            this.Btnupload = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Btnupload");
            this.Export = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Export");
            this.ExportOff = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Export_of");

            this.Filehoder = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Filehoder");
            this.Year = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Year");
            this.Month = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Month");
            this.PerPage = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".PerPage");
            this.Search = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".Search");
            this.NumberInsertedRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".NumberInsertedRow.#cdata-section");
            this.YearLable = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".year_lable");
            this.YearSelect = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".year_select");
            this.MonthSelect = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".month_select");


        }
    }
}