using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BI_Project.Services.User;
using BI_Project.Models;
using BI_Project.Models.EntityModels;
using BI_Project.Models.UI;
using BI_Project.Helpers;
using BI_Project.Services.Departments;
using BI_SUN.Services.SetDefaultPage;
using bicen.Models.EntityModels;

namespace BI_Project.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (Session[this.SESSION_NAME_USER_NAME] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {


                this.SetCommonData();


                this.GetLanguage();
                ViewData["VIEWDATA_LANGUAGE"] = this.LANGUAGE_OBJECT;

                BI_Project.Models.BusinessModels.BlockWelcomeLangModel blockWelcomeLang = new Models.BusinessModels.BlockWelcomeLangModel();
                BI_Project.Models.UI.BlockModel blockModel = new BlockModel("block_welcome", this.LANGUAGE_OBJECT, blockWelcomeLang);
                ViewData["BlockData"] = blockModel;
                return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
            }
        }


        


        public ActionResult Logout()
        {
            //var session = Session[this.SESSION_NAME_USER_NAME].ToString();
            EntityUserTimeModel logger = (EntityUserTimeModel)Session["Logger"];
            if (logger != null)
            {
                this.SetConnectionDB();
                UserServices userServices = new UserServices(this.DBConnection);
                var insertlog = userServices.UpdateLogUserLogout(logger);
            }
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Clear();            
            Logging.WriteToLog("[Info] " + Session["UserName"] + " logged out.", LogType.Access);
            return RedirectToAction("Login");
        }


        [HttpGet]
        public ActionResult Login()
        {
            this.GetLanguage();
            ViewData["VIEWDATA_LANGUAGE"] = this.LANGUAGE_OBJECT;
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/login.cshtml");
        }
        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {            
            Char charRange = '/';
            //STEP1: GOI HAM LOGIN TOI CSDL *****************************************************
            this.SetConnectionDB();
            BI_Project.Services.User.UserServices userServices = new UserServices(this.DBConnection);
            BI_Project.Services.Departments.DepartmentServices departmentServices = new DepartmentServices(this.DBConnection);
            Session["HDSD"] = loginModel.Department;
            EntityUserModel entityUser = userServices.CheckLogin(loginModel);
            EntityUserTimeModel logger = new EntityUserTimeModel();
            EntityDepartmentModel entityDepartment = departmentServices.GetEntityById(entityUser.DeptId);
            SetDefaultPageService setDefault = new SetDefaultPageService(DBConnection);
            if (entityUser.UserName != null)
            {
                logger.UserId = entityUser.UserId;
                logger.PKID = Guid.NewGuid();
                logger.TimeLogin = DateTime.Now;
                logger.Date = DateTime.Now.Date;
                logger.UserName = entityUser.UserName;
                Session["Logger"] = logger;
                var insertlog = userServices.UpdateLogUserLogin(logger);
                Session["UserName"] = entityUser.UserName;
                Session["UserId"] = entityUser.UserId;
                Session["FullName"] = entityUser.FullName;
                Session[this.SESSION_NAME_USER_NAME] = entityUser.UserName;
                Session[this.SESSION_NAME_USERID] = entityUser.UserId;
                Session["DepartIdUserLogin"] = entityUser.DeptId;
                Session["IsAdmin"] = entityUser.IsAdmin;
                Session["IsSuperAdmin"] = entityUser.IsSuperAdmin;
                Session["CodeIsAdmin"] = entityDepartment.Code;
                Session["Filter01IsAdmin"] = entityDepartment.Filter01;
                Session["ListAllowedMenuPath"] = entityUser.LstMenus.Select(x => x.Path).ToList();
                List<EntityUserMenuModel> entityUserMenuModel = setDefault.GetListDefaultPage(entityUser.UserId);
                foreach (EntityUserMenuModel item in entityUserMenuModel)
                {
                    if (item.IsDefaultPage == true)
                    {
                        var _path = item.Path;
                        string _controller = _path.Split(charRange)[0];
                        string _action = _path.Split(charRange)[1];
                        int _menuId = item.MenuId;

                        return RedirectToAction(_action + "/" + _menuId, _controller);

                    }
                }
                return RedirectToAction("Index");

            }
            if (userServices.ERROR != null)
            {
                Session["msgcode"] = MessageType.ServerError;
                FileHelper.SaveFile(userServices.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");
            }
            else Session["msgcode"] = MessageType.BusinessError;

            this.GetLanguage();
            ViewData["VIEWDATA_LANGUAGE"] = this.LANGUAGE_OBJECT;
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/login.cshtml", loginModel);

            //STEP2: NEU DANG NHAP KHONG THANH CONG

        }



    }
}