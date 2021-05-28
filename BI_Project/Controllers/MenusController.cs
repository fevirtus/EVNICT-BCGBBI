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

namespace BI_Project.Controllers
{
    public class MenusController : BaseController
    {

        //private string x = SESSION_NAME_USERID;

        public ActionResult Create()
        {
            if (null == Session[this.SESSION_NAME_USERID])
            {
                return RedirectToAction("Login", "Home");
            }
            if ((bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }
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
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_create_menu", this.LANGUAGE_OBJECT, blockLang);
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

            if ((bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }
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

        //public ActionResult List()
        //{
        //    if (null == Session[this.SESSION_NAME_USERID])
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }
        //    if ((bool)Session["IsAdmin"] == false)
        //    {
        //        return RedirectToAction("Logout", "Home");
        //    }
        //    this.SetCommonData();
        //    ViewData["pagename"] = "menu_list";
        //    ViewData["action_block"] = "Menus/block_menu_list";


        //    this.GetLanguage();
        //    ViewData["VIEWDATA_LANGUAGE"] = this.LANGUAGE_OBJECT;


        //    BlockMenuListLangModel blockLang = new BlockMenuListLangModel();
        //    BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_menu_list", this.LANGUAGE_OBJECT, blockLang);
        //    //blockModel.DataModel = ViewData["block_menu_left_data"];
        //    blockModel.Hidden = 0;
        //    ViewData["BlockData"] = blockModel;
        //    return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        //}

        //lIST


        public ActionResult List(int? DeptID = null, int? userId = null, int? roleId = null)
        {
            if (Session["IsAdmin"] != null && (bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }
            var _deptID = -1;
            if (Session["DepartIdUserLogin"] != null)
                _deptID = (int)Session["DepartIdUserLogin"];
            //DeptID = (int)Session["DepartIdUserLogin"];
            if (DeptID >= 0)
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
                if ((bool)Session["IsAdmin"] == false)
                {
                    return RedirectToAction("Logout", "Home");
                }

                ViewData["pagename"] = "menu_list";
                ViewData["action_block"] = "Menus/block_menu_list";

                GetLanguage();
                ViewData["VIEWDATA_LANGUAGE"] = LANGUAGE_OBJECT;

                BlockMenuListLangModel blockLang = new BlockMenuListLangModel();
                BlockModel blockModel = new BlockModel("block_menu_list", LANGUAGE_OBJECT, blockLang);
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
            if ((bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }
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
    }
}