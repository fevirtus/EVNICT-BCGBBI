using BI_Project.Controllers;
using BI_Project.Helpers;
using BI_Project.Helpers.Utility;
using BI_Project.Models.UI;
using BI_Project.Services.BCGB;
using BI_Project.Services.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BI_Core.Tableau;
using BI_Project.Services.Menus;
using BI_Project.Models.EntityModels;
using BI_Project.Services.ReportRequire;
using BI_Project.Services.User;
using System.Text;
using BI_Project.Services.GBTask;
using Newtonsoft.Json;
using System.Web;
using System.IO;
using bicen.Models.EntityModels;
using BI_Project.Services.ReportBi;
using System.Globalization;

namespace BI_Project.Controllers
{
    public class BCGBController : BaseController
    {
        public ActionResult List()
        {
            //setup connection
            this.SetCommonData();            
            this.SetConnectionDB();
            this.GetLanguage();
            BCGBServices services = new BCGBServices(this.DBConnection);
            GBTaskServices GBTaskservices = new GBTaskServices(this.DBConnection);
            DepartmentServices departmentServices = new DepartmentServices(this.DBConnection);
            ReportRequireService reportRequireServices = new ReportRequireService(this.DBConnection);

            //get current departId
            string DepartId = "";
            if (Session["DepartIdUserLogin"] != null)   DepartId = Session["DepartIdUserLogin"].ToString();
            //get current departCode
            string DepartCode = "";
            if (Session["CodeIsAdmin"] != null) DepartCode = Session["CodeIsAdmin"].ToString();
            //get reportRequireId
            string ReportRequirementIdStr = (Request.QueryString["ReportRequirementId"] == null ? "0" : Request.QueryString["ReportRequirementId"].ToString());
            int ReportRequirementId = Convert.ToInt32(ReportRequirementIdStr);
            //get list evaluate by reportRequireId
            List<EntityGBRPTEvaluateModel> lstEvaluate = GBTaskservices.GetListEvaluate(ReportRequirementId);
            //get list all department
            List<EntityDepartmentModel> lstDepartment = departmentServices.GetList();
            //get list Task order by reportRequireId and DepartmentId
            List<EntityGBTaskModel> LISTTASK = new List<EntityGBTaskModel>();
            switch (DepartCode)
            {
                case "KH":
                    LISTTASK = GBTaskservices.GetList(ReportRequirementId);
                    break;
                default:
                    LISTTASK = GBTaskservices.GetList(ReportRequirementId, DepartCode);
                    break;
            }
            //get list Report Confirm order by reportRequireId and DepartmentId
            List<EntityReportBIModel> listReportConfirm = services.GetList("", ReportRequirementId);
            //get a record reportRequirement by Id
            ReportRequireModel objtRequirementSelected = reportRequireServices.GetReportById(ReportRequirementId);
            
            //write action to log
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            Logging.WriteToLog(this.GetType().ToString() + "-List()", LogType.Access);
                        
            //send data to front
            ViewData["pagename"] = "BCGB";
            ViewData["action_block"] = "BCGB/BCGB";
            ViewData["ReportRequirementId"] = ReportRequirementId;
            ViewData["Evaluates"] = lstEvaluate;
            ViewData["departments"] = lstDepartment;
            ViewData["LISTTASK"] = LISTTASK;
            ViewData["reportList"] = listReportConfirm;
            ViewData["ReportRequirementSelected"] = objtRequirementSelected;

            //return a view (direct page)
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");

            //BI_Project.Models.UI.BlockModel blockModelGBTask = new Models.UI.BlockModel("block_task_list", this.LANGUAGE_OBJECT, blockLang);
            //blockModelGBTask.DataModel = GBTaskservices.GetList(ReportRequirementId);
            //BlockModel blockModel = new BlockModel("BCGB");
            //blockModel.DataModel = services.GetList("", ReportRequirementId);
            //ViewData["BlockData"] = blockModel;
            //ViewData["ListReportBI"] = services.GetListReportBI(ReportRequirementId);

        }

