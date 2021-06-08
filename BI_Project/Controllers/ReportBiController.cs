using BI_Project.Helpers;
using BI_Project.Helpers.Utility;
using BI_Project.Models.EntityModels;
using BI_Project.Models.UI;
using BI_Project.Services.Departments;
using BI_Project.Services.ReportBi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Script.Serialization;


namespace BI_Project.Controllers
{
    public class ReportBiController : BaseController
    {
        public ActionResult List()
        {
            /*if (null == Session[this.SESSION_NAME_USERID])
            {
                return RedirectToAction("Login", "Home");
            }
            if ((bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }*/

            this.SetCommonData();
            this.GetLanguage();
            ViewData["pagename"] = "report_bi_list";
            ViewData["action_block"] = "ReportBI/block_report_bi_list";

            this.SetConnectionDB();
            ReportBIServices services = new ReportBIServices(this.DBConnection);
         
            BlockDepartmentListLangModel blockLang = new BlockDepartmentListLangModel();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_report_bi_list", this.LANGUAGE_OBJECT, blockLang);

            AreaServices areaServices = new AreaServices(this.DBConnection);

            List<EntityAreaModel> listArea = areaServices.GetList();

            List<EntityReportBIModel> listReport = services.GetList();
            listReport = setAreaNameforListReport(listReport, listArea);


            ViewData["reportlist"] = listReport;

            blockModel.DataModel = listReport;
            blockModel.Hidden = 0;

            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            this.SaveAccessLog("list");
            ViewData["BlockData"] = blockModel;
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        public ActionResult Create()
        {
            this.SaveAccessLog("create");
           /* if (null == Session[this.SESSION_NAME_USERID])
            {
                return RedirectToAction("Login", "Home");
            }
            if ((bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }*/
            this.SetCommonData();

       

            ViewData["pagename"] = "report_bi_create";
            ViewData["action_block"] = "ReportBI/block_report_bi_create";
            ViewData["data-form"] = TempData["data"];

            string id = (Request.QueryString["Id"] == null ? "0" : Request.QueryString["Id"].ToString());
            this.SetConnectionDB();
            ReportBIServices services = new ReportBIServices(this.DBConnection);

            EntityReportBIModel model = new EntityReportBIModel();

            Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);

            List<EntityDepartmentModel> listPb= departmentServices.GetList();

            listPb.Insert(0,new EntityDepartmentModel());

            ViewData["departments"] = listPb;

            Services.ReportBi.AreaServices areServices = new Services.ReportBi.AreaServices(this.DBConnection);

            List<EntityAreaModel> listArea= areServices.GetList();

            listArea.Insert(0, new EntityAreaModel());

            ViewData["listarea"] = listArea;


            ViewData["listcycle"] = setListCycle();


            if (ViewData["data-form"] != null)
            {
                model = (EntityReportBIModel)ViewData["data-form"];
            }
            else
            {
                model = services.GetEntityById(Int32.Parse(id));
            }

            //EntityDepartmentModel modelResponse = services.GetEntityById(Int32.Parse(departId));
            this.GetLanguage();
            if (model.Id > 0) ViewData["pagename"] = "report_bi_edit";

            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            BlockDepartmentCreateLangModel blockLang = new BlockDepartmentCreateLangModel();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_report_bi_create", this.LANGUAGE_OBJECT, blockLang);
            blockModel.DataModel = model;
            ViewData["BlockData"] = blockModel;


            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpPost]        
        public ActionResult Create(EntityReportBIModel model)
        {
           /* if (null == Session[this.SESSION_NAME_USERID])
            {
                return RedirectToAction("Login", "Home");
            }

            if ((bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }*/
            this.SetConnectionDB();

            this.GetLanguage();
            int output = 0;
            if (model.ReportName == null)
            {
                Session["msg_code"] = -1;
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_report_bi_create.error_code");
                TempData["data"] = model;
                return RedirectToAction("Create");
            }
            if(model.ReportCode == null)
            {
                model.ReportCode = model.ReportName.Substring(0, 3);
            }
           
           
            ReportBIServices services = new ReportBIServices(this.DBConnection);
          
            output = services.CapNhatReportBi(model);

            /****************************************RESPONSE FAILE OR SUCCESS******************************************/

            //this.GetLanguage();
            BlockDepartmentCreateLangModel blockLang = new BlockDepartmentCreateLangModel();
            blockLang.BlockName = "block_report_bi_create";
            blockLang.SetLanguage(this.LANGUAGE_OBJECT);
            Session["msg_text"] = blockLang.GetMessage(output);
            Session["msg_code"] = output;

            if (services.ERROR != null) BI_Project.Helpers.FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");

            if (model.Id > 0 && output > 0)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_report_bi_create.success_edit", this.LANGUAGE_OBJECT);
            }
            if (output == 0)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_report_bi_create.error_business_1", this.LANGUAGE_OBJECT);
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
           /* if ((bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }*/
            string id = this.HttpContext.Request["id"];
            int output = 0;
            try
            {
                this.SetConnectionDB();



                ReportBIServices services = new ReportBIServices(this.DBConnection);
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

        private List<EntityCycleModel> setListCycle()
        {
            List<EntityCycleModel> listkq=new List<EntityCycleModel>();
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
        public JsonResult Delete(int id)
        {
            Boolean result = false;
            this.SetConnectionDB();
            ReportBIServices services = new ReportBIServices(this.DBConnection);
            if (services.ERROR != null) throw new Exception(services.ERROR);
            services.Delete(id);
            result = true;
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetReportById(int reportId)
        {
            this.SetConnectionDB();
            ReportBIServices services = new ReportBIServices(this.DBConnection);
            EntityReportBIModel reportSelected = services.GetEntityById(reportId);

            string value = string.Empty;
            value = JsonConvert.SerializeObject(reportSelected, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);

        }
       
        public JsonResult Save(EntityReportBIModel  model)
        {
            Boolean result = false;
            this.SetConnectionDB();
            ReportBIServices services = new ReportBIServices(this.DBConnection);
            services.CapNhatReportBi(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<EntityReportBIModel> setAreaNameforListReport(List<EntityReportBIModel> listReport, List<EntityAreaModel> listArea)
        {
            if (listReport.Any())
            {
                foreach(EntityReportBIModel report in listReport)
                {
                    report.AreaName = getAreaName(report, listArea);
                }
            }
            return listReport;
        }
        private string getAreaName(EntityReportBIModel report, List<EntityAreaModel> listArea)
        {
            string areaName="";
            if (listArea.Any())
            {
                foreach (EntityAreaModel area in listArea)
                {
                    if(report.AreaId== area.Id)
                    {
                        areaName = area.AreaName;
                    }
                }
            }

            return areaName;
        
        }
    }
}