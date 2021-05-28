using System;

namespace BI_Project.Models.UI
{
    public class PageModel
    {
        public string PageName { set; get; }
        private Object _languageObject;

        public string Title { set; get; }

        /// <summary>
        /// Used to show H1 tag inside body tag
        /// </summary>
        public string H1Title { set; get; }



        public PageModel(string pageName)
        {
            this.PageName = pageName;

        }

        public void SetLanguage(Object languageObject)
        {
            _languageObject = languageObject;
            this.Title = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(languageObject, "pages." + this.PageName + ".title");
        }

        public string GetElementByPath(string path)
        {
            string output = "";

            output = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(_languageObject, "pages." + this.PageName + "." + path);
            return output;
        }


    }
}