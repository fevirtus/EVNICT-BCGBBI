using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BI_Project.Services.GBTask;
using BI_Project.Models.EntityModels;
using BI_Project.Models.UI;
using BI_Project.Helpers;
using BI_Project.Helpers.Utility;
using System.Text.RegularExpressions;
using BI_Project.Services;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

namespace BI_Project.Controllers
{
    public class GBTaskController : BaseController
    {
        string m_BlockModel = "block_task_list";
        string m_action_block = "GBTask/block_task_list";
        string m_pagename = "task_list";

        string m_action_block_addNew = "block_user_create";
        string m_pagename_addNew = "user_create";



        public ActionResult List()
        {          
            this.SetCommonData();
            ViewData["pagename"] = m_pagename;
            ViewData["action_block"] = m_action_block;//"GBTask/block_user_list";
            this.GetLanguage();
            this.SetConnectionDB();
            GBTaskServices services = new GBTaskServices(this.DBConnection);

            BlockLangGBTaskListModel blockLang = new BlockLangGBTaskListModel();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel(m_BlockModel, this.LANGUAGE_OBJECT, blockLang);

            //Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);

            //ViewData["departments"] = departmentServices.GetList();
            string id = (Request.QueryString["ReportRequirementId"] == null ? "0" : Request.QueryString["ReportRequirementId"].ToString());
            int ReportRequirementId = Convert.ToInt32(id);
            ViewData["ReportRequirementId"] = ReportRequirementId;
            blockModel.DataModel = services.GetList(ReportRequirementId);
            List<EntityGBRPTEvaluateModel> lstEvaluate = services.GetListEvaluate(ReportRequirementId);
            ViewData["Evaluates"] = lstEvaluate;

            Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);
            ViewData["departments"] = departmentServices.GetList();
            ViewData["LISTTASK"] = services.GetList(ReportRequirementId);
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            Logging.WriteToLog(this.GetType().ToString() + "-List()", LogType.Access);
            
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        public ActionResult Create()
        {
            this.SetCommonData();

            ViewData["pagename"] = "task_create";
            ViewData["action_block"] = "GBTask/block_task_create";
            ViewData["data_form1"] = TempData["data"];

            string id = (Request.QueryString["id"] == null ? "0" : Request.QueryString["id"].ToString());
            string RptId = (Request.QueryString["ReportRequirementId"] == null ? "0" : Request.QueryString["ReportRequirementId"].ToString());
            int ReportRequirementId = Convert.ToInt32(RptId);

            this.SetConnectionDB();
            GBTaskServices services = new GBTaskServices(this.DBConnection);
            EntityCreateTaskModel model = new EntityCreateTaskModel();
            if (TempData["data"] != null)
            {
                model = (EntityCreateTaskModel)ViewData["data_form1"];
            }
            else
            {
                BlockDataGBTaskCreateModel task = new BlockDataGBTaskCreateModel();
                task = services.GetEntityById(Int32.Parse(id));
                model.Created = DateTime.Now;
                model.Id = task.Id;
                model.Title = task.Title;
                model.Deadline = task.Deadline.ToString("dd/MM/yyyy");
                model.DepartmentCode = task.DepartmentCode;
                model.Description = task.Description;
                model.ImageFile = task.ImageFile;
                model.ResultFile = task.ResultFile;
                model.DataStatus = task.DataStatus;
                model.Comment = task.Comment;
                model.Status = task.Status;
                model.ReportRequirementId = task.ReportRequirementId;
            }


            //BlockLangGBTaskListModel blockLang = new BlockLangGBTaskListModel();

            Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);
            ViewData["departments"] = departmentServices.GetList();
            if (model.Id == 0)
            {
                model.ReportRequirementId = ReportRequirementId;
                model.Deadline = DateTime.Now.ToString("dd/MM/yyyyy");
                ViewData["CurrentDepartmentCode"] = departmentServices.GetList()[0].Code;
            }
            else
            {
                ViewData["CurrentDepartmentCode"] = model.DepartmentCode;
            }
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_task_create");
            blockModel.DataModel = model;
            ViewData["BlockData"] = blockModel;
            ViewData["data_form1"] = model;