        public ActionResult ViewDetail()
        {
            //setup connection
            this.SetCommonData();
            this.SetConnectionDB();
            this.GetLanguage();
            BCGBServices services = new BCGBServices(this.DBConnection);
            GBTaskServices GBTaskservices = new GBTaskServices(this.DBConnection);
            DepartmentServices departmentServices = new DepartmentServices(this.DBConnection);
            ReportRequireService reportRequireServices = new ReportRequireService(this.DBConnection);

            //get current departCode
            string DepartCode = "";
            if (Session["CodeIsAdmin"] != null) DepartCode = Session["CodeIsAdmin"].ToString();
            //get reportRequireId
            string ReportRequirementIdStr = (Request.QueryString["ReportRequirementId"] == null ? "0" : Request.QueryString["ReportRequirementId"].ToString());
            int ReportRequirementId = Convert.ToInt32(ReportRequirementIdStr);
            //get list all department
            List<EntityDepartmentModel> lstDepartment = departmentServices.GetList();
            //get list Task order by reportRequireId and DepartmentId
            List<EntityGBTaskModel> LISTTASK = new List<EntityGBTaskModel>();
            switch (DepartCode)
            {
                case "KH":
                    LISTTASK = GBTaskservices.GetList(ReportRequirementId);
                    break;
                default:
                    LISTTASK = GBTaskservices.GetList(ReportRequirementId, DepartCode);
                    break;
            }
            //get list Report Confirm order by reportRequireId and DepartmentId
            List<EntityReportBIModel> listReportConfirm = services.GetList("", ReportRequirementId);
            //get a record reportRequirement by Id
            ReportRequireModel objtRequirementSelected = reportRequireServices.GetReportById(ReportRequirementId);

            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            Logging.WriteToLog(this.GetType().ToString() + "-List()", LogType.Access);

            ViewData["pagename"] = "BCGB";
            ViewData["action_block"] = "BCGB/ViewDetail";
            ViewData["ReportRequirementId"] = ReportRequirementId;
            ViewData["departments"] = lstDepartment;
            ViewData["LISTTASK"] = LISTTASK;
            ViewData["reportList"] = listReportConfirm;
            ViewData["ReportRequirementSelected"] = objtRequirementSelected;

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpGet]
        [CheckUserMenus]
        public ActionResult Detail()
        {
            //lay url tu menu voi id
            ViewData["pagename"] = "BCGB";
            ViewData["action_block"] = "BCGB/block_BCGB_detail";

            SetCommonData();
            GetLanguage();
            SetConnectionDB();


            PageModel pageModel = new PageModel("BCGB");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);

            pageModel.Title = pageModel.GetElementByPath("title");
            ViewData["page_model"] = pageModel;

            string id = (Request.QueryString["id"] == null ? "0" : Request.QueryString["id"].ToString());
            int ReportRequirementId = Int32.Parse((Request.QueryString["ReportRequirementId"] == null ? "0" : Request.QueryString["ReportRequirementId"].ToString()));
            TableauModel param = new TableauModel();
            ViewData["BlockData"] = param;
            MenuServices _menuServices = new MenuServices(DBConnection);


