using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BI_Project.Models.EntityModels;
using System.Text.RegularExpressions;
namespace BI_Project.Helpers
{
    public static class UIHelper
    {

        public static string GeneralTreeFantacy(string baseUrl, Dictionary<string, string> dicExtraClass, List<EntityMenuModel> lstMenus, List<int> lstAllowedMenus)
        {
            string output = "";

            int beforelevel = 1, currentLevel = 1, i = 0;
            string ul_class = "";
            string ul_open = "<ul ", li_open = "<li ", li_close = "</li>";
            string strMenus = "";
            string i_tag = "<i class='icon-bars-alt'></i>";
            string i_tagTree = "<i class='icon-tree6'></i>";

            if (dicExtraClass != null) ul_class = dicExtraClass["ul_class"];

            ul_open += " class =\"" + ul_class + "\"  ";
            List<Dictionary<string, string>> textMenus = new List<Dictionary<string, string>>();

            foreach (EntityMenuModel menu in lstMenus)
            {
                Dictionary<string, string> textItem = new Dictionary<string, string>();
                textMenus.Add(textItem);


                string a_open = "<a href=\"" + baseUrl + "/" + menu.Path + "/" + menu.MenuId + "\"> ";
                string a_text = menu.Name;

                string[] lstLevel = Regex.Split(menu.LevelTree + "@@@abc", "@@@");
                currentLevel = lstLevel.Length - 1;
                int gap = currentLevel - beforelevel;
                //WHEN NEXT TO SUB MENU 
                if (gap > 0)
                {
                    //string str_pan = "<a href=\"#dropdown\"" + i.ToString() + " data-toggle=\"collapse\" >" +                        " </a>";

                    textMenus[i - 1]["ul_open"] = ul_open + " id=\"dropdown" + i.ToString() + "\" >";
                    textMenus[i - 1]["a_text"] = textMenus[i - 1]["a_text"];


                }
                //SET VALUE FOR ALL NORMAL ELEMENTS

                textMenus[i]["open_li"] = li_open + " class ='icon_" + menu.MenuId.ToString() + "' id='" + menu.MenuId + "'" + " >";
                if (lstAllowedMenus.Contains(menu.MenuId)) textMenus[i]["open_li"] = li_open + " class ='selected icon_" + menu.MenuId.ToString() + "' id='" + menu.MenuId + "'" + " >";

                textMenus[i]["a_open"] = a_open;
                textMenus[i]["a_text"] = a_text;

                if (gap == 0 && i > 0)
                {
                    textMenus[i - 1]["close_li"] = li_close;
                    // textMenus[i]["close_li"] = li_close;
                }



                if (gap < 0)
                {
                    textMenus[i - 1]["close_li"] = "";
                    while (gap < 0)
                    {
                        textMenus[i - 1]["close_li"] += "</li></ul>";
                        gap++;
                    }
                    textMenus[i - 1]["close_li"] += "</li>";
                }





                beforelevel = currentLevel;
                i++;
            }

            if (currentLevel > 1)
            {
                textMenus[i - 1]["close_li"] = "";
                while (currentLevel > 1)
                {
                    textMenus[i - 1]["close_li"] += "</li></ul>";
                    currentLevel--;
                }

                textMenus[i - 1]["close_li"] += "</li>";
            }



            foreach (Dictionary<string, string> menuItem in textMenus)
            {
                if (menuItem.Keys.Contains<string>("ul_close"))
                {
                    strMenus += menuItem["ul_close"];
                }
                strMenus += menuItem["open_li"];

                if (baseUrl != "")
                {
                    strMenus += menuItem["a_open"] + i_tag + "<span   style = 'display: flex;text - overflow: ellipsis;'> " + menuItem["a_text"] + "</span>" + "</a>";
                }
                else strMenus += "<span>" + menuItem["a_text"] + "</span>";

                if (menuItem.Keys.Contains<string>("ul_open"))
                {
                    strMenus += menuItem["ul_open"];
                }
                if (menuItem.ContainsKey("close_li"))
                {
                    strMenus += menuItem["close_li"];
                }

            }
            output += strMenus + "";

            //string inputTagHtml = " <input type='hidden' id ='" + inputagName + "' name='" + inputagName + "' value='";
            //foreach(int menuSelected in lstAllowedMenus)
            //{
            //    inputTagHtml += menuSelected.ToString() + ",";
            //}
            //inputTagHtml = inputTagHtml.TrimEnd(',') +"' />" ;
            //inputagName = inputTagHtml;

            return output;

        }
        public static string GeneralReportFantacy(string baseUrl, Dictionary<string, string> dicExtraClass, List<EntityReportBIModel> lstReport, List<int> lstAllowReport)
        {
            string strMenus = "";

            foreach (EntityReportBIModel report in lstReport)
            {
                strMenus = strMenus + "<li>" + report.ReportName + "</li>";
            }       
            //string inputTagHtml = " <input type='hidden' id ='" + inputagName + "' name='" + inputagName + "' value='";
            //foreach(int menuSelected in lstAllowedMenus)
            //{
            //    inputTagHtml += menuSelected.ToString() + ",";
            //}
            //inputTagHtml = inputTagHtml.TrimEnd(',') +"' />" ;
            //inputagName = inputTagHtml;

            return strMenus;

        }


