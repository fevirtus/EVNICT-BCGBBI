using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.IO;

using BI_Project.Services.Importers;
using BI_Project.Models.EntityModels;
using BI_Project.Models.UI;
using BI_Project.Helpers;
using BI_Project.Helpers.Utility;

using BI_Core.Tableau;
using BI_Project.Services.Menus;
using System.Configuration;
using bicen.Services;
using BI_Project.Services.User;
using BI_Project.Services.Departments;
using System.Text;
using bicen.Models.EntityModels;

namespace BI_Project.Controllers
{
    public class DashboardTableauController : BaseController
    {
        public string EXCEL_UPLOAD_FOLDER { set; get; }


        public DashboardTableauController() : base()
        {
            this.EXCEL_UPLOAD_FOLDER = WebConfigurationManager.AppSettings["EXCEL_UPLOAD_FOLDER"];
            //this.DBConnection.ConnectString = this.CONNECT_STRING_STAGING;
        }

        [HttpGet]
        [CheckUserMenus]
        public ActionResult Index(int id)
        {
            //lay url tu menu voi id

            ViewData["pagename"] = "Embed_Tableau";
            ViewData["action_block"] = "Tableau/TableauView";

            SetCommonData();
            GetLanguage();
            SetConnectionDB();


            BI_Project.Models.UI.PageModel pageModel = new Models.UI.PageModel("Embed_Tableau");
            // BI_Project.Models.UI.BlockModel blockModel = new BlockModel("TableauView");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            //pageModel.H1Title = pageModel.GetElementByPath("page_excel.menu" + id + ".h1");
            pageModel.Title = pageModel.GetElementByPath("title");
            ViewData["page_model"] = pageModel;

            TableauModel param = new TableauModel();
            ViewData["BlockData"] = param;
            MenuServices _menuServices = new MenuServices(DBConnection);


            EntityMenuModel _entityMenuModel = _menuServices.GetMenuModel(id.ToString());
            EntityUserTimeModel logger = (EntityUserTimeModel)Session["Logger"];
            logger.Dashboard = _entityMenuModel.Name;
            if (logger != null)
            {
                this.SetConnectionDB();
                UserServices userServices = new UserServices(this.DBConnection);
                var insertlog = userServices.UpdateLogUserDashboard(logger);
            }
            UserServices _userServices = new UserServices(DBConnection);

            DepartmentServices _departmentServices = new DepartmentServices(DBConnection);

            EntityDepartmentModel _entityDepartmentModel = new EntityDepartmentModel();

            //param.Site_Root = _entityMenuModel.Site_Root;
            param.Ticket = BI_Core.Helpers.TableauHelper.GetTicket("");

            param.TableauUrl = _entityMenuModel.TableauUrl;
            //param.TableauUrl = "BCPTNPTD_1/Dashboard1?:iid=2";
            param.Hidden = 1;
            param.username = Session["UserName"].ToString();
            ViewBag.Id = id;

            var listFilter01 = _departmentServices.GetList().Select(x => x.Filter01).ToArray();

            StringBuilder builderOrganization = new StringBuilder();
            foreach (var _list in listFilter01)
            {
                builderOrganization.Append(_list).Append(',');
            }


            string _resultListOrganization = builderOrganization.ToString().TrimEnd(',');
            ViewBag.ListDepartment = _resultListOrganization;
            if (Session["CodeIsAdmin"] == null) Session["Filter01IsAdmin"] = "P";
            //if (Session["IsAdmin"] is false && (Session["Filter01IsAdmin"].ToString() != "PE" || Session["Filter01IsAdmin"].ToString() != "PA" || Session["Filter01IsAdmin"].ToString() != "PB" || Session["Filter01IsAdmin"].ToString() != "PC" || Session["Filter01IsAdmin"].ToString() != "PD"))
            if(Session["CodeIsAdmin"].ToString() != "P")
            {
                param.filter = Session["CodeIsAdmin"].ToString();
                param.GetFilter(id);
            }
            Random rd = new Random();
            int item = rd.Next(100, 999);
            string log = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) + "_" + item;
            if (_menuServices.ERROR != null) FileHelper.SaveFile(new { ERROR = _menuServices.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            FileHelper.SaveFile(_entityMenuModel, this.LOG_FOLDER + "/MenuModel_" + log + ".txt");
            FileHelper.SaveFile(param.Ticket, this.LOG_FOLDER + "/Ticket_" + log + ".txt");
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");


        }


    }
}