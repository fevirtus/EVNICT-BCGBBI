using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Models.UI
{
    public enum MessageType { Success = 1, ServerError =-2 , BusinessError =-1}
    public class BlockLanguageModel
    {
        public string BlockTitle { set; get; }

        public string BlockName { set; get; }

        public string MessageSuccess { set; get; }

        public string MessageServerError { set; get; }

        public string MessageBusinessError { set; get; }
        public virtual void SetLanguage(string blockName,Object languageObject)
        {
            BlockName = blockName;
            this.BlockTitle =  BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".title");
            this.MessageSuccess = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "messages." + this.BlockName + ".success");
            this.MessageBusinessError = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "messages." + this.BlockName + ".error_business_1");
            this.MessageServerError = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "commons.messages.ServerError");
        }
        public virtual void SetLanguage(Object languageObject)
        {
            this.BlockTitle = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "blocks." + this.BlockName + ".title");
            this.MessageSuccess = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "messages." + this.BlockName + ".success");
            this.MessageBusinessError = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "messages." + this.BlockName + ".error_business_1");
            this.MessageServerError = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "commons.messages.ServerError");
        }

        public  string GetLangByPath(string path,Object languageObject)
        {
            string output = "";

            try
            {
                output = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, path);
            }
            catch(Exception ex)
            {

            }

            return output;
        }
        public string GetMessage(int t )
        {
            string output = "";
            switch (t)
            {
                case 1:
                    output = this.MessageSuccess;
                    break;
                case -2:
                    output = this.MessageServerError;
                    break;
                case -1:
                    output = this.MessageBusinessError;
                    break;
                default:
                    output = this.MessageSuccess;
                    break;
            }
            return output;
        }


        public static string GetElementLang(Object languageObject, string path)
        {
            string output = "";
            output =  BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, path);
            return output;
        }
    }
}