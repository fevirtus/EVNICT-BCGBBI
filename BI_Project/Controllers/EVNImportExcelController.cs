using BI_Project.Helpers;
using BI_Project.Helpers.Utility;
using BI_Project.Models.UI;
using BI_Project.Services.Importers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace BI_Project.Controllers
{
    public class EVNImportExcelController : BaseController
    {
        // GET: EVNImportExcel
        public string EXCEL_UPLOAD_FOLDER { set; get; }
        public string EXCEL_EXPORT_FOLDER { get; set; }


        public EVNImportExcelController() : base()
        {
            this.EXCEL_UPLOAD_FOLDER = WebConfigurationManager.AppSettings["EXCEL_UPLOAD_FOLDER"];
            this.EXCEL_EXPORT_FOLDER = WebConfigurationManager.AppSettings["EXCEL_EXPORT_FOLDER"];
        }

        [HttpGet]
        //[CheckUserMenus]
        public ActionResult Index(string id)
        {
            int noPages = 0, noRecords = 0;
            string vID = "1";
            BlockUIExcelUploadModel uiModel = new BlockUIExcelUploadModel();
            try
            {
                uiModel.CurrentPage = Int32.Parse(Request.Params["page"]);

            }
            catch (Exception e )
            {
                uiModel.CurrentPage = 1;
            }
            this.CheckPermission();
            ViewData["DEPARTMENTID"] = Session["CodeIsAdmin"];
            ViewData["pagename"] = "Nhận kết quả từ LRS";
            ViewData["action_block"] = "Excels/UploadLRS";
            this.SetCommonData();
            this.GetLanguage();

            //DECLARE PAGE MODEL
            BI_Project.Models.UI.PageModel pageModel = new Models.UI.PageModel("upload_excel");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            pageModel.H1Title = "Nhận dữ liệu kết quả NCPT từ chương trình ITRON-LRS";
            pageModel.Title = "Nhận file LRS";
            ViewData["page_model"] = pageModel;


            BlockLangExcelUpload blockLang = new BlockLangExcelUpload();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_upload_excel", this.LANGUAGE_OBJECT, blockLang);
            BlockDataExcelUploadModel blockData = new BlockDataExcelUploadModel();
            blockData.PermissionID = vID;

            //BI_Project.Helpers.Utility.JTokenHelper.GetLanguage("~/" + this.LANGUAGE_FOLDER, this.LANGUAGE);
            string xmlConfigFilePath = this.CONFIG_FOLDER + "\\excel_format_" + blockData.PermissionID.ToString() + ".xml";
            blockData.HelpDoc = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.HelpDocumentPath");
            blockData.Note = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Note.#cdata-section");
            blockData.Export = Convert.ToBoolean(BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.isExport"));
            blockData.ExportBaseMonth = Convert.ToBoolean(BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.isExportBaseMonth"));
            blockData.Url = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.url");

            blockData.ExportOff = Convert.ToBoolean(BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.isExportOff"));


            EVNImporterServices services = new EVNImporterServices(DBConnection);
            //ImporterServices _services = new ImporterServices(DBConnection);
            int userid = (int)Session[this.SESSION_NAME_USERID];            
            blockData.ListHistory = services.GetHistoryList(userid, uiModel.CurrentPage, uiModel.PerPage, vID, ref noPages, ref noRecords, uiModel.Month, uiModel.Year);
            blockData.NumberPages = noPages;
            blockData.NumberRecords = noPages;
            blockData.CurrentPage = uiModel.CurrentPage;
            blockData.FolderUpload = pageModel.GetElementByPath("page_excel.menu" + vID + ".UploadedDirectory");
            blockModel.DataModel = blockData;
            //blockModel.DataModel = ViewData["block_menu_left_data"];
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            ViewData["BlockData"] = blockModel;

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }


        [HttpGet]
        //[CheckUserMenus]
        public ActionResult ImpTotal(string id)
        {
            int noPages = 0, noRecords = 0;
            string vID = "2";
            BlockUIExcelUploadModel uiModel = new BlockUIExcelUploadModel();
            try
            {
                uiModel.CurrentPage = Int32.Parse(Request.Params["page"]);

            }
            catch (Exception e)
            {
                uiModel.CurrentPage = 1;
            }
            this.CheckPermission();
            ViewData["DEPARTMENTID"] = Session["CodeIsAdmin"];
            ViewData["pagename"] = "Nhận dữ liệu điện nhận đầu nguồn toàn đơn vị";
            ViewData["action_block"] = "Excels/UploadDauNguon";
            this.SetCommonData();
            this.GetLanguage();

            //DECLARE PAGE MODEL
            BI_Project.Models.UI.PageModel pageModel = new Models.UI.PageModel("upload_excel");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            pageModel.H1Title = pageModel.GetElementByPath("page_excel.menu" + vID + ".h1");
            pageModel.Title = pageModel.GetElementByPath("page_excel.menu" + vID + ".title");
            ViewData["page_model"] = pageModel;


            BlockLangExcelUpload blockLang = new BlockLangExcelUpload();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_upload_excel", this.LANGUAGE_OBJECT, blockLang);
            BlockDataExcelUploadModel blockData = new BlockDataExcelUploadModel();
            blockData.PermissionID = vID;

            //BI_Project.Helpers.Utility.JTokenHelper.GetLanguage("~/" + this.LANGUAGE_FOLDER, this.LANGUAGE);
            string xmlConfigFilePath = this.CONFIG_FOLDER + "\\excel_format_" + blockData.PermissionID.ToString() + ".xml";
            blockData.HelpDoc = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.HelpDocumentPath");
            blockData.Note = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Note.#cdata-section");
            blockData.Export = Convert.ToBoolean(BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.isExport"));
            blockData.ExportBaseMonth = Convert.ToBoolean(BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.isExportBaseMonth"));
            blockData.Url = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.url");

            blockData.ExportOff = Convert.ToBoolean(BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.isExportOff"));


            EVNImporterServices services = new EVNImporterServices(DBConnection);
            //ImporterServices _services = new ImporterServices(DBConnection);
            int userid = (int)Session[this.SESSION_NAME_USERID];
            blockData.ListHistory = services.GetHistoryList(userid, uiModel.CurrentPage, uiModel.PerPage, vID, ref noPages, ref noRecords, uiModel.Month, uiModel.Year);
            blockData.NumberPages = noPages;
            blockData.NumberRecords = noPages;
            blockData.CurrentPage = uiModel.CurrentPage;
            blockData.FolderUpload = pageModel.GetElementByPath("page_excel.menu" + vID + ".UploadedDirectory");
            blockModel.DataModel = blockData;
            //blockModel.DataModel = ViewData["block_menu_left_data"];
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

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
            ViewData["DEPARTMENTID"] = Session["CodeIsAdmin"];
            ViewData["pagename"] = "upload_excel";
            ViewData["action_block"] = "Excels/UploadLRS";
            SetCommonData();
            GetLanguage();

            //DECLARE PAGE MODEL
            BI_Project.Models.UI.PageModel pageModel = new Models.UI.PageModel("upload_excel");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            pageModel.H1Title = "Nhận kết quả NCPT từ ITRON-LRS";
            pageModel.Title = "Nhận file LRS";
            ViewData["page_model"] = pageModel;


            BlockLangExcelUpload blockLang = new BlockLangExcelUpload();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_upload_excel", this.LANGUAGE_OBJECT, blockLang);
            //BlockDataExcelUploadModel blockData = new BlockDataExcelUploadModel();
            uiModel.PermissionID = id;
            EVNImporterServices services = new EVNImporterServices(DBConnection);

            string xmlConfigFilePath = this.CONFIG_FOLDER + "\\excel_format_" + uiModel.PermissionID.ToString() + ".xml";
            uiModel.HelpDoc = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.HelpDocumentPath");

            int userid = (int)Session[this.SESSION_NAME_USERID];
            uiModel.ListHistory = services.GetHistoryList(userid, uiModel.CurrentPage, uiModel.PerPage, id, ref noPages, ref noRecords, uiModel.Month, uiModel.Year);
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


        [HttpPost]
        public ActionResult ImpTotal(string id, BlockDataExcelUploadModel uiModel)
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
            ViewData["DEPARTMENTID"] = Session["CodeIsAdmin"];
            ViewData["pagename"] = "upload_excel";
            ViewData["action_block"] = "Excels/UploadDauNguon";
            SetCommonData();
            GetLanguage();

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
            EVNImporterServices services = new EVNImporterServices(DBConnection);

            string xmlConfigFilePath = this.CONFIG_FOLDER + "\\excel_format_" + uiModel.PermissionID.ToString() + ".xml";
            uiModel.HelpDoc = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.HelpDocumentPath");

            int userid = (int)Session[this.SESSION_NAME_USERID];
            uiModel.ListHistory = services.GetHistoryList(userid, uiModel.CurrentPage, uiModel.PerPage, id, ref noPages, ref noRecords, uiModel.Month, uiModel.Year);
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
                EVNImporterServices services = new EVNImporterServices(DBConnection);
                string tablename = "", sheetActive = "", uploadFolder = "", fileNativeName = "";
                string note = "", helpDocumentPath = "";
                int startRow = 0, numberRow = 0, numberCell = 0;
                List<string> lstCellName = new List<string>();
                string xmlFilePath = this.CONFIG_FOLDER + "\\excel_format_" + model.PermissionId.ToString() + ".xml";
                List<MappingExcelDB> lstColumns = services.GetColumnList(xmlFilePath, ref uploadFolder, ref tablename,
                    ref startRow, ref sheetActive, ref helpDocumentPath, ref note, ref fileNativeName); //, ref numberRow, ref numberCell, ref lstCellName



                //************************** UPLOAD FILE TO THE UPLOAD FOLDER***********************************************
                string excelfilename = BI_Project.Helpers.Utility.APIStringHelper.GenerateId() + Path.GetExtension(model.FileObj.FileName);

                string excelFilePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_UPLOAD_FOLDER) + uploadFolder + "/" + excelfilename;
                model.FileObj.SaveAs(excelFilePath);

                //************************ INSERT DATA TO DATABASE ********************

                int userid = (int)Session[this.SESSION_NAME_USERID];
                int uploadRoleId = model.PermissionId;
                int currentYear = model.currentyear;
                //services.Import2Database(userid,excelFilePath, tablename, startRow, sheetActive,helpDocumentPath,
                //    note,fileNativeName,fileNativeName,uploadRoleId, lstColumns,excelfilename);
                services.ImportToDatabase(currentYear,userid, excelFilePath, tablename, startRow, sheetActive, helpDocumentPath,
                    note, fileNativeName, fileNativeName, uploadRoleId, lstColumns, excelfilename); //, numberRow, numberCell, lstCellName
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


        /// <summary>
        /// Use with the xml file contains paras elements
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImportExcel(FileUploadModel model, string MaDV)
        {
            //Test run time upload excel
            //DateTime timeFinish = DateTime.UtcNow;
            //DateTime timeStart = DateTime.UtcNow;
            //TimeSpan timeSpan = new TimeSpan();
            //timeStart = DateTime.Parse(DateTime.Now.ToString("h:mm:ss tt"));
            this.GetLanguage();
            try
            {

                //--------------------GET XML CONFIG -----------------------------------------

                this.SetConnectionDB();
                EVNImporterServices services = new EVNImporterServices( DBConnection);
                List<string> lstCellName = new List<string>();
                string xmlFilePath = this.CONFIG_FOLDER + "\\excel_format_" + model.PermissionId.ToString() + ".xml";
                ExcelModel lstColumns = services.GetXMLConfig(xmlFilePath);

                //************************** UPLOAD FILE TO THE UPLOAD FOLDER***********************************************

                string excelfilename = BI_Project.Helpers.Utility.APIStringHelper.GenerateId() + Path.GetExtension(model.FileObj.FileName);

                //string excelFilePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_UPLOAD_FOLDER) + lstColumns.FolderUploadedDirectory + "/" + excelfilename;
                string excelFilePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_UPLOAD_FOLDER) + lstColumns.FolderUploadedDirectory + "\\" + excelfilename;
                model.FileObj.SaveAs(excelFilePath);

                //************************ INSERT DATA TO DATABASE ********************
                int userid = (int)Session[this.SESSION_NAME_USERID];
                int uploadRoleId = model.PermissionId;
                if (uploadRoleId == 1) 
                    services.Import2Database(MaDV, userid, excelFilePath, uploadRoleId, lstColumns, excelfilename);
                else
                    services.ImportTotalDatabase(MaDV, userid, excelFilePath, uploadRoleId, lstColumns, excelfilename);

                //services.Import2Database(userid, excelFilePath)
                if (services.ERROR != null) throw new Exception(services.ERROR);

                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.success");

                Session["msg_code"] = 1;

                //timeFinish = DateTime.Parse(DateTime.Now.ToString("h:mm:ss tt"));

                //timeSpan = timeFinish.Subtract(timeStart);

                //FileHelper.SaveFile(timeSpan, this.LOG_FOLDER + "/TIMESPAN" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            }

            catch (Exception ex)
            {
                //FileHelper.SaveFile(new { ERROR = ex }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
                ERRORS = ex.Message;
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.error") + " Lỗi " + ex.Message;

                Session["msg_code"] = -1;
            }
            if (model.PermissionId == 1)
                return RedirectToAction("Index/" + model.PermissionId);
            else
                return RedirectToAction("ImpTotal/" + model.PermissionId);
        }



        [HttpPost]
        public ActionResult ImportLRS(FileUploadModel model, string MaDV)
        {
            //Test run time upload excel
            //DateTime timeFinish = DateTime.UtcNow;
            //DateTime timeStart = DateTime.UtcNow;
            //TimeSpan timeSpan = new TimeSpan();
            //timeStart = DateTime.Parse(DateTime.Now.ToString("h:mm:ss tt"));
            this.GetLanguage();
            try
            {

                //--------------------GET XML CONFIG -----------------------------------------

                this.SetConnectionDB();
                EVNImporterServices services = new EVNImporterServices(DBConnection);
                List<string> lstCellName = new List<string>();
                string xmlFilePath = this.CONFIG_FOLDER + "\\excel_format_" + model.PermissionId.ToString() + ".xml";
                ExcelModel lstColumns = services.GetXMLConfig(xmlFilePath);

                //************************** UPLOAD FILE TO THE UPLOAD FOLDER***********************************************

                string excelfilename = BI_Project.Helpers.Utility.APIStringHelper.GenerateId() + Path.GetExtension(model.FileObj.FileName);

                //string excelFilePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_UPLOAD_FOLDER) + lstColumns.FolderUploadedDirectory + "/" + excelfilename;
                string excelFilePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_UPLOAD_FOLDER) + lstColumns.FolderUploadedDirectory + "\\" + excelfilename;
                model.FileObj.SaveAs(excelFilePath);

                //************************ INSERT DATA TO DATABASE ********************
                int userid = (int)Session[this.SESSION_NAME_USERID];
                int uploadRoleId = model.PermissionId;
                services.Import2Database(MaDV, userid, excelFilePath, uploadRoleId, lstColumns, excelfilename);
                
                //services.Import2Database(userid, excelFilePath)
                if (services.ERROR != null) throw new Exception(services.ERROR);

                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.success");

                Session["msg_code"] = 1;

                //timeFinish = DateTime.Parse(DateTime.Now.ToString("h:mm:ss tt"));

                //timeSpan = timeFinish.Subtract(timeStart);

                //FileHelper.SaveFile(timeSpan, this.LOG_FOLDER + "/TIMESPAN" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            }

            catch (Exception ex)
            {
                //FileHelper.SaveFile(new { ERROR = ex }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
                ERRORS = ex.Message;
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.error") + " Lỗi " + ex.Message;

                Session["msg_code"] = -1;
            }
            if (model.PermissionId == 1)
                return RedirectToAction("Index/" + model.PermissionId);
            else
                return RedirectToAction("ImpTotal/" + model.PermissionId);
        }


        [HttpPost]
        public ActionResult ImportTotal(FileUploadModel model, string MaDV)
        {
            //Test run time upload excel
            //DateTime timeFinish = DateTime.UtcNow;
            //DateTime timeStart = DateTime.UtcNow;
            //TimeSpan timeSpan = new TimeSpan();
            //timeStart = DateTime.Parse(DateTime.Now.ToString("h:mm:ss tt"));
            this.GetLanguage();
            try
            {

                //--------------------GET XML CONFIG -----------------------------------------

                this.SetConnectionDB();
                EVNImporterServices services = new EVNImporterServices(DBConnection);
                List<string> lstCellName = new List<string>();
                string xmlFilePath = this.CONFIG_FOLDER + "\\excel_format_" + model.PermissionId.ToString() + ".xml";
                ExcelModel lstColumns = services.GetXMLConfig(xmlFilePath);

                //************************** UPLOAD FILE TO THE UPLOAD FOLDER***********************************************

                string excelfilename = BI_Project.Helpers.Utility.APIStringHelper.GenerateId() + Path.GetExtension(model.FileObj.FileName);

                //string excelFilePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_UPLOAD_FOLDER) + lstColumns.FolderUploadedDirectory + "/" + excelfilename;
                string excelFilePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_UPLOAD_FOLDER) + lstColumns.FolderUploadedDirectory + "\\" + excelfilename;
                model.FileObj.SaveAs(excelFilePath);

                //************************ INSERT DATA TO DATABASE ********************
                int userid = (int)Session[this.SESSION_NAME_USERID];
                int uploadRoleId = model.PermissionId;
                services.ImportTotalDatabase(MaDV, userid, excelFilePath, uploadRoleId, lstColumns, excelfilename);

                //services.Import2Database(userid, excelFilePath)
                if (services.ERROR != null) throw new Exception(services.ERROR);

                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.success");

                Session["msg_code"] = 1;

                //timeFinish = DateTime.Parse(DateTime.Now.ToString("h:mm:ss tt"));

                //timeSpan = timeFinish.Subtract(timeStart);

                //FileHelper.SaveFile(timeSpan, this.LOG_FOLDER + "/TIMESPAN" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            }

            catch (Exception ex)
            {
                //FileHelper.SaveFile(new { ERROR = ex }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
                ERRORS = ex.Message;
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.error") + " Lỗi " + ex.Message;

                Session["msg_code"] = -1;
            }
            if (model.PermissionId == 1)
                return RedirectToAction("Index/" + model.PermissionId);
            else
                return RedirectToAction("ImpTotal/" + model.PermissionId);
        }

        [HttpGet]
        public ActionResult ExportExcel(string id)
        {
            string xmlFilePath = this.CONFIG_FOLDER + "\\excel_export_format.xml";

            BI_Project.Helpers.ExcelHelper excelHelper = new ExcelHelper();
            
            string excelFileName = "";

            ExcelXmlModel model = excelHelper.GetUploadExcelXMLConfig(xmlFilePath, id);
           
            this.SetConnectionDB();

            EVNImporterServices services = new EVNImporterServices( DBConnection);
            this.LrsConnection.OpenDBConnect();
            List<Dictionary< string,object>> data =  services.GetStoreData(model);

            MemoryStream memoryStream = excelHelper.ExportExcel(model, data, ref excelFileName);            
            this.LrsConnection.CloseDBConnect();
            return File(memoryStream.ToArray(), "application/vnd.ms-excel", excelFileName + ".xlsx");

        }


        [HttpPost]
        public ActionResult ExportExcelInventory(FileUploadModel blockData)
        {
            int month = blockData.currentMonth;
            int year = blockData.currentyear;
            string id = blockData.PermissionId.ToString();

            string xmlFilePath = this.CONFIG_FOLDER + "\\excel_export_format.xml";

            BI_Project.Helpers.ExcelHelper excelHelper = new ExcelHelper();

            string excelFileName = "";

            ExcelXmlModel model = excelHelper.GetUploadExcelXMLConfig(xmlFilePath, id);
         

            this.SetConnectionDB();

            EVNImporterServices services = new EVNImporterServices( DBConnection);
            this.LrsConnection.OpenDBConnect();
            List<Dictionary<string, object>> data = services.GetStoreDataInventory(model, month, year);

            MemoryStream memoryStream = excelHelper.ExportExcel(model, data, ref excelFileName);

            this.LrsConnection.CloseDBConnect();            

            return File(memoryStream.ToArray(), "application/vnd.ms-excel", excelFileName + ".xlsx");

        }


        [HttpGet]
        public ActionResult ExportExcelWithConfigReport(string id)
        {
            string xmlFilePath = this.CONFIG_FOLDER + "\\excel_export_format.xml";

            BI_Project.Helpers.ExcelHelper excelHelper = new ExcelHelper();

            string excelFileName = "";

            ExcelXmlModel model = excelHelper.GetUploadExcelXMLConfig(xmlFilePath, id);
            

            this.SetConnectionDB();

            EVNImporterServices services = new EVNImporterServices( DBConnection);
            this.LrsConnection.OpenDBConnect();
            List<Dictionary<string, object>> data = services.GetExcelConfigData(model);

            MemoryStream memoryStream = excelHelper.ExportExcel(model, data, ref excelFileName);
            

            this.LrsConnection.CloseDBConnect();
            return File(memoryStream.ToArray(), "application/vnd.ms-excel", excelFileName + ".xlsx");
        }

        [HttpPost]
        public async Task<FileResult> ExportPLExcel(FileUploadModel uploadModel)
        {           
            
            var memory = new MemoryStream();
            string xmlFilePath = this.CONFIG_FOLDER + "\\excel_export_format.xml";
            string id = uploadModel.PermissionId.ToString();

            ExcelHelper excelHelper = new ExcelHelper();
            ExcelXmlModel model = excelHelper.GetUploadExcelXMLConfig(xmlFilePath,id);

            try
            {
                /**************************************EXPORT EXCEL OFFLINE***************************************/
                if (Convert.ToBoolean(uploadModel.ExportOff) == true)
                {
                    //Get last time write file
                    string path = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_EXPORT_FOLDER) + model.ExcelXmlCommon.exportDirectory;

                    string fileName = excelHelper.GetNewFile(path);
                    if (fileName == null) throw new Exception();
                    string filePath = path + "\\" + fileName;

                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                }
                /**************************************EXPORT EXCEL ONLINE***************************************/
                else
                {
                    this.SetConnectionDB();
                    EVNImporterServices services = new EVNImporterServices(this.DBConnection);
                    this.LrsConnection.OpenDBConnect();
                    string filePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_EXPORT_FOLDER) + model.ExcelXmlCommon.exportDirectory + "//" + model.ExcelXmlCommon.ExcelFileName + "_" + APIStringHelper.GenerateId() + ".xlsx";
                    memory = services.ExportExcelWithConfigReport(id, filePath, model);
                    this.LrsConnection.CloseDBConnect();
                }
            }
            catch(Exception ex)
            {
                this.ERRORS = ex.ToString();
            }            
            return File(memory.ToArray(), "application/vnd.ms-excel", model.ExcelXmlCommon.ExcelFileName + ".xlsx");
        }

    }
}