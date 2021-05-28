using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BI_Project.Services.User;
using BI_Project.Models.EntityModels;
using BI_Project.Models.UI;
using BI_Project.Helpers;
using BI_Project.Helpers.Utility;
using System.Text.RegularExpressions;
using BI_Project.Services.Menus;

namespace BI_Project.Controllers
{
    public class UsersController : BaseController
    {
        
        public ActionResult List()
        {
            
            if (Session["IsAdmin"] == null || (bool)Session["IsAdmin"]==false)
            {
                return RedirectToAction("Logout", "Home");
            }
            this.SetCommonData();
            ViewData["pagename"] = "user_list";
            ViewData["action_block"] = "Users/block_user_list";


            this.GetLanguage();
            

            this.SetConnectionDB();
            UserServices services = new UserServices(this.DBConnection);

            BlockLangUserListModel blockLang = new BlockLangUserListModel();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_user_list", this.LANGUAGE_OBJECT, blockLang);

            //Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);

            //ViewData["departments"] = departmentServices.GetList();

            if ((bool)Session["IsSuperAdmin"])
            {
                blockModel.DataModel = services.GetList();

            }
            else if ((bool)Session["IsAdmin"])
            {
                blockModel.DataModel = services.GetList((int)Session["DepartIdUserLogin"]);
            }

            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId()+".txt");

