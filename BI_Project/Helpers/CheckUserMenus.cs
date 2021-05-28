using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

using BI_Project.Models.EntityModels;
using BI_Project.Services.User;
using BI_Project.Services;
namespace BI_Project.Helpers
{
    public class CheckUserMenus: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string connectString = WebConfigurationManager.AppSettings["CONNECT_STRING"];
            DBConnection dBConnection = new DBConnection(connectString);
            var IDSession = filterContext.HttpContext.Session["session_userid"];
            bool isAdmin = false;
            int id = (IDSession != null) ? (int)IDSession : 0;
            UserServices userServices = new UserServices(dBConnection);

            try
            {
                isAdmin = (bool)filterContext.HttpContext.Session["isAdmin"];
            }
            catch(Exception)
            {
                isAdmin = false;
            }
            try
            {
                EntityUserModel currentUser = userServices.FindById(id);
                List<EntityMenuModel> userMenu = userServices.GetAllowedMenuAndRoles(currentUser.UserId);

                string _path = filterContext.HttpContext.Request.RawUrl;

                bool hasPermission = false;
                if (isAdmin == true) hasPermission = true;
                else
                {
                    foreach (EntityMenuModel menu in userMenu)
                    {
                        if (menu.Path == _path)

                            hasPermission = true;

                    }

                }
                if (!hasPermission) throw new Exception();
            }
            catch
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "Home", action = "Logout" })
                );
            }



        }


    }
}