            this.GetLanguage();
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        public JsonResult GetTaskById(int Id)
        {
            this.SetConnectionDB();
            GBTaskServices services = new GBTaskServices(this.DBConnection);
            BlockDataGBTaskCreateModel task = services.GetEntityById(Id);
            EntityCreateTaskModel model = new EntityCreateTaskModel();
            
            model.Created = DateTime.Now;
            model.Id = task.Id;
            model.Title = task.Title;
            model.Deadline = task.Deadline.ToString("dd/MM/yyyy");
            model.DepartmentCode = task.DepartmentCode;
            model.Description = task.Description;
            model.ImageFile = task.ImageFile;
            model.ResultFile = task.ResultFile;
            model.DataStatus = task.DataStatus;
            model.Comment = task.Comment;
            model.Status = task.Status;
            model.ReportRequirementId = task.ReportRequirementId;
            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Save(BlockDataGBTaskCreateModel model)
        {
            this.SetConnectionDB();
            GBTaskServices services = new GBTaskServices(this.DBConnection);
            var checkData = services.FindById(model.Id);
            int result = 0;
            if (checkData != null)
            {
                //model.CreatorId = Session[this.SESSION_NAME_USERID];
                result = services.CreateTask(model);
            }
            else
            {
                model.Id = 0;
                result = services.CreateTask(model);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEvaluateByDept(string DepartmentCode,int ReportRequirementId)
        {   
            this.SetConnectionDB();
            GBTaskServices services = new GBTaskServices(this.DBConnection);
            
            List<EntityGBRPTEvaluateModel> lstEvaluate = services.GetListEvaluate(ReportRequirementId);
            EntityGBRPTEvaluateModel model = lstEvaluate.Find(x => x.DepartmentCode.Equals(DepartmentCode));
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_task_evaluate");
            blockModel.DataModel = model;
            ViewData["BlockData"] = blockModel;

            ViewData["data_form1"] = model;
            Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);
            ViewData["departments"] = departmentServices.GetList();
            if (model != null)
            {
                if (model.DepartmentCode_Evaluate != null)
                    ViewData["CurrentDepartmentCode"] = model.DepartmentCode_Evaluate;
            }

            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Evaluate()
        {           
            this.SetCommonData();

            ViewData["pagename"] = "task_evaluate";
            ViewData["action_block"] = "GBTask/block_task_evaluate";
            ViewData["data_form1"] = TempData["data"];

            string DepartmentCode = (Request.QueryString["DepartmentCode"] == null ? "0" : Request.QueryString["DepartmentCode"].ToString());
            this.SetConnectionDB();
            GBTaskServices services = new GBTaskServices(this.DBConnection);
            string id = (Request.QueryString["ReportRequirementId"] == null ? "0" : Request.QueryString["ReportRequirementId"].ToString());
            int ReportRequirementId = Convert.ToInt32(id);

            List<EntityGBRPTEvaluateModel> lstEvaluate = services.GetListEvaluate(ReportRequirementId);
            EntityGBRPTEvaluateModel model = lstEvaluate.Find(x => x.DepartmentCode.Equals(DepartmentCode));
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_task_evaluate");
            blockModel.DataModel = model;
            ViewData["BlockData"] = blockModel;

            ViewData["data_form1"] = model;
            Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);
            ViewData["departments"] = departmentServices.GetList();
            if (model != null)
            {
                if (model.DepartmentCode_Evaluate != null)
                    ViewData["CurrentDepartmentCode"] = model.DepartmentCode_Evaluate;
            }
            this.GetLanguage();
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        public ActionResult Confirm()
        {         
            this.SetCommonData();


            ViewData["pagename"] = "task_confirm";
            ViewData["action_block"] = "GBTask/block_task_confirm";
            ViewData["data_form1"] = TempData["data"];

            string id = (Request.QueryString["id"] == null ? "0" : Request.QueryString["id"].ToString());
            this.SetConnectionDB();
            GBTaskServices services = new GBTaskServices(this.DBConnection);
            BlockDataGBTaskCreateModel model = new BlockDataGBTaskCreateModel();
            if (TempData["data"] != null)
            {
                model = (BlockDataGBTaskCreateModel)ViewData["data_form1"];
            }
            else
            {
                model = services.GetEntityById(Int32.Parse(id));
            }
            //BlockLangGBTaskListModel blockLang = new BlockLangGBTaskListModel();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_task_confirm");
            blockModel.DataModel = model;
            ViewData["BlockData"] = blockModel;
            ViewData["data_form1"] = model;



            //Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);
            //ViewData["departments"] = departmentServices.GetList();
            //ViewData["CurrentDepartmentCode"] = model.DepartmentCode;
            ViewData["currentStatus"] = model.Status;

            this.GetLanguage();
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + BI_Project.Helpers.Utility.APIStringHelper.GenerateFileId() + ".txt");

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpPost]
        //Hàm lưu
        public ActionResult Create(EntityCreateTaskModel model)
        {
            BlockDataGBTaskCreateModel task = new BlockDataGBTaskCreateModel();
            CultureInfo provider = CultureInfo.InvariantCulture;
            task.Created = DateTime.Now;
            task.Id = model.Id;
            task.Title = model.Title;
            task.Deadline = DateTime.ParseExact(model.Deadline, "dd/MM/yyyy", provider); ;
            task.DepartmentCode = model.DepartmentCode;
            task.Description = model.Description;
            task.ImageFile = model.ImageFile;
            task.ResultFile = model.ResultFile;
            task.DataStatus = model.DataStatus;
            task.Comment = model.Comment;
            task.Status = model.Status;
            task.ReportRequirementId = model.ReportRequirementId;

            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);

            ViewData["data_form1"] = TempData["data"];
            this.GetLanguage();
            //**************** DATABASE PROCESS*******************************************************
            this.SetConnectionDB();

            GBTaskServices services = new GBTaskServices(this.DBConnection);
            // check tài khoản đã tồn tại trong hệ thống hay chưa

            var checkData = services.FindById(model.Id);
            int result = 0;
            if (checkData != null)
            {
                result = services.CreateTask(task);
            }
            else
            {
                model.Id = 0;
                result = services.CreateTask(task);
            }
            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            //***********************INSERT OR EDIT SUCCESSFULLY * *************************************************
            List<EntityGBTaskModel> LISTTASK = services.GetList(model.ReportRequirementId);
            ViewData["LISTTASK"] = LISTTASK;
            return PartialView("~/Views/BCGB/_Task.cshtml", new ViewDataDictionary {
                { "LISTTASK", LISTTASK },
                { "ReportRequirementId", model.ReportRequirementId}
            });
        }

        [HttpPost]
        //Hàm lưu
        public void SaveTask(BlockDataGBTaskCreateModel model)
        {

            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);
           
            ViewData["data_form1"] = TempData["data"];

            //**************** DATABASE PROCESS*******************************************************
            this.SetConnectionDB();

            GBTaskServices services = new GBTaskServices(this.DBConnection);
            // check tài khoản đã tồn tại trong hệ thống hay chưa

            var checkData = services.FindById(model.Id);
            int result = 0;
            if (checkData != null)
            {
                //model.CreatorId = Session[this.SESSION_NAME_USERID];
                result = services.CreateTask(model);
            }
            else
            {
                model.Id = 0;
                result = services.CreateTask(model);
            }

            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            //***********************INSERT OR EDIT SUCCESSFULLY * *************************************************            
        }

