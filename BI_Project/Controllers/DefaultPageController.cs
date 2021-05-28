using BI_Project.Controllers;
using BI_Project.Models.EntityModels;
using BI_SUN.Services.SetDefaultPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bicen.Controllers
{
    public class DefaultPageController : BaseController
    {
        // GET: DefaultPage
        //[CheckUserMenus]
        public ActionResult SetDefaultPage(int id)
        {
            ViewData["pagename"] = "SetDefaultPage";
            ViewData["action_block"] = "DefaultPage/SetDefaultPage";
            SetCommonData();
            GetLanguage();
            SetConnectionDB();
            SetDefaultPageService _setDefaultPage = new SetDefaultPageService(DBConnection);

            ViewData["usermenu"] = _setDefaultPage.GetListDefaultPage(id);
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpPost]
        //[Route("/SetDefaultPage/{id?}")]
        public ActionResult SetDefaultPage(int id, int menuId)
        {
            SetConnectionDB();
            SetDefaultPageService _setDefaultPage = new SetDefaultPageService(DBConnection);
            ViewData["usermenu"] = _setDefaultPage.GetListDefaultPage(id);
            _setDefaultPage.UpdatePageDefault(id, menuId);
            return Redirect("SetDefaultPage?id=" + id);
        }

        public ActionResult UC()
        {
            ViewData["pagename"] = "SetDefaultPage";
            ViewData["action_block"] = "DefaultPage/UC";
            SetCommonData();
            GetLanguage();
            SetConnectionDB();
            SetDefaultPageService _setDefaultPage = new SetDefaultPageService(DBConnection);
            //BI_Core.Tableau.TableauModel = new tableauModel = (BI_Core.Tableau.TableauModel)
            //ViewData["usermenu"] = _setDefaultPage.GetListDefaultPage(id);
            //return View();
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }


        //dagn ky
        [HttpPost]
        public ActionResult UpdateQuanLyMau(List<EntityQuanLyMau> model)
        {
            SetConnectionDB();            
            SetDefaultPageService rs = new SetDefaultPageService(DBConnection);
            rs.UpdateQuanLyMau(model);
            string madv = model[0].DEPARTMENTID;
            int nam = model[0].YEAR_LRS;

            return QuanLyMau(madv, nam);
        }


        public ActionResult QuanLyMau(string MaDV, int iNam = 0)
        {
            try
            {
                ViewData["pagename"] = "Quản lý mẫu nghiên cứu phụ tải";
                ViewData["action_block"] = "DefaultPage/QuanLyMau";
                GetLanguage();
                SetCommonData();
                SetConnectionDB();
                
                //lấy ds cho tìm kiếm
                SetDefaultPageService rs = new SetDefaultPageService(DBConnection);
                ViewData["dsDonVi"] = rs.GetListDepartments(Session["CodeIsAdmin"].ToString(), 1);
                //ViewData["usermenu"] = rs.GetListDefaultPage(id);
                //lấy ds trạm
                List<EntityQuanLyMau> model = new List<EntityQuanLyMau>();
                if (MaDV != null)
                {
                    model = rs.GetListQuanLyMau(MaDV, iNam);
                }
                EntityTimKiemModel timkiem = new EntityTimKiemModel();
                timkiem.MA_DVIQLY = MaDV;
                timkiem.NAM = iNam;
                ViewData["timkiem"] = timkiem;
                //ViewData["BlockData"] = model;
                return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml", model);
            }
            catch (Exception ex)
            {
                return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
            }
        }



    }
}
