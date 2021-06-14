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

namespace BI_Project.Controllers
{
    public class BCGBController : BaseController
    {
        public ActionResult List()
        {
            this.SetCommonData();
            ViewData["pagename"] = "BCGB";
            ViewData["action_block"] = "BCGB/BCGB";
            this.GetLanguage();
            this.SetConnectionDB();
            BCGBServices services = new BCGBServices(this.DBConnection);

            //hanv
            BlockLangGBTaskListModel blockLang = new BlockLangGBTaskListModel();
            BI_Project.Models.UI.BlockModel blockModelGBTask = new Models.UI.BlockModel("block_task_list", this.LANGUAGE_OBJECT, blockLang);
            GBTaskServices GBTaskservices = new GBTaskServices(this.DBConnection);
            string id = (Request.QueryString["ReportRequirementId"] == null ? "0" : Request.QueryString["ReportRequirementId"].ToString());
            int ReportRequirementId = Convert.ToInt32(id);
            ViewData["ReportRequirementId"] = ReportRequirementId;
            blockModelGBTask.DataModel = GBTaskservices.GetList(ReportRequirementId);

            List<EntityGBRPTEvaluateModel> lstEvaluate = GBTaskservices.GetListEvaluate(ReportRequirementId);
            ViewData["Evaluates"] = lstEvaluate;

            DepartmentServices departmentServices = new DepartmentServices(this.DBConnection);
            ViewData["departments"] = departmentServices.GetList();
            ViewData["LISTTASK"] = GBTaskservices.GetList(ReportRequirementId);
            //end

            BlockModel blockModel = new BlockModel("BCGB");
            blockModel.DataModel = services.GetList("", ReportRequirementId);
            List<EntityReportBIModel> listReportConfirm = services.GetList("", ReportRequirementId);
            BI_Project.Services.ReportRequire.ReportRequireModel objtRequirementSelected = new Services.ReportRequire.ReportRequireService(this.DBConnection).GetReportById(ReportRequirementId);

            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            Logging.WriteToLog(this.GetType().ToString() + "-List()", LogType.Access);

            ViewData["BlockData"] = blockModel;
            //ViewData["ListReportBI"] = services.GetListReportBI(ReportRequirementId);
            ViewData["ReportRequirementSelected"] = objtRequirementSelected;
            ViewData["reportList"] = listReportConfirm;


            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        public ActionResult ViewDetail()
        {
            this.SetCommonData();
            ViewData["pagename"] = "ViewDetail";
            ViewData["action_block"] = "BCGB/ViewDetail";
            this.GetLanguage();
            this.SetConnectionDB();
            BCGBServices services = new BCGBServices(this.DBConnection);

            //hanv
            BlockLangGBTaskListModel blockLang = new BlockLangGBTaskListModel();
            BI_Project.Models.UI.BlockModel blockModelGBTask = new Models.UI.BlockModel("block_task_list", this.LANGUAGE_OBJECT, blockLang);
            GBTaskServices GBTaskservices = new GBTaskServices(this.DBConnection);
            string id = (Request.QueryString["ReportRequirementId"] == null ? "0" : Request.QueryString["ReportRequirementId"].ToString());
            int ReportRequirementId = Convert.ToInt32(id);
            ViewData["ReportRequirementId"] = ReportRequirementId;
            blockModelGBTask.DataModel = GBTaskservices.GetList(ReportRequirementId);

            DepartmentServices departmentServices = new DepartmentServices(this.DBConnection);
            ViewData["departments"] = departmentServices.GetList();
            string DepartCode = "";
            if (Session["CodeIsAdmin"] != null)
                DepartCode = Session["CodeIsAdmin"].ToString();
            string DepartId = "";
            if (Session["DepartIdUserLogin"] != null)
                DepartId = Session["DepartIdUserLogin"].ToString();
            ViewData["LISTTASK"] = GBTaskservices.GetList(ReportRequirementId);
            //end

            List<EntityReportBIModel> listReportConfirm = services.GetList("", ReportRequirementId);
            BI_Project.Services.ReportRequire.ReportRequireModel objtRequirementSelected = new Services.ReportRequire.ReportRequireService(this.DBConnection).GetReportById(ReportRequirementId);

            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            Logging.WriteToLog(this.GetType().ToString() + "-List()", LogType.Access);

            ViewData["reportList"] = listReportConfirm;
            ViewData["ReportRequirementSelected"] = objtRequirementSelected;
            
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        //không hiểu gì hết trơn
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
            BCGBServices services = new BCGBServices(this.DBConnection);
            List<EntityReportConfirmHistoryModel> ListReportConfirmHistory = services.GetListReportConfirmHistory(ReportRequirementId, ReporBI_Id);
            this.GetLanguage();
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");
            ViewData["ListReportConfirmHistory"] = ListReportConfirmHistory;
            return PartialView("_ReportConfirmHistory", new ViewDataDictionary {
                {"ListReportConfirmHistory", ListReportConfirmHistory }
            });
        }
        
        public ActionResult ViewReportBI(int id)
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


            EntityReportBIModel _report = new ReportBIServices(DBConnection).GetEntityById(id);
            EntityUserTimeModel logger = (EntityUserTimeModel)Session["Logger"];
            logger.Dashboard = _report.ReportName;
            if (logger != null)
            {
                this.SetConnectionDB();
                UserServices userServices = new UserServices(this.DBConnection);
                var insertlog = userServices.UpdateLogUserDashboard(logger);
            }
            UserServices _userServices = new UserServices(DBConnection);

            DepartmentServices _departmentServices = new DepartmentServices(DBConnection);

            EntityDepartmentModel _entityDepartmentModel = new EntityDepartmentModel();

            param.Site_Root = _report.ReportCode;
            param.Ticket = BI_Core.Helpers.TableauHelper.GetTicket(_report.ReportCode);

            param.TableauUrl = _report.UrlLink;
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
                param.GetFilter(id);
            }
            Random rd = new Random();
            int item = rd.Next(100, 999);
            string log = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) + "_" + item;
            if (_menuServices.ERROR != null) FileHelper.SaveFile(new { ERROR = _menuServices.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            //FileHelper.SaveFile(_entityMenuModel, this.LOG_FOLDER + "/MenuModel_" + log + ".txt");
            FileHelper.SaveFile(param.Ticket, this.LOG_FOLDER + "/Ticket_" + log + ".txt");

            return PartialView("_TableauView", param);
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

        public JsonResult GetConfirmById(int ReportRquireId, int ReporBI_Id)
        {
            this.SetConnectionDB();
            BCGBServices services = new BCGBServices(this.DBConnection);
            EntityReportConfirmModel model = new EntityReportConfirmModel();
            model = services.getRCById(ReportRquireId, ReporBI_Id);
            EntityReportBIModel objReport = new BI_Project.Services.ReportBi.ReportBIServices(this.DBConnection).GetEntityById(ReporBI_Id);
            if (objReport != null)
            {
                model.ReportBIName = objReport.ReportName;
            }
            model.ReportRequirementId = ReportRquireId;
            model.ReportId = ReporBI_Id;

            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);
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
            model.Created = DateTime.Now;
           
            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);
            ViewData["data_form"] = model;

            this.GetLanguage();

            this.SetConnectionDB();
            BCGBServices services = new BCGBServices(this.DBConnection);
            // check tài khoản đã tồn tại trong hệ thống hay chưa
            model.ConfirmDate = DateTime.Now;
            //var checkData = services.FindById(model.Id);
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
            TempData["data"] = model;
            List<EntityReportBIModel> reportList = services.GetList("", model.ReportRequirementId);
            ViewData["reportList"] = reportList;
            return PartialView("_Reload", new ViewDataDictionary {
                { "reportList", reportList },
                { "ReportRequirementId", model.ReportRequirementId}
            });
        }

        [HttpPost]
        public ActionResult DepartSaveConfirm(EntityReportConfirmModel model)
        {
            model.Created = DateTime.Now;

            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);
            ViewData["data_form"] = model;

            this.GetLanguage();

            this.SetConnectionDB();
            BCGBServices services = new BCGBServices(this.DBConnection);
            // check tài khoản đã tồn tại trong hệ thống hay chưa
            model.ConfirmDate = DateTime.Now;
            //var checkData = services.FindById(model.Id);
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
            TempData["data"] = model;
            List<EntityReportBIModel> reportList = services.GetList("", model.ReportRequirementId);
            ViewData["reportList"] = reportList;
            return PartialView("_Reload", new ViewDataDictionary {
                { "reportList", reportList },
                { "ReportRequirementId", model.ReportRequirementId}
            });
        }
    }
}