        [HttpPost]
        //Hàm lưu
        public ActionResult Confirm(BlockDataGBTaskCreateModel model)
        {

            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);
           

            ViewData["data_form1"] = TempData["data"];
            // get language
            this.GetLanguage();
            if (string.IsNullOrEmpty(model.Comment))
            {
                return RedirectToAction("List");
            }

            //**************** DATABASE PROCESS*******************************************************
            this.SetConnectionDB();

            GBTaskServices services = new GBTaskServices(this.DBConnection);
            // check tài khoản đã tồn tại trong hệ thống hay chưa

            var checkData = services.FindById(model.Id);
            int result = 0;
            if (checkData != null)
            {
                //model.CreatorId = Session[this.SESSION_NAME_USERID];

                HttpPostedFileBase postedFile = model.ImageFile;

                if (postedFile != null)
                {
                    string path = Server.MapPath("~/Uploads/TASK/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string[] arrStr = postedFile.FileName.ToString().Split('.');
                    string fileName = "TASK_" + model.Id.ToString() + "." + arrStr[1];
                    postedFile.SaveAs(path + Path.GetFileName(fileName));
                    model.ResultFile = "/Uploads/TASK/" + fileName;


                }
                result = services.ConfirmTask(model);
            }
            else
            {
                return RedirectToAction("List", "BCGB", new { ReportRequirementId = model.ReportRequirementId });
            }

            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");


            //***********************INSERT OR EDIT SUCCESSFULLY * *************************************************
            if (result > 0)
            {
                //                return RedirectToAction("List");
                return RedirectToAction("List", "BCGB", new { ReportRequirementId = model.ReportRequirementId });
            }
            TempData["data"] = model;
            //return RedirectToAction("Confirm");
            return RedirectToAction("List", "BCGB", new { ReportRequirementId = model.ReportRequirementId });

        }

        [HttpPost]
        //Hàm lưu
        public ActionResult DepartConfirm(BlockDataGBTaskCreateModel model)
        {

            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);


