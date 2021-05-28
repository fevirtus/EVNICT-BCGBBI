using BI_Project.Helpers;
using BI_Project.Helpers.Utility;
using BI_Project.Models.EntityModels;
using BI_Project.Models.UI;
using BI_Project.Services.Importers;
using BI_SUN.Services.SetDefaultPage;
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
    public class EVNCategoryController : BaseController
    {
        // GET: EVNImportExcel
        public string EXCEL_UPLOAD_FOLDER { set; get; }
        public string EXCEL_EXPORT_FOLDER { get; set; }


        public EVNCategoryController() : base()
        {
            this.EXCEL_UPLOAD_FOLDER = WebConfigurationManager.AppSettings["EXCEL_UPLOAD_FOLDER"];
            this.EXCEL_EXPORT_FOLDER = WebConfigurationManager.AppSettings["EXCEL_EXPORT_FOLDER"];
        }

        [HttpGet]
        //[CheckUserMenus]
        public ActionResult ProfileList(string id)
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
            ViewData["pagename"] = "Danh mục";
            ViewData["action_block"] = "Category/Profiles";
            this.SetCommonData();
            this.GetLanguage();

            //DECLARE PAGE MODEL
            BI_Project.Models.UI.PageModel pageModel = new Models.UI.PageModel("upload_excel");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            pageModel.H1Title = "Danh mục ngành nghề NCPT";
            pageModel.Title = "Danh mục ngành nghề";
            ViewData["page_model"] = pageModel;


            BlockLangExcelUpload blockLang = new BlockLangExcelUpload();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_upload_excel", this.LANGUAGE_OBJECT, blockLang);
            BlockCategoryModel blockData = new BlockCategoryModel();
            EVNCategoryServices services = new EVNCategoryServices(DBConnection);
            //ImporterServices _services = new ImporterServices(DBConnection);
            int userid = 0;
            try
            {
                userid = (int)Session[this.SESSION_NAME_USERID];
            }
            catch (Exception)
            {
                userid = 0;
            }                
            blockData.ListHistory = services.GetProfileList(userid, uiModel.CurrentPage, uiModel.PerPage, vID, ref noPages, ref noRecords);
            blockData.NumberPages = noPages;
            blockData.CurrentPage = uiModel.CurrentPage;
            blockModel.DataModel = blockData;
            ViewData["BlockData"] = blockModel;

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
            //return View("~/Views/Category/ProfileList.cshtml");
        }



        [HttpGet]
        //[CheckUserMenus]
        public ActionResult CheckCMIS()
        {
            return CheckCMIS(Session["CodeIsAdmin"].ToString(), DateTime.Now.Month, DateTime.Now.Year, 0);
        }

        [HttpPost]
        //[CheckUserMenus]
        public ActionResult CheckCMIS(string MaDV, int Thang, int Nam, int LoaiBC)
        {
            ViewData["pagename"] = "Đối soát số liệu CMIS3.0";
            ViewData["action_block"] = "Category/CheckCMIS";
            this.SetCommonData();
            this.GetLanguage();

            //DECLARE PAGE MODEL
            BI_Project.Models.UI.PageModel pageModel = new Models.UI.PageModel("upload_excel");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            pageModel.H1Title = "Kiểm tra số liệu";
            pageModel.Title = "Danh mục ngành nghề";
            ViewData["page_model"] = pageModel;

            SetDefaultPageService rs = new SetDefaultPageService(DBConnection);
            ViewData["dsDonVi"] = rs.GetListDepartments(Session["CodeIsAdmin"].ToString(), 1);

            EntityTimKiemModel timkiem = new EntityTimKiemModel();
            timkiem.MA_DVIQLY = MaDV;
            timkiem.NAM = Nam;
            timkiem.THANG = Thang;
            timkiem.LUYKE = LoaiBC;
            ViewData["timkiem"] = timkiem;


            EVNCategoryServices services = new EVNCategoryServices(DBConnection);
            ViewData["LRS_RESUL"] = services.GetProfileResulList(MaDV, Thang, Nam, LoaiBC);

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
            //return View("~/Views/Category/ProfileList.cshtml");
        }


        [HttpPost]
        public ActionResult ProfileList(string id, BlockCategoryModel uiModel)
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

            ViewData["pagename"] = "Danh mục";
            ViewData["action_block"] = "Category/Profiles.cshtml";
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
            //BlockCategoryModel blockData = new BlockCategoryModel();
            uiModel.PermissionID = id;
            EVNCategoryServices services = new EVNCategoryServices(DBConnection);

            string xmlConfigFilePath = this.CONFIG_FOLDER + "\\excel_format_" + uiModel.PermissionID.ToString() + ".xml";
            uiModel.HelpDoc = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.HelpDocumentPath");

            int userid = (int)Session[this.SESSION_NAME_USERID];
            uiModel.ListHistory = services.GetProfileList(userid, uiModel.CurrentPage, uiModel.PerPage, id, ref noPages, ref noRecords);
            uiModel.NumberPages = noPages;
            uiModel.CurrentPage = uiModel.CurrentPage;
            blockModel.DataModel = uiModel;
            //blockModel.DataModel = ViewData["block_menu_left_data"];
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            ViewData["BlockData"] = blockModel;

            //return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
            return View("~/Views/Category/ProfileList.cshtml");
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
                EVNCategoryServices services = new EVNCategoryServices(DBConnection);
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
        public ActionResult ImportExcel(FileUploadModel model)
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
                EVNCategoryServices services = new EVNCategoryServices( DBConnection);
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
                    services.Import2Database(userid, excelFilePath, uploadRoleId, lstColumns, excelfilename);
                else
                    services.ImportTotalDatabase(userid, excelFilePath, uploadRoleId, lstColumns, excelfilename);

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

            EVNCategoryServices services = new EVNCategoryServices( DBConnection);
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

            EVNCategoryServices services = new EVNCategoryServices( DBConnection);
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

            EVNCategoryServices services = new EVNCategoryServices( DBConnection);
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
                    EVNCategoryServices services = new EVNCategoryServices(this.DBConnection);
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