using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;
using BI_Project.Services.User;
using BI_Project.Models.UI;
using BI_Project.Helpers;
using BI_Project.Services.Menus;
using BI_Project.Models.EntityModels;

namespace BI_Project.Controllers
{
    public class BaseController : Controller
    {
        public string SESSION_LANGUAGE_NAME { set; get; }


        public string LANGUAGE_FOLDER { set; get; }
        public string LANGUAGE { set; get; }

        public JObject LANGUAGE_OBJECT { set; get; }
        public string LOG_FOLDER { set; get; }

        public string LOG_FILE_ID { set; get; }

        public string CONFIG_FOLDER { set; get; }
        public string THEME_FOLDER { set; get; }
        public string THEME_ACTIVE { set; get; }
        /// <summary>
        /// used to set mode of the system is debug or not
        /// 
        /// </summary>
        public bool IS_DEBUG { set; get; }

        public string ERRORS { set; get; }

        public string CONNECT_STRING_STAGING { set; get; }
        public string CONNECTION_STRING { set; get; }

        public string SESSION_NAME_USERID { get; set; }
        public string SESSION_NAME_USER_NAME { get; set; }
        public Services.DBConnection DBConnection { set; get; }
        public Services.DBConnection LrsConnection { get; set; }
        public BaseController()
        {
            //DBConnection = new Services.DBConnection();
            this.SetProperty();
            // this.GetLanguage();
        }

        private void SetProperty()
        {
            this.CONNECT_STRING_STAGING = WebConfigurationManager.AppSettings["CONNECT_STRING_STAGING"];
            this.CONNECTION_STRING = WebConfigurationManager.AppSettings["CONNECT_STRING"];
            this.SESSION_NAME_USERID = WebConfigurationManager.AppSettings["SESSION_NAME_USERID"];
            this.LOG_FOLDER = WebConfigurationManager.AppSettings["LOG_FOLDER"];
            this.THEME_ACTIVE = WebConfigurationManager.AppSettings["THEME_ACTIVE"];
            this.THEME_FOLDER = WebConfigurationManager.AppSettings["THEME_FOLDER"];
            this.SESSION_LANGUAGE_NAME = WebConfigurationManager.AppSettings["SESSION_LANGUAGE_NAME"];
            this.LANGUAGE_FOLDER = WebConfigurationManager.AppSettings["LANGUAGE_FOLDER"];

            this.CONFIG_FOLDER = WebConfigurationManager.AppSettings["CONFIG_FOLDER"];
        }

        public void GetLanguage()
        {
            if (null == Session[this.SESSION_LANGUAGE_NAME])
            {
                this.LANGUAGE = "vi";
                Session[this.SESSION_LANGUAGE_NAME] = this.LANGUAGE;
            }

            else this.LANGUAGE = Session[this.SESSION_LANGUAGE_NAME].ToString();

            this.LANGUAGE_OBJECT = BI_Project.Helpers.Utility.JTokenHelper.GetLanguage("~/" + this.LANGUAGE_FOLDER, this.LANGUAGE);
            ViewData["VIEWDATA_LANGUAGE"] = this.LANGUAGE_OBJECT;
        }

        public void SetConnectionDB()
        {
            this.DBConnection = new Services.DBConnection(this.CONNECTION_STRING);
            this.LrsConnection = new Services.DBConnection(this.CONNECTION_STRING);
        }

        //public void SetConnectionOracleDB()
        //{
        //    oracleConnection = new Services.ConnectOracleDB(CONNECT_STRING_STAGING);
        //}
        /// <summary>
        /// GET THE DATA USE FOR COMMON TARGET AS MENUS,....
        /// </summary>
        public void SetCommonData()
        {

            //this.SetConnectionDB();
            //BI_Project.Services.User.UserServices userServices = new UserServices(this.DBConnection);
            //ViewData["block_menu_left_data"] = userServices.GetListMenus((int)Session[this.SESSION_NAME_USERID],    (bool) Session["IsAdmin"]);

            this.SetConnectionDB();
            UserServices userServices = new UserServices(DBConnection);

            EntityUserModel currentUser = new EntityUserModel();
            if (Session[SESSION_NAME_USERID] != null)
            {
                currentUser = userServices.GetEntityById((int)Session[SESSION_NAME_USERID]);

                ViewData["block_menu_left_data"] = userServices.GetListMenus(currentUser);
                var it = ViewData["block_menu_left_data"];
                MenuServices menuServices = new MenuServices(DBConnection);

                var menuData = menuServices.GetMenusByDepId(currentUser.UserId, currentUser.DeptId);

                ViewData["MenuHeaderData"] = menuData;
            }
        }


        public void SetMessageText(BlockLanguageModel blockLanguage)
        {
            //string output = null;
            try
            {
                if (Session["msgtype"] != null)
                {
                    int msgType = Int32.Parse(Session["msgtype"].ToString());
                    ViewBag.Message = blockLanguage.GetMessage(msgType);

                    Session["msgtype"] = null;
                    Session.Remove("msgtype");
                }
            }
            catch (Exception ex)
            {

            }
            //return output;
        }

        public ActionResult CheckPermission()
        {
            bool hasPermission = true;

            if (!hasPermission)
            {
                return RedirectToAction("Home", "Logout");
            }
            return Content("");
        }

        public ActionResult CheckAdminPermission()
        {
            if (Session["IsAdmin"] == null || (bool)Session["IsAdmin"] == false)
            {
                //RedirectToAction("Logout", "Home");
                Server.Transfer("Home/Logout");
                return Content("<script>window.location = 'http://www.example.com';</script>");
            }
            return Content("");
        }


        public void SaveAccessLog(string functionname)
        {
            Logging.WriteToLog(this.GetType().ToString() + "-" + functionname, LogType.Access);
        }
    }
}