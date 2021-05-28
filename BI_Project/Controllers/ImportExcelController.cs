using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.IO;

using BI_Project.Services.Importers;
using BI_Project.Models.EntityModels;
using BI_Project.Models.UI;
using BI_Project.Helpers;
using BI_Project.Helpers.Utility;

namespace BI_Project.Controllers
{
    public class ImportExcelController : BaseController
    {
        public string EXCEL_UPLOAD_FOLDER { set; get; }

        
        public ImportExcelController():base()
        {
            this.EXCEL_UPLOAD_FOLDER = WebConfigurationManager.AppSettings["EXCEL_UPLOAD_FOLDER"];
            //this.DBConnection.ConnectString = this.CONNECT_STRING_STAGING;
        }

        [HttpGet]
        [CheckUserMenus]
        public ActionResult Index(string id)
        {
            int noPages =0 , noRecords = 0;
            BlockUIExcelUploadModel uiModel = new BlockUIExcelUploadModel();
            try
            {
                uiModel.CurrentPage = Int32.Parse( Request.Params["page"]);
                
            }
            catch(Exception)
            {
                uiModel.CurrentPage = 1;
            }
            
            
            this.CheckPermission();
            
            ViewData["pagename"] = "upload_excel";
            ViewData["action_block"] = "Excels/block_upload_excel";
            this.SetCommonData();
            this.GetLanguage();

            //DECLARE PAGE MODEL
            BI_Project.Models.UI.PageModel pageModel = new Models.UI.PageModel("upload_excel");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            pageModel.H1Title = pageModel.GetElementByPath("page_excel.menu" + id + ".h1");
            pageModel.Title = pageModel.GetElementByPath("page_excel.menu" + id + ".title");
            ViewData["page_model"] = pageModel;

            
            BlockLangExcelUpload blockLang = new BlockLangExcelUpload();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_upload_excel", this.LANGUAGE_OBJECT, blockLang);
            BlockDataExcelUploadModel blockData = new BlockDataExcelUploadModel();
            blockData.PermissionID = id;

            //BI_Project.Helpers.Utility.JTokenHelper.GetLanguage("~/" + this.LANGUAGE_FOLDER, this.LANGUAGE);
            string xmlConfigFilePath = this.CONFIG_FOLDER + "\\excel_format_" + blockData.PermissionID.ToString() + ".xml";
            blockData.HelpDoc = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.HelpDocumentPath");
            blockData.Note = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Note.#cdata-section");
            ImporterServices services = new ImporterServices(this.DBConnection);

            blockData.ListHistory = services.GetHistoryList(uiModel.CurrentPage, uiModel.PerPage, id,ref noPages,ref noRecords,uiModel.Month,uiModel.Year);
            blockData.NumberPages = noPages;
            blockData.NumberRecords = noPages;
            blockData.CurrentPage = uiModel.CurrentPage;
            blockData.FolderUpload = pageModel.GetElementByPath("page_excel.menu" + id + ".UploadedDirectory");
            blockModel.DataModel = blockData;
            //blockModel.DataModel = ViewData["block_menu_left_data"];
            if(services.ERROR != null) FileHelper.SaveFile(services.ERROR , this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            ViewData["BlockData"] = blockModel;

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpPost]
        public ActionResult Index(string id, BlockDataExcelUploadModel uiModel)
        {
            int noPages = 0, noRecords = 0;
            
            try
            {
                uiModel.CurrentPage = Int32.Parse(Request.Params["page"]);

            }
            catch (Exception)
            {
                uiModel.CurrentPage = 1;
            }


            this.CheckPermission();

            ViewData["pagename"] = "upload_excel";
            ViewData["action_block"] = "Excels/block_upload_excel";
            this.SetCommonData();
            this.GetLanguage();

            //DECLARE PAGE MODEL
            BI_Project.Models.UI.PageModel pageModel = new Models.UI.PageModel("upload_excel");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            pageModel.H1Title = pageModel.GetElementByPath("page_excel.menu" + id + ".h1");
            pageModel.Title = pageModel.GetElementByPath("page_excel.menu" + id + ".title");
            ViewData["page_model"] = pageModel;


            BlockLangExcelUpload blockLang = new BlockLangExcelUpload();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_upload_excel", this.LANGUAGE_OBJECT, blockLang);
            //BlockDataExcelUploadModel blockData = new BlockDataExcelUploadModel();
            uiModel.PermissionID = id;
            ImporterServices services = new ImporterServices(this.DBConnection);

            string xmlConfigFilePath = this.CONFIG_FOLDER + "\\excel_format_" + uiModel.PermissionID.ToString() + ".xml";
            uiModel.HelpDoc = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.HelpDocumentPath");

            uiModel.ListHistory = services.GetHistoryList(uiModel.CurrentPage, uiModel.PerPage, id, ref noPages, ref noRecords,uiModel.Month,uiModel.Year);
            uiModel.NumberPages = noPages;
            uiModel.NumberRecords = noPages;
            uiModel.CurrentPage = uiModel.CurrentPage;
            uiModel.FolderUpload = pageModel.GetElementByPath("page_excel.menu" + id + ".UploadedDirectory");
            blockModel.DataModel = uiModel;
            //blockModel.DataModel = ViewData["block_menu_left_data"];
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            ViewData["BlockData"] = blockModel;

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }



        //[HttpPost]
        //public ActionResult Import(FileUploadModel model)
        //{
        //    this.GetLanguage();

        //    //DECLARE PAGE MODEL

        //    try
        //    {
        //        //************************** CHECK PERMISSION **************************
        //        this.SetConnectionDB();

        //        //************************** GET CONFIG
        //        ImporterServices services = new ImporterServices(this.DBConnection, this.CONNECT_STRING_STAGING);
        //        string tablename = "", sheetActive = "", uploadFolder = "", fileNativeName = "";
        //        string note = "", helpDocumentPath = "";
        //        int startRow = 0;
        //        string xmlFilePath = this.CONFIG_FOLDER + "\\excel_format_" + model.PermissionId.ToString() + ".xml";
        //        List<MappingExcelDB> lstColumns = services.GetColumnList(xmlFilePath, ref uploadFolder, ref tablename,
        //            ref startRow, ref sheetActive, ref helpDocumentPath, ref note, ref fileNativeName);



        //        //************************** UPLOAD FILE TO THE UPLOAD FOLDER***********************************************
        //        string excelfilename = BI_Project.Helpers.Utility.APIStringHelper.GenerateId() + Path.GetExtension(model.FileObj.FileName);

        //        string excelFilePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_UPLOAD_FOLDER) + uploadFolder + "/" + excelfilename;
        //        model.FileObj.SaveAs(excelFilePath);

        //        //************************ INSERT DATA TO DATABASE ********************

        //        int userid = (int)Session[this.SESSION_NAME_USERID];
        //        int uploadRoleId = model.PermissionId;
        //        services.Import2Database(userid, excelFilePath, tablename, startRow, sheetActive, helpDocumentPath,
        //            note, fileNativeName, fileNativeName, uploadRoleId, lstColumns, excelfilename);
        //        if (services.ERROR != null) throw new Exception(services.ERROR);

        //        Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.success");

        //        Session["msg_code"] = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        FileHelper.SaveFile(new { ERROR = ex }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
        //        Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.error");

        //        Session["msg_code"] = -1;
        //    }

        //    return RedirectToAction("Index/" + model.PermissionId);
        //}


        [HttpPost]
        public ActionResult Import(FileUploadModel model)
        {
            this.GetLanguage();

            //DECLARE PAGE MODEL

            try
            {
                //************************** CHECK PERMISSION **************************
                this.SetConnectionDB();

                //var test = model.FileObj;

                //************************** GET CONFIG
                ImporterServices services = new ImporterServices(this.DBConnection, this.CONNECT_STRING_STAGING);
                string tablename = "", sheetActive = "", uploadFolder = "", fileNativeName = "";
                string note = "", helpDocumentPath = "";
                int startRow = 0, numberRow = 0, numberCell = 0;
                List<string> lstCellName = new List<string>();
                string xmlFilePath = this.CONFIG_FOLDER + "\\excel_format_" + model.PermissionId.ToString() + ".xml";
                List<MappingExcelDB> lstColumns = services.GetColumnList(xmlFilePath, ref uploadFolder, ref tablename,
                    ref startRow, ref sheetActive, ref helpDocumentPath, ref note, ref fileNativeName, ref numberRow, ref numberCell, ref lstCellName);



                //************************** UPLOAD FILE TO THE UPLOAD FOLDER***********************************************
                string excelfilename = BI_Project.Helpers.Utility.APIStringHelper.GenerateId() + Path.GetExtension(model.FileObj.FileName);

                string excelFilePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_UPLOAD_FOLDER) + uploadFolder + "/" + excelfilename;
                model.FileObj.SaveAs(excelFilePath);

                //************************ INSERT DATA TO DATABASE ********************

                int userid = (int)Session[this.SESSION_NAME_USERID];
                int uploadRoleId = model.PermissionId;
                //services.Import2Database(userid,excelFilePath, tablename, startRow, sheetActive,helpDocumentPath,
                //    note,fileNativeName,fileNativeName,uploadRoleId, lstColumns,excelfilename);
                services.Import2Database(userid, excelFilePath, tablename, startRow, sheetActive, helpDocumentPath,
                    note, fileNativeName, fileNativeName, uploadRoleId, lstColumns, excelfilename, numberRow, numberCell, lstCellName);
                if (services.ERROR != null) throw new Exception(services.ERROR);

                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.success");

                Session["msg_code"] = 1;
            }
            catch (Exception ex)
            {
                FileHelper.SaveFile(new { ERROR = ex }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.error");

                Session["msg_code"] = -1;
            }

            return RedirectToAction("Index/" + model.PermissionId);
        }



    }
}