            ViewData["data_form1"] = TempData["data"];
            // get language
            this.GetLanguage();
            //if (string.IsNullOrEmpty(model.Comment))
            //{
            //    return RedirectToAction("ViewDetail");
            //}

            //**************** DATABASE PROCESS*******************************************************
            this.SetConnectionDB();
            GBTaskServices services = new GBTaskServices(this.DBConnection);
                      
                        
            // check tài khoản đã tồn tại trong hệ thống hay chưa
            var checkData = services.FindById(model.Id);
            int result = 0;
            if (checkData != null)
            {
                //model.CreatorId = Session[this.SESSION_NAME_USERID];

                HttpPostedFileBase postedFile = model.ImageFile;

                if (postedFile != null)
                {
                    string path = Server.MapPath("~/Uploads/TASK/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string[] arrStr = postedFile.FileName.ToString().Split('.');
                    string fileName = "TASK_" + model.Id.ToString() + "." + arrStr[1];
                    postedFile.SaveAs(path + Path.GetFileName(fileName));
                    model.ResultFile = "/Uploads/TASK/" + fileName;


                }
                result = services.ConfirmTask(model);
            }
            else
            {
                List<EntityGBTaskModel> LISTTASK = services.GetList(model.ReportRequirementId);
                ViewData["LISTTASK"] = LISTTASK;
                return PartialView("_Task", new ViewDataDictionary {
                { "LISTTASK", LISTTASK },
                { "ReportRequirementId", model.ReportRequirementId}
            });
            }

            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");


            //***********************INSERT OR EDIT SUCCESSFULLY * *************************************************
            if (result > 0)
            {
                //                return RedirectToAction("List");
                return PartialView("_Task", new ViewDataDictionary {
                { "LISTTASK", LISTTASK },
                { "ReportRequirementId", model.ReportRequirementId}
            });
            }
            //TempData["data"] = model;
            ////return RedirectToAction("Confirm");
            //return RedirectToAction("ViewDetail", "BCGB", new { ReportRequirementId = model.ReportRequirementId });
            
            return PartialView("_Task", new ViewDataDictionary {
                { "LISTTASK", LISTTASK },
                { "ReportRequirementId", model.ReportRequirementId}
            });
        }
        
        [HttpPost]
        //Hàm lưu
        public ActionResult Evaluate(EntityGBRPTEvaluateModel model)
        {
            Logging.WriteToLog(this.GetType().ToString() + "-create()", LogType.Access);

            ViewData["data_form1"] = TempData["data"];
            //**************** DATABASE PROCESS*******************************************************
            this.SetConnectionDB();
            GBTaskServices services = new GBTaskServices(this.DBConnection);           

            // check tài khoản đã tồn tại trong hệ thống hay chưa
            int result = 0;
            model.CreatorId = (int)Session[this.SESSION_NAME_USERID];
            result = services.CreateEvaluate(model);
            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            //***********************INSERT OR EDIT SUCCESSFULLY * *************************************************
            List<EntityGBRPTEvaluateModel> lstEvaluate = services.GetListEvaluate(model.ReportRequirementId);
            ViewData["Evaluates"] = lstEvaluate;
            TempData["data"] = model;
            //return RedirectToAction("List");            
            return PartialView("_Evalue", new ViewDataDictionary {
                { "Evaluates", lstEvaluate }
            });
        }
                
        [HttpPost]
        public ActionResult Delete(string TaskId)
        {
            string id = (Request.QueryString["ReportRequirementId"] == null ? "0" : Request.QueryString["ReportRequirementId"].ToString());
            int ReportRequirementId = Convert.ToInt32(id);
            //string rptId = this.HttpContext.Request["ReportRequirementId"];
            Logging.WriteToLog(this.GetType().ToString() + "-delete(), id=" + TaskId, LogType.Access);
            int output = 0;
            this.SetConnectionDB();
            GBTaskServices services = new GBTaskServices(this.DBConnection);
            try
            {
                output = services.Delete(Int32.Parse(TaskId));
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
                Session["msg_text"] = "Xóa thành công";
            }
            else
            {
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "commons.messages.ServerError");
            }
            Session["msg_code"] = output;

            List<EntityGBTaskModel> LISTTASK = services.GetList(Int32.Parse(id));
            ViewData["LISTTASK"] = services.GetList(Int32.Parse(id));
            return PartialView("_Task", new ViewDataDictionary {
                { "LISTTASK", LISTTASK },
                { "ReportRequirementId", Int32.Parse(id)}
            });
        }

    }
}