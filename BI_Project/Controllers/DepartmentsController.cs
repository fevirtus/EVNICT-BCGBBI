using BI_Project.Helpers;
using BI_Project.Helpers.Utility;
using BI_Project.Models.EntityModels;
using BI_Project.Models.UI;
using BI_Project.Services.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BI_Project.Controllers
{
    public class DepartmentsController : BaseController
    {
        public ActionResult List()
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
            ViewData["pagename"] = "department_list";
            ViewData["action_block"] = "Departments/block_department_list";


            this.GetLanguage();


            this.SetConnectionDB();
            DepartmentServices services = new DepartmentServices(this.DBConnection);

            BlockDepartmentListLangModel blockLang = new BlockDepartmentListLangModel();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_department_list", this.LANGUAGE_OBJECT, blockLang);
            //blockModel.DataModel = ViewData["block_menu_left_data"];


            if ((bool)Session["IsSuperAdmin"])
            {
                ViewData["departments"] = services.GetList();

            }
            else if ((bool)Session["IsAdmin"])
            {
                ViewData["listdepartmentsadmin"] = services.GetListAdminLogin((string)Session["CodeIsAdmin"]);
            }
            blockModel.DataModel = services.GetList();
            blockModel.Hidden = 0;

            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            this.SaveAccessLog("list");
            ViewData["BlockData"] = blockModel;
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        public ActionResult Create()
        {
            this.SaveAccessLog("create");
            if (null == Session[this.SESSION_NAME_USERID])
            {
                return RedirectToAction("Login", "Home");
            }
            if ((bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }
            this.SetCommonData();


            ViewData["pagename"] = "department_create";
            ViewData["action_block"] = "Departments/block_department_create";
            ViewData["data-form"] = TempData["data"];

            string departId = (Request.QueryString["departid"] == null ? "0" : Request.QueryString["departid"].ToString());
            this.SetConnectionDB();
            DepartmentServices services = new DepartmentServices(this.DBConnection);

            EntityDepartmentModel model = new EntityDepartmentModel();

            if (ViewData["data-form"] != null)
            {
                model = (EntityDepartmentModel)ViewData["data-form"];
            }
            else
            {
                model = services.GetEntityById(Int32.Parse(departId));
            }

            //EntityDepartmentModel modelResponse = services.GetEntityById(Int32.Parse(departId));
            this.GetLanguage();
            if (model.DepartId > 0) ViewData["pagename"] = "department_edit";

            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            BlockDepartmentCreateLangModel blockLang = new BlockDepartmentCreateLangModel();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_department_create", this.LANGUAGE_OBJECT, blockLang);
            blockModel.DataModel = model;
            ViewData["BlockData"] = blockModel;


            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpPost]
        [CheckUserMenus]
        public ActionResult Create(EntityDepartmentModel model)
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

            this.GetLanguage();
            int output = 0;
            if (model.Filter01 == null)
            {
                Session["msg_code"] = -1;
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_department_create.error_code");
                TempData["data"] = model;
                return RedirectToAction("Create");
            }
            if(model.Filter01.Length == 1)
            {
                model.Code = model.Filter01.Substring(0, 1);
            }
            else
            {
                model.Code = model.Filter01.Substring(0, 2);
            }
            if (model.Filter01.Length == 4)
            {
                model.Filter02 = model.Filter01;
            }
            else
            {
                model.Filter02 = null;
            }
            DepartmentServices departServices = new DepartmentServices(this.DBConnection);

            output = departServices.CreateDepart(model);

            /****************************************RESPONSE FAILE OR SUCCESS******************************************/

            //this.GetLanguage();
            BlockDepartmentCreateLangModel blockLang = new BlockDepartmentCreateLangModel();
            blockLang.BlockName = "block_department_create";
            blockLang.SetLanguage(this.LANGUAGE_OBJECT);
            Session["msg_text"] = blockLang.GetMessage(output);
            Session["msg_code"] = output;

            if (departServices.ERROR != null) BI_Project.Helpers.FileHelper.SaveFile(departServices.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");

            if (model.DepartId > 0 && output > 0)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_department_create.success_edit", this.LANGUAGE_OBJECT);
            }
            if (output == 0)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_department_create.error_business_1", this.LANGUAGE_OBJECT);
                //return RedirectToAction("Create?roleid=" + model.RoleId);
            }
            if (output > 0)
            {
                return RedirectToAction("List");
            }


            TempData["data"] = model;
            return RedirectToAction("Create");

        }

        [HttpGet]
        [CheckUserMenus]
        public ActionResult Delete()
        {
            this.SaveAccessLog("delete");
            if ((bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }
            string id = this.HttpContext.Request["departid"];
            int output = 0;
            try
            {
                this.SetConnectionDB();



                DepartmentServices services = new DepartmentServices(this.DBConnection);
                if (services.ERROR != null) throw new Exception(services.ERROR);
                output = services.Delete(Int32.Parse(id));
            }
            catch (Exception ex)
            {
                FileHelper.SaveFile(new { data = id, ERROR = ex }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
                output = -2;
            }
            //*************************************XU LY VAN DE THONG BAO THANH CONG HAY THAT BAI********************
            this.GetLanguage();
            BlockDepartmentCreateLangModel blockLang = new BlockDepartmentCreateLangModel();
            blockLang.BlockName = "block_department_list";
            blockLang.SetLanguage(this.LANGUAGE_OBJECT);

            Session["msg_code"] = output;
            if (output > 1)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_department_list.success_delete", this.LANGUAGE_OBJECT);
            }

            return RedirectToAction("List");
        }
    }
}