        public static string GeneralTreeFantacy(string baseUrl, string url, Dictionary<string, string> dicExtraClass, List<EntityMenuModel> lstMenus, List<int> lstAllowedMenus)
        {
            string output = "";

            int beforelevel = 1, currentLevel = 1, i = 0;
            string ul_class = "";
            string href = "";


            string ul_open = "<ul ", li_open = "<li ", li_close = "</li>";
            string strMenus = "";
            string i_tag = "<i class='icon-list'></i>";
            string i_tagTree = "<i class='icon-tree6'></i>";
            int levelMax = 0;
            List<int> levels = new List<int>();

            if (dicExtraClass != null) ul_class = dicExtraClass["ul_class"];

            ul_open += " class =\"" + ul_class + "\"  ";
            List<Dictionary<string, string>> textMenus = new List<Dictionary<string, string>>();

            foreach (EntityMenuModel menu in lstMenus)
            {
                Dictionary<string, string> textItem = new Dictionary<string, string>();
                textMenus.Add(textItem);

                string[] lstLevel = Regex.Split(menu.LevelTree + "@@@abc", "@@@");
                currentLevel = lstLevel.Length - 1;
                levels.Add(currentLevel);
            }
            if (levels.Count != 0)
                levelMax = levels.Max();

            foreach (EntityMenuModel menu in lstMenus)
            {
                i_tag = "<i class='" + (menu.IconCSS != null && menu.IconCSS.Trim() != "" ? menu.IconCSS : "icon-list") + "'></i>";
                Dictionary<string, string> textItem = new Dictionary<string, string>();
                //textMenus.Add(textItem);
                string[] lstLevel = Regex.Split(menu.LevelTree + "@@@abc", "@@@");
                currentLevel = lstLevel.Length - 1;
                int gap = currentLevel - beforelevel;

                href = baseUrl + "/" + menu.Path + "/" + menu.MenuId;

                string a_open = "<a href=\"" + baseUrl + "/" + menu.Path + "/" + menu.MenuId + "\"> ";
                string a_text = menu.Name;


                if (href == url)
                {
                    li_open += "class=\"currentactive level-" + currentLevel + "\"";
                }
                else
                {
                    li_open += "class=\"level-" + currentLevel + "\"";
                }

                //WHEN NEXT TO SUB MENU 
                if (gap > 0)
                {
                    //string str_pan = "<a href=\"#dropdown\"" + i.ToString() + " data-toggle=\"collapse\" >" +                        " </a>";
                    try
                    {
                        textMenus[i - 1]["ul_open"] = ul_open + " id=\"dropdown" + i.ToString() + "\" >";
                        textMenus[i - 1]["a_text"] = textMenus[i - 1]["a_text"];
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }

                }
                //SET VALUE FOR ALL NORMAL ELEMENTS

                textMenus[i]["open_li"] = li_open + " class ='icon_" + menu.MenuId.ToString() + "' id='" + menu.MenuId + "'" + " >";
                if (lstAllowedMenus.Contains(menu.MenuId)) textMenus[i]["open_li"] = li_open + " class ='selected icon_" + menu.MenuId.ToString() + "' id='" + menu.MenuId + "'" + " >";

                textMenus[i]["a_open"] = a_open;
                textMenus[i]["a_text"] = a_text;

                if (gap == 0 && i > 0)
                {
                    textMenus[i - 1]["close_li"] = li_close;
                }



                if (gap < 0)
                {
                    textMenus[i - 1]["close_li"] = "";
                    while (gap < 0)
                    {
                        textMenus[i - 1]["close_li"] += "</li></ul>";
                        gap++;
                    }
                    textMenus[i - 1]["close_li"] += "</li>";
                }





                beforelevel = currentLevel;
                li_open = "<li ";
                i++;
            }

            if (currentLevel > 1)
            {
                textMenus[i - 1]["close_li"] = "";
                while (currentLevel > 1)
                {
                    textMenus[i - 1]["close_li"] += "</li></ul>";
                    currentLevel--;
                }

                textMenus[i - 1]["close_li"] += "</li>";
            }



            foreach (Dictionary<string, string> menuItem in textMenus)
            {
                if (menuItem.Keys.Contains<string>("ul_close"))
                {
                    strMenus += menuItem["ul_close"];
                }
                strMenus += menuItem["open_li"];

                if (baseUrl != "")
                {
                    strMenus += menuItem["a_open"] + i_tag + "<span>" + menuItem["a_text"] + "</span>" + "</a>";
                }
                else strMenus += "<span>" + menuItem["a_text"] + "</span>";

                if (menuItem.Keys.Contains<string>("ul_open"))
                {
                    strMenus += menuItem["ul_open"];
                }
                if (menuItem.ContainsKey("close_li"))
                {
                    strMenus += menuItem["close_li"];
                }

            }
            output += strMenus + "";

            //string inputTagHtml = " <input type='hidden' id ='" + inputagName + "' name='" + inputagName + "' value='";
            //foreach(int menuSelected in lstAllowedMenus)
            //{
            //    inputTagHtml += menuSelected.ToString() + ",";
            //}
            //inputTagHtml = inputTagHtml.TrimEnd(',') +"' />" ;
            //inputagName = inputTagHtml;

            return output;

        }