            Logging.WriteToLog(this.GetType().ToString() + "-List()", LogType.Access);
            ViewData["BlockData"] = blockModel;
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        public ActionResult Create()
        {
            
            if (Session["IsAdmin"] == null || (bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }
            this.SetCommonData();


            ViewData["pagename"] = "user_create";
            ViewData["action_block"] = "Users/block_user_create";
            ViewData["data_form"] = TempData["data"];

            string id = (Request.QueryString["id"] == null ? "0" : Request.QueryString["id"].ToString());
            this.SetConnectionDB();
            UserServices services = new UserServices(this.DBConnection);
            BlockDataUserCreateModel model = new BlockDataUserCreateModel();
            if (TempData["data"] != null)
            {
                model = (BlockDataUserCreateModel)ViewData["data_form"];
            }
            else
            {
                model = services.GetEntityById(Int32.Parse(id));
            }
            ViewData["data_form"] = model;
            Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);

            ViewData["departments"] = departmentServices.GetList();
            ViewData["listdepartmentsadmin"] = departmentServices.GetListAdminLogin((string)Session["CodeIsAdmin"]);
            ViewData["CurrentUser"] = id;
            ViewData["currentOrgId"] = model.DeptId;
            this.GetLanguage();
            if (model.UserId > 0) ViewData["pagename"] = "user_edit";

            BI_Project.Services.Roles.RoleServices roleServices = new Services.Roles.RoleServices(this.DBConnection);

            model.ListAllRoles = roleServices.GetList();

            MenuServices menuServices = new MenuServices(DBConnection);
            var menuData = menuServices.GetMenusByDepId((int)Session[SESSION_NAME_USERID]);
            //ViewData["CurrentDeptId"] = deptId;
            ViewData["MenuData"] = menuData;

            BlockLangUserCreateModel blockLang = new BlockLangUserCreateModel();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_user_create", this.LANGUAGE_OBJECT, blockLang);
            blockModel.DataModel = model;
            ViewData["BlockData"] = blockModel;

            if(roleServices.ERROR != null) FileHelper.SaveFile(roleServices.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpGet]
        [CheckUserMenus]
        public ActionResult Delete()
        {
            if ((bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }
            string id = this.HttpContext.Request["id"];
            Logging.WriteToLog(this.GetType().ToString() + "-delete(), id="+id, LogType.Access);
            int output = 0;
            try
            {
                this.SetConnectionDB();
                UserServices services = new UserServices(this.DBConnection);

                output = services.Delete(Int32.Parse(id));
                if (services.ERROR != null) throw new Exception(services.ERROR);
            }
            catch (Exception ex)
            {
                this.ERRORS = ex.ToString();
                FileHelper.SaveFile(ex, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
                output = -2;
            }
            ////*************************************XU LY VAN DE THONG BAO THANH CONG HAY THAT BAI********************
            this.GetLanguage();
            if (output > 0)
            {
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_user_list.success_delete");
            }
            else
            {
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "commons.messages.ServerError");
            }
            Session["msg_code"] = output;
            return RedirectToAction("List");
        }

        [HttpPost]
        [CheckUserMenus]
        public ActionResult Create(BlockDataUserCreateModel model)
        {
            string p = "";
            string s = "";

            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);
            if (null == Session[this.SESSION_NAME_USERID])
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["IsAdmin"]  == null || (bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewData["data_form"] = TempData["data"];
            // get language
          
            this.GetLanguage();

            // validate du lieu
            if (!string.IsNullOrEmpty(model.Email))
            {
                string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                         @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex re = new Regex(emailRegex);
                if (!re.IsMatch(model.Email))
                {
                    ModelState.AddModelError("Email", "Email is not valid");
                    Session["msg_code"] = -1;
                    Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_user_create.error_email");
                    TempData["data"] = model;
                    return RedirectToAction("Create");

                }

            }
            if(model.Password != model.ConfirmPassword)
            {
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_user_create.error_password");
                TempData["data"] = model;
                return RedirectToAction("Create");
            }
            if (!string.IsNullOrEmpty(model.Password))
            {
                string passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^])[A-Za-z\d@$!%*?&#^]{8,}$";
                Regex re = new Regex(passwordRegex);
                if (!re.IsMatch(model.Password))
                {
                    ModelState.AddModelError("User", "User not regex");
                    Session["msg_code"] = -1;
                    Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_user_create.error_pass");
                    TempData["data"] = model;
                    return RedirectToAction("ChangePassword");

                }

            }
            if (!string.IsNullOrEmpty(model.Phone))
            {
                string phoneRegex = @"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$";
                Regex re = new Regex(phoneRegex);
                if (!re.IsMatch(model.Phone))
                {
                    ModelState.AddModelError("Phone", "Phone is not valid");
                    Session["msg_code"] = -1;
                    Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_user_create.error_phone");
                    TempData["data"] = model;
                    return RedirectToAction("Create");

                }

            }

            //if (model.LstSelectedRole.Count() == 0)
            //{
            //    ModelState.AddModelError("ListRoles", "Role is not valid");
            //    Session["msg_code"] = -1;
            //    Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_user_create.error_Role");
            //    TempData["data"] = model;
            //    return RedirectToAction("Create");
            //}



            //**************** DATABASE PROCESS*******************************************************
            this.SetConnectionDB();

            UserServices services = new UserServices(this.DBConnection);
            // check tài khoản đã tồn tại trong hệ thống hay chưa

            var checkUser = services.GetList();
            var UserEdit = checkUser.FirstOrDefault(x => x.UserId == model.UserId);
            EntityUserModel Usercheck = new EntityUserModel();
            if(model.IsSuperAdmin == true)
            {
                var User = checkUser.FirstOrDefault(x => x.UserName == model.UserName && x.UserId != model.UserId);
                Usercheck = User;
            }
            else
            {
                var CodeUser = services.GetCodeByDeptId(model.DeptId);
                var User = checkUser.FirstOrDefault(x => x.UserName == model.UserName && x.UserId != model.UserId && (x.DeptId == model.DeptId || x.Code == CodeUser));
                Usercheck = User;
            }
            if (Usercheck != null)
            {
                Session["msg_code"] = -1;
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_user_create.error_User");
                TempData["data"] = model;
                return RedirectToAction("Create");
            }
            int result = 0;
            if (UserEdit != null)
            {
                 result = services.Create(model, UserEdit.Password, UserEdit.Salt);
            }
            else
            {
                 result = services.Create(model, p, s);
            }


            if(services.ERROR!=null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            //**************** GET LANGUAGE AND MESSAGE******************************************************************
            //this.GetLanguage();

            if (result > 0)
            {
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_user_create.success");
            }
            else
            {
               Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_user_create.error_business_1");
            }
            
            Session["msg_code"] = result;
            if (model.UserId > 0 && result > 1)
            {
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT,"messages.block_user_create.success_edit");
            }
            //***********************INSERT OR EDIT SUCCESSFULLY * *************************************************
            if (result > 0)
            {
                return RedirectToAction("List");
            }
            TempData["data"] = model;
            //ViewBag.User = model;
            //this.SetCommonData();
            //BlockLangUserCreateModel blockLang = new BlockLangUserCreateModel();
            //BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_user_create", this.LANGUAGE_OBJECT, blockLang);
            //blockModel.DataModel = model;
            //ViewData["BlockData"] = blockModel;
            //ViewData["action_block"] = "Users/block_user_create";
            return RedirectToAction("Create");
            //return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");


            //*************************************************************************
            //BlockLangRoleCreateModel blockLang = new BlockLangRoleCreateModel();
            //BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_role_create", this.LANGUAGE_OBJECT, blockLang);
            //blockModel.DataModel = model;
            //ViewData["BlockData"] = blockModel;
            //return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");

            //return RedirectToAction("Create?roleid="+model.RoleId);


        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            Logging.WriteToLog(this.GetType().ToString() + "-changepww()", LogType.Access);
            if (Session[this.SESSION_NAME_USERID] == null )
            {
                return RedirectToAction("Logout", "Home");
            }
            
            this.SetCommonData();
            ViewData["pagename"] = "change_password";
            ViewData["action_block"] = "Users/block_user_changepw";
            this.GetLanguage();
            BlockLangUserChangepwModel blockLang = new BlockLangUserChangepwModel();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_user_changepw", this.LANGUAGE_OBJECT, blockLang);
            //blockModel.DataModel = ViewData["block_menu_left_data"];
            
            ViewData["BlockData"] = blockModel;
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpPost]
        public ActionResult ChangePassword(BlockDataUserChangepwModel model)
        {
            Logging.WriteToLog(this.GetType().ToString() + "-changepw-submit()", LogType.Access);
            if (Session[this.SESSION_NAME_USERID] == null)
            {
                return RedirectToAction("Logout", "Home");
            }
            this.SetCommonData();
            this.GetLanguage();
            BlockLangUserChangepwModel blockLang = new BlockLangUserChangepwModel();
            
            if (model.ConfirmPassword != model.Password)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_user_changepw.not_equalto", this.LANGUAGE_OBJECT);
                Session["msg_code"] = 0;
                return RedirectToAction("ChangePassword");
            }
            if (!string.IsNullOrEmpty(model.Password))
            {
                string passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^])[A-Za-z\d@$!%*?&#^]{8,}$";
                Regex re = new Regex(passwordRegex);
                if (!re.IsMatch(model.Password))
                {
                    ModelState.AddModelError("User", "User not regex");
                    Session["msg_code"] = -1;
                    Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_user_create.error_pass");
                    TempData["data"] = model;
                    return RedirectToAction("ChangePassword");

                }

            }
            //Session["msg_code"] = output;
            this.SetConnectionDB();

            UserServices services = new UserServices(this.DBConnection);
            model.UserId = (int)Session[this.SESSION_NAME_USERID];
            int output = services.ChangePw(model);
            this.ERRORS = services.ERROR;
            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            
            if (output == model.UserId)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_user_changepw.success", this.LANGUAGE_OBJECT);
                Session["msg_code"] = 1;
            }

            else
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_user_changepw.not_exist_user", this.LANGUAGE_OBJECT);
                Session["msg_code"] = 0;

            }

            return RedirectToAction("ChangePassword");
        }

    }
}