            EntityMenuModel _entityMenuModel = _menuServices.GetMenuModel(id.ToString());
            bicen.Models.EntityModels.EntityUserTimeModel logger = (bicen.Models.EntityModels.EntityUserTimeModel)Session["Logger"];
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
            if (Session["CodeIsAdmin"].ToString() != "P")
            {
                param.filter = Session["CodeIsAdmin"].ToString();
                param.GetFilter(Int32.Parse(id));
            }
            Random rd = new Random();
            int item = rd.Next(100, 999);
            string log = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) + "_" + item;
            if (_menuServices.ERROR != null) FileHelper.SaveFile(new { ERROR = _menuServices.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            FileHelper.SaveFile(_entityMenuModel, this.LOG_FOLDER + "/MenuModel_" + log + ".txt");
            FileHelper.SaveFile(param.Ticket, this.LOG_FOLDER + "/Ticket_" + log + ".txt");
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpGet]
        public ActionResult History(int ReportRequirementId, int ReporBI_Id)
        {
            this.SetConnectionDB();
            this.GetLanguage();
            BCGBServices services = new BCGBServices(this.DBConnection);
            List<EntityReportConfirmHistoryModel> ListReportConfirmHistory = services.GetListReportConfirmHistory(ReportRequirementId, ReporBI_Id);
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");
            ViewData["ListReportConfirmHistory"] = ListReportConfirmHistory;
            return PartialView("_ReportConfirmHistory", new ViewDataDictionary {
                {"ListReportConfirmHistory", ListReportConfirmHistory }
            });
        }
        
        public ActionResult ViewReportBI(int id)
        {
            //init variable
            PageModel pageModel = new PageModel("Embed_Tableau");
            TableauModel param = new TableauModel();
            EntityDepartmentModel _entityDepartmentModel = new EntityDepartmentModel();

            //setup connection and pre attribute
            SetCommonData();
            GetLanguage();
            SetConnectionDB();
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            pageModel.Title = pageModel.GetElementByPath("title");
            MenuServices _menuServices = new MenuServices(DBConnection);
            ReportBIServices reportBIservieces = new ReportBIServices(DBConnection);
            UserServices _userServices = new UserServices(DBConnection);
            DepartmentServices _departmentServices = new DepartmentServices(DBConnection);

            //get a reportBI by Id
            EntityReportBIModel _report = reportBIservieces.GetEntityById(id);
            //update status to log
            EntityUserTimeModel logger = (EntityUserTimeModel)Session["Logger"];
            logger.Dashboard = _report.ReportName;
            if (logger != null)
            {
                this.SetConnectionDB();
                UserServices userServices = new UserServices(this.DBConnection);
                var insertlog = userServices.UpdateLogUserDashboard(logger);
            }

            //send data to front
            ViewData["pagename"] = "Embed_Tableau";
            ViewData["action_block"] = "Tableau/TableauView";
            ViewData["page_model"] = pageModel;
            ViewData["BlockData"] = param;

            //set attribute for ViewBC
            param.Site_Root = _report.ReportCode;
            param.Ticket = BI_Core.Helpers.TableauHelper.GetTicket(_report.ReportCode);
            param.TableauUrl = _report.UrlLink;
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
            if (Session["CodeIsAdmin"].ToString() != "P")
            {
                param.filter = Session["CodeIsAdmin"].ToString();
                param.GetFilter(id);
            }
            Random rd = new Random();
            int item = rd.Next(100, 999);
            string log = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "_" + item;
            if (_menuServices.ERROR != null) FileHelper.SaveFile(new { ERROR = _menuServices.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            FileHelper.SaveFile(param.Ticket, this.LOG_FOLDER + "/Ticket_" + log + ".txt");

            return PartialView("_TableauView", param);
        }

        //get a refcord Report Confirm by id and reportBI_Id to display on dialog
        public JsonResult GetConfirmById(int ReportRquireId, int ReporBI_Id)
        {
            //setup connection
            this.SetConnectionDB();
            BCGBServices services = new BCGBServices(this.DBConnection);
            ReportBIServices reportBIServices = new ReportBIServices(this.DBConnection);

            //get record report confirm what we want
            EntityReportConfirmModel model = services.getRCById(ReportRquireId, ReporBI_Id);
            //get reportBI if have and set value into reportConfirm object
            EntityReportBIModel objReport = reportBIServices.GetEntityById(ReporBI_Id);
            if (objReport != null)
            {
                model.ReportBIName = objReport.ReportName;
            }
            model.ReportRequirementId = ReportRquireId;
            model.ReportId = ReporBI_Id;

            //change this object to a json file and put it to front
            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            //return a json object
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HistoryDetail()
        {

            ViewData["pagename"] = "BCGB_historyDetail";
            ViewData["action_block"] = "BCGB/block_BCGB_historyDetail";
            ViewData["data_form"] = TempData["data"];

            string id = (Request.QueryString["id"] == null ? "0" : Request.QueryString["id"].ToString());
            int ReportRequirementId = Int32.Parse((Request.QueryString["ReportRequirementId"] == null ? "0" : Request.QueryString["ReportRequirementId"].ToString()));
            this.SetConnectionDB();
            BCGBServices services = new BCGBServices(this.DBConnection);
            BlockModel blockModel = new BlockModel("block_BCGB_historyDetail");
            ViewData["BlockData"] = blockModel;

            this.GetLanguage();
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpPost]
        public ActionResult DepartConfirm(EntityReportConfirmModel model)
        {
            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);
            ViewData["data_form"] = TempData["data"];
            // get language
            this.GetLanguage();
            if (string.IsNullOrEmpty(model.Description))
            {
                return RedirectToAction("ViewDetail");
            }
            //**************** DATABASE PROCESS*******************************************************
            this.SetConnectionDB();
            BCGBServices services = new BCGBServices(this.DBConnection);
            model.ConfirmDate = DateTime.Now;
            // check tài khoản đã tồn tại trong hệ thống hay chưa
            var checkData = services.FindById(model.Id);
            int result = 0;
            HttpPostedFileBase postedFile = model.ImageFile;

            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/Report/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string[] arrStr = postedFile.FileName.ToString().Split('.');
                string fileName = "TASK_" + model.ReportId.ToString() + "_" + model.ReportRequirementId + "." + arrStr[1];
                postedFile.SaveAs(path + Path.GetFileName(fileName));
                model.ReportPathFile = "/Uploads/Report/" + fileName;
            }
            result = services.UpdateReport(model);
            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            //***********************INSERT OR EDIT SUCCESSFULLY * *************************************************
            if (result > 0)
            {
                return RedirectToAction("ViewDetail", "BCGB", new { ReportRequirementId = model.ReportRequirementId });
            }
            TempData["data"] = model;
            return RedirectToAction("ViewDetail", "BCGB", new { ReportRequirementId = model.ReportRequirementId });
        }

        [HttpPost]
        public ActionResult Confirm(EntityReportConfirmModel model)
        {
            model.Created = DateTime.Now;
            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);
            ViewData["data_form"] = TempData["data"];
            // get language
            this.GetLanguage();
            if (string.IsNullOrEmpty(model.Description))
            {
                return RedirectToAction("List");
            }
            //**************** DATABASE PROCESS*******************************************************
            this.SetConnectionDB();
            BCGBServices services = new BCGBServices(this.DBConnection);
            // check tài khoản đã tồn tại trong hệ thống hay chưa
            model.ConfirmDate = DateTime.Now;
            var checkData = services.FindById(model.Id);
            int result = 0;
            HttpPostedFileBase postedFile = model.ImageFile;

            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/Report/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string[] arrStr = postedFile.FileName.ToString().Split('.');
                string fileName = "TASK_" + model.ReportId.ToString() + "_" + model.ReportRequirementId + "." + arrStr[1];
                postedFile.SaveAs(path + Path.GetFileName(fileName));
                model.ReportPathFile = "/Uploads/Report/" + fileName;
            }
            result = services.UpdateReport(model);
            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            //***********************INSERT OR EDIT SUCCESSFULLY * *************************************************
            if (result > 0)
            {
                return RedirectToAction("List", "BCGB", new { ReportRequirementId = model.ReportRequirementId });
            }
            TempData["data"] = model;
            return RedirectToAction("List", "BCGB", new { ReportRequirementId = model.ReportRequirementId });
        }