        public static string GeneralUISelectTag(List<BI_Project.Models.EntityModels.EntityMenuModel> lstMenus, Dictionary<string, string> dicExtraClass)
        {
            string output = "";
            int beforelevel = 1, currentLevel = 1, i = 0;


            string optgroup_open = "<optgroup  style='font - weight:bold; font - size:14px' label=",
                option_open = "<option ", option_close = "</option>", optgroup_close = "</optgroup>";
            string strMenus = "";




            List<Dictionary<string, string>> textMenus = new List<Dictionary<string, string>>();

            foreach (EntityMenuModel menu in lstMenus)
            {
                Dictionary<string, string> textItem = new Dictionary<string, string>();
                textMenus.Add(textItem);



                string[] lstLevel = Regex.Split(menu.LevelTree + "@@@abc", "@@@");
                currentLevel = lstLevel.Length - 1;
                int gap = currentLevel - beforelevel;

                textMenus[i]["option_open"] = option_open + " value='" + menu.MenuId + "'>" + menu.Name;
                textMenus[i]["text"] = menu.Name;
                //WHEN NEXT TO SUB MENU 
                if (gap > 0)
                {

                    textMenus[i - 1]["optgroup_open"] = optgroup_open;

                }
                //SET VALUE FOR ALL NORMAL ELEMENTS


                if (gap == 0 && i > 0)
                {
                    textMenus[i]["option_close"] = option_close;

                    textMenus[i - 1]["is_option_open"] = "1";
                    // textMenus[i]["close_li"] = li_close;
                }



                if (gap < 0)
                {
                    textMenus[i - 1]["is_option_open"] = "1";
                    textMenus[i - 1]["option_close"] = "";
                    while (gap < 0)
                    {
                        textMenus[i - 1]["option_close"] += option_close + optgroup_close;
                        gap++;
                    }
                    textMenus[i - 1]["option_close"] += option_close;
                }





                beforelevel = currentLevel;
                i++;
            }

            if (currentLevel > 1)
            {
                textMenus[i - 1]["is_option_open"] = "1";
                textMenus[i - 1]["option_close"] = "";
                while (currentLevel > 1)
                {
                    textMenus[i - 1]["option_close"] += option_close + optgroup_close;
                    currentLevel--;
                }
                textMenus[i - 1]["option_close"] += option_close;
            }



            foreach (Dictionary<string, string> menuItem in textMenus)
            {
                if (menuItem.Keys.Contains("is_option_open"))
                {
                    strMenus += menuItem["option_open"];
                }
                else
                {
                    strMenus += menuItem["optgroup_open"] + "'" + menuItem["text"] + "' >";
                }
                if (menuItem.Keys.Contains("option_close"))
                    strMenus += menuItem["option_close"];
            }
            output += strMenus + "";


            return output;
        }
    }

}