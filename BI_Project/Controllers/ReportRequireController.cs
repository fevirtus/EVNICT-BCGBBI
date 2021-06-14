using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BI_Project.Services.Menus;
using BI_Project.Models.EntityModels;
using BI_Project.Services.Departments;
using BI_Project.Models.UI;
using BI_Project.Helpers;
using System.Web.Script.Serialization;
using BI_Project.Services.ReportRequire;
using Newtonsoft.Json;
using BI_Project.Services.Roles;
using BI_Project.Services.ReportBi;
using static BI_Project.Helpers.UIMenuTreeHelper;

namespace BI_Project.Controllers
{
    public class ReportRequireController : BaseController
    {
        public void SetConnectionDB()
        {
            this.DBConnection = new Services.DBConnection(this.CONNECTION_STRING);
            this.LrsConnection = new Services.DBConnection(this.CONNECTION_STRING);
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetReportById(int reportId)
        {
            string value = string.Empty;
            if (reportId > 0)
            {
                this.SetConnectionDB();
                ReportRequireService reportService = new ReportRequireService(this.DBConnection);
                ReportRequireModel reportSelected = reportService.GetReportById(reportId);


                value = JsonConvert.SerializeObject(reportSelected, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            }

            return Json(value, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Save(ReportRequireCreateModel model)
        {
            int userId = (int)Session[this.SESSION_NAME_USERID];
            if (userId != null)
            {
                model.CreatorId = userId;
            }

            Boolean result = false;
            this.SetConnectionDB();
            ReportRequireService reportService = new ReportRequireService(this.DBConnection);
            if (model.Id > 0)
            {
                // update                
                int i = reportService.Update(model);
                result = true;
            }
            else
            {
                // insert
                int i = reportService.Insert(model);
                result = true;
            }

            ViewData["reportList"] = reportService.GetList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save2(ReportRequireCreateModel model)
        {
            int userId = (int)Session[this.SESSION_NAME_USERID];
            if (userId > 0)
            {
                model.CreatorId = userId;
            }

            Boolean result = false;
            this.SetConnectionDB();
            ReportRequireService reportService = new ReportRequireService(this.DBConnection);
            if (model.Id > 0)
            {
                // update                
                int i = reportService.Update(model);
                result = true;
            }
            else
            {
                // insert
                int i = reportService.Insert(model);
                result = true;
            }

            List<ReportRequireModel> reportList = reportService.GetList();
            ViewData["reportList"] = reportList;

            return PartialView("_ListRequire", new ViewDataDictionary { { "reportList", reportList } });
        }

        public JsonResult Delete(int reportId)
        {
            Boolean result = false;
            this.SetConnectionDB();
            ReportRequireService reportService = new ReportRequireService(this.DBConnection);
            int i = reportService.Delete(reportId);
            result = true;
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Delete2(int reportId)
        {
            this.SetConnectionDB();
            ReportRequireService reportService = new ReportRequireService(this.DBConnection);
            int i = reportService.Delete(reportId);

            List<ReportRequireModel> reportList = reportService.GetList();
            ViewData["reportList"] = reportList;

            return PartialView("_ListRequire", new ViewDataDictionary { { "reportList", reportList } });

        }

        public ActionResult Create()
        {
            if (null == Session[this.SESSION_NAME_USERID])
            {
                return RedirectToAction("Login", "Home");
            }
            //if ((bool)Session["IsAdmin"] == false)
            //{
            //    return RedirectToAction("Logout", "Home");
            //}
            this.SetCommonData();
            ViewData["pagename"] = "menu_create";
            ViewData["action_block"] = "Menus/block_create_menu";
            ViewData["data-form"] = TempData["data"];




            string menuId = (Request.QueryString["menuid"] == null ? "0" : Request.QueryString["menuid"].ToString());
            this.SetConnectionDB();
            MenuServices menuServices = new MenuServices(this.DBConnection);

            EntityMenuModel entityMenuModel = new EntityMenuModel();

            entityMenuModel = menuServices.GetMenuModel(menuId);
            Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);
            ViewData["CurrentOrgId"] = entityMenuModel.DeptID;
            ViewData["departments"] = departmentServices.GetList();
            ViewData["listdepartmentsadmin"] = departmentServices.GetListAdminLogin((string)Session["CodeIsAdmin"]);
            this.GetLanguage();
            ViewData["VIEWDATA_LANGUAGE"] = this.LANGUAGE_OBJECT;


            BlockCreateMenuLangModel blockLang = new BlockCreateMenuLangModel();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_create_reportrequire", this.LANGUAGE_OBJECT, blockLang);
            blockModel.DataModel = entityMenuModel;
            ViewData["BlockData"] = blockModel;
            if (menuServices.ERROR != null) BI_Project.Helpers.FileHelper.SaveFile(menuServices.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpPost]
        [CheckUserMenus]
        public ActionResult Create(EntityMenuModel menu)
        {
            if (null == Session[this.SESSION_NAME_USERID])
            {
                return RedirectToAction("Login", "Home");
            }

            //if ((bool)Session["IsAdmin"] == false)
            //{
            //    return RedirectToAction("Logout", "Home");
            //}
            this.SetConnectionDB();


            int output = 0;
            MenuServices menuServices = new MenuServices(this.DBConnection);

            output = menuServices.CreateMenu(menu);

            /****************************************RESPONSE FAILE OR SUCCESS******************************************/

            this.GetLanguage();
            BlockCreateMenuLangModel blockLang = new BlockCreateMenuLangModel();
            blockLang.BlockName = "block_menu_create";
            blockLang.SetLanguage(this.LANGUAGE_OBJECT);
            Session["msg_text"] = blockLang.GetMessage(output);
            Session["msg_code"] = output;

            if (menuServices.ERROR != null) BI_Project.Helpers.FileHelper.SaveFile(menuServices.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");

            if (menu.MenuId > 0 && output > 0)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_menu_create.success_edit", this.LANGUAGE_OBJECT);
            }
            if (output == 0)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_menu_create.error_business_1", this.LANGUAGE_OBJECT);
                //return RedirectToAction("Create?roleid=" + model.RoleId);
            }
            if (output > 0)
            {
                return RedirectToAction("List");
            }


            TempData["data"] = menu;
            return RedirectToAction("Create");

        }

        public ActionResult List(int? DeptID = null, int? userId = null, int? roleId = null)
        {
            this.SetConnectionDB();
            ReportRequireService reportService = new ReportRequireService(this.DBConnection);
            ViewData["reportList"] = reportService.GetList();


            ViewData["data-form"] = TempData["data"];

            string roleid = (Request.QueryString["roleid"] == null ? "0" : Request.QueryString["roleid"].ToString());
            this.SetConnectionDB();
            RoleServices services = new RoleServices(this.DBConnection);

            ViewData["reportbis"] = new Services.ReportBi.ReportBIServices(this.DBConnection).GetList();

            var _deptID = 0;
            if (Session["DepartIdUserLogin"] != null)

                _deptID = (int)Session["DepartIdUserLogin"];
            else
                return RedirectToAction("Login", "Home");
            //DeptID = (int)Session["DepartIdUserLogin"];
            if (DeptID == null)
            {
                DeptID = _deptID;
            }

            //CheckAdminPermission();
            SetCommonData();
            MenuServices menuServices = new MenuServices(DBConnection);

            if (!Request.IsAjaxRequest())
            {
                if (null == Session[SESSION_NAME_USERID])
                {
                    return RedirectToAction("Login", "Home");
                }
                //if ((bool)Session["IsAdmin"] == false)
                //{
                //    return RedirectToAction("Logout", "Home");
                //}

                ViewData["pagename"] = "menu_list";
                ViewData["action_block"] = "ReportRequire/block_reportrequire_list";

                GetLanguage();
                ViewData["VIEWDATA_LANGUAGE"] = LANGUAGE_OBJECT;

                BlockMenuListLangModel blockLang = new BlockMenuListLangModel();
                BlockModel blockModel = new BlockModel("block_reportrequire_list", LANGUAGE_OBJECT, blockLang);
                Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);

                ViewData["departments"] = departmentServices.GetList();
                ViewData["listdepartmentsadmin"] = departmentServices.GetListAdminLogin((string)Session["CodeIsAdmin"]);


                //blockModel.DataModel = model;
                ViewData["BlockData"] = blockModel;

                SetConnectionDB();

                var menuData = menuServices.GetMenusByDepId((int)Session[SESSION_NAME_USERID], DeptID);
                ViewData["CurrentOrgId"] = DeptID;
                ViewData["MenuData"] = menuData;
                ViewData["listcycle"] = setListCycle();

                return View("~/" + THEME_FOLDER + "/" + THEME_ACTIVE + "/index.cshtml");
            }
            else
            {
                var menuData = menuServices.GetMenusByDepId((int)Session[SESSION_NAME_USERID], DeptID);
                // lấy danh sách quyền
                if (userId != null)
                {
                    var menuUserData = menuServices.GetUserMenusByUserId(userId.Value);
                    foreach (var item in menuData)
                    {
                        var inttt = menuUserData.IndexOf(item.MenuId);
                        if (menuUserData.IndexOf(item.MenuId) != -1) item.Selected = true;
                        else item.Selected = false;
                    }
                }
                else if (roleId != null)
                {
                    var menuUserData = menuServices.GetRoleMenusByRoleId(roleId.Value);
                    foreach (var item in menuData)
                    {
                        if (menuUserData.IndexOf(item.MenuId) != -1) item.Selected = true;
                        else item.Selected = false;
                    }
                }

                var uiMenuTreeHelper = new UIMenuTreeHelper(menuData);
                var uiMenuDataJson = uiMenuTreeHelper.BuildMenuToJsonStr(uiMenuTreeHelper.RootId);
                return Json((new JavaScriptSerializer()).Deserialize(uiMenuDataJson, typeof(object)), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReportList(int? RequireId = null)
        {
            SetConnectionDB();
            Services.ReportBi.ReportBIServices service = new Services.ReportBi.ReportBIServices(DBConnection);
            Services.BCGB.BCGBServices serviceBCGB = new Services.BCGB.BCGBServices(DBConnection);
            List<EntityReportBIModel> list = service.GetList();
            RoleServices roleServices = new RoleServices(DBConnection);
            List<EntityReportBIModel> listRole = new List<EntityReportBIModel>();
            if (RequireId != null && RequireId > 0)
                listRole = serviceBCGB.GetList("", RequireId);

            AreaServices areaServices = new AreaServices(DBConnection);
            List<EntityAreaModel> listArea = new List<EntityAreaModel>();
            List<MenuFancyTreeItem> listFan = new List<MenuFancyTreeItem>();
            listArea = areaServices.GetList();
            if (listArea != null && listArea.Count > 0)
            {
                foreach (EntityAreaModel area in listArea)
                {
                    MenuFancyTreeItem objFolder = new MenuFancyTreeItem()
                    {
                        title = area.AreaName,
                        key = 0,
                        folder = true,
                        hideCheckbox = true
                        //parentId= area.ParentId
                    };

                    var lstResultReport = list.FindAll(o => o.AreaId == area.Id);
                    if (lstResultReport != null && lstResultReport.Count > 0)
                    {
                        List<MenuFancyTreeItem> listchildren = new List<MenuFancyTreeItem>();
                        foreach (EntityReportBIModel reporBI in lstResultReport)
                        {
                            bool blReportSelect = false;

                            string depCode = reporBI.DepartmentCode;
                            string preTitle = "";
                            if (reporBI.DataAuto)
                            {
                                preTitle = "[" + depCode + new string(' ', 10 - depCode.Length) + "] [TĐ]";
                            }
                            else
                            {
                                preTitle = "[" + depCode + new string(' ', 10 - depCode.Length) + "] [KTĐ]";
                            }

                            if (listRole != null && listRole.Count > 0)
                            {
                                var blResult = listRole.FindAll(o => o.Id == reporBI.Id);
                                if (blResult != null && blResult.Count() > 0)
                                    blReportSelect = true;
                            }
                            MenuFancyTreeItem obj = new MenuFancyTreeItem()
                            {

                                title = preTitle + reporBI.ReportName,
                                key = reporBI.Id,
                                selected = blReportSelect,
                                parentId = area.Id,
                                hideCheckbox = false
                            };
                            listchildren.Add(obj);
                        }
                        objFolder.children = listchildren;
                    }
                    listFan.Add(objFolder);

                }
            }
            return Json(listFan, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List2(int? DeptID = null, int? userId = null, int? roleId = null)
        {
            this.SetConnectionDB();
            ReportRequireService reportService = new ReportRequireService(this.DBConnection);
            ViewData["reportList"] = reportService.GetList2();

            var _deptID = (int)Session["DepartIdUserLogin"];
            //DeptID = (int)Session["DepartIdUserLogin"];
            if (DeptID == null)
            {
                DeptID = _deptID;
            }

            //CheckAdminPermission();
            SetCommonData();
            MenuServices menuServices = new MenuServices(DBConnection);

            if (!Request.IsAjaxRequest())
            {
                if (null == Session[SESSION_NAME_USERID])
                {
                    return RedirectToAction("Login", "Home");
                }

                ViewData["pagename"] = "menu_list";
                ViewData["action_block"] = "ReportRequire/block_reportrequire_list2";

                GetLanguage();
                ViewData["VIEWDATA_LANGUAGE"] = LANGUAGE_OBJECT;

                BlockMenuListLangModel blockLang = new BlockMenuListLangModel();
                BlockModel blockModel = new BlockModel("block_reportrequire_list", LANGUAGE_OBJECT, blockLang);
                Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);

                ViewData["departments"] = departmentServices.GetList();
                ViewData["listdepartmentsadmin"] = departmentServices.GetListAdminLogin((string)Session["CodeIsAdmin"]);



                ViewData["BlockData"] = blockModel;

                SetConnectionDB();

                var menuData = menuServices.GetMenusByDepId((int)Session[SESSION_NAME_USERID], DeptID);
                ViewData["CurrentOrgId"] = DeptID;
                ViewData["MenuData"] = menuData;

                return View("~/" + THEME_FOLDER + "/" + THEME_ACTIVE + "/index.cshtml");
            }
            else
            {
                var menuData = menuServices.GetMenusByDepId((int)Session[SESSION_NAME_USERID], DeptID);
                // lấy danh sách quyền
                if (userId != null)
                {
                    var menuUserData = menuServices.GetUserMenusByUserId(userId.Value);
                    foreach (var item in menuData)
                    {
                        var inttt = menuUserData.IndexOf(item.MenuId);
                        if (menuUserData.IndexOf(item.MenuId) != -1) item.Selected = true;
                        else item.Selected = false;
                    }
                }
                else if (roleId != null)
                {
                    var menuUserData = menuServices.GetRoleMenusByRoleId(roleId.Value);
                    foreach (var item in menuData)
                    {
                        if (menuUserData.IndexOf(item.MenuId) != -1) item.Selected = true;
                        else item.Selected = false;
                    }
                }

                var uiMenuTreeHelper = new UIMenuTreeHelper(menuData);
                var uiMenuDataJson = uiMenuTreeHelper.BuildMenuToJsonStr(uiMenuTreeHelper.RootId);
                return Json((new JavaScriptSerializer()).Deserialize(uiMenuDataJson, typeof(object)), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [CheckUserMenus]
        public ActionResult Delete()
        {
            //if ((bool)Session["IsAdmin"] == false)
            //{
            //    return RedirectToAction("Logout", "Home");
            //}
            string menuid = this.HttpContext.Request["menuid"];
            try
            {
                this.SetConnectionDB();


                int output = 0;
                MenuServices menuServices = new MenuServices(this.DBConnection);

                output = menuServices.Delete(Int32.Parse(menuid));
                if (menuServices.ERROR != null) BI_Project.Helpers.FileHelper.SaveFile(menuServices.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");
            }
            catch (Exception ex)
            {

            }


            return RedirectToAction("List");
        }

        private List<EntityCycleModel> setListCycle()
        {
            List<EntityCycleModel> listkq = new List<EntityCycleModel>();
            EntityCycleModel cycle;

            cycle = new EntityCycleModel();
            listkq.Add(cycle);

            cycle = new EntityCycleModel();
            cycle.Id = "WEEKLY";
            cycle.Name = "TUẦN";
            listkq.Add(cycle);

            cycle = new EntityCycleModel();
            cycle.Id = "MONTH";
            cycle.Name = "THÁNG";
            listkq.Add(cycle);

            cycle = new EntityCycleModel();
            cycle.Id = "QUARTER";
            cycle.Name = "QUÝ";
            listkq.Add(cycle);

            cycle = new EntityCycleModel();
            cycle.Id = "YEAR";
            cycle.Name = "NĂM";
            listkq.Add(cycle);

            return listkq;
        }
    }
}