        [HttpPost]
        public ActionResult Save(EntityReportConfirmModel model)
        {
            //setup connection
            this.SetConnectionDB();
            BCGBServices services = new BCGBServices(this.DBConnection);
            List<EntityReportBIModel> reportList = services.GetList("", model.ReportRequirementId);

            //set attribute for report
            model.Created = DateTime.Now;
            model.ConfirmDate = DateTime.Now;
            //check exist file or not to create href of file
            HttpPostedFileBase postedFile = model.ImageFile;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/Report/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string[] arrStr = postedFile.FileName.ToString().Split('.');
                string fileName = "TASK_" + model.ReportId.ToString() + "_" + model.ReportRequirementId + "." + arrStr[1];
                postedFile.SaveAs(path + Path.GetFileName(fileName));
                model.ReportPathFile = "/Uploads/Report/" + fileName;
            }
            //save this report to db
            int result = services.UpdateReport(model);
                       
            //logger
            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);
            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            //set attribute for View
            ViewData["data_form"] = model;
            TempData["data"] = model;            
            ViewData["reportList"] = reportList;
            return PartialView("_Reload", new ViewDataDictionary {
                { "reportList", reportList },
                { "ReportRequirementId", model.ReportRequirementId}
            });
        }

        [HttpPost]
        public ActionResult DepartSaveConfirm(EntityReportConfirmModel model)
        {
            //setup connection
            this.SetConnectionDB();
            BCGBServices services = new BCGBServices(this.DBConnection);
            
            //set attribute for report
            model.Created = DateTime.Now;
            model.ConfirmDate = DateTime.Now;
            //check exist file or not to create href of file
            HttpPostedFileBase postedFile = model.ImageFile;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/Report/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string[] arrStr = postedFile.FileName.ToString().Split('.');
                string fileName = "TASK_" + model.ReportId.ToString() + "_" + model.ReportRequirementId + "." + arrStr[1];
                postedFile.SaveAs(path + Path.GetFileName(fileName));
                model.ReportPathFile = "/Uploads/Report/" + fileName;
            }
            //save this report to db
            int result = services.UpdateReport(model);
            List<EntityReportBIModel> reportList = services.GetList("", model.ReportRequirementId);

            //logger
            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);
            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            //set attribute for View
            ViewData["data_form"] = model;
            TempData["data"] = model;
            ViewData["reportList"] = reportList;
            return PartialView("_Reload", new ViewDataDictionary {
                { "reportList", reportList },
                { "ReportRequirementId", model.ReportRequirementId}
            });
        }

    }
}