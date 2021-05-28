using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using OfficeOpenXml;
using Newtonsoft.Json.Linq;


namespace BI_Project.Helpers
{
    public class ExcelXMLPara
    {
        public string Name { set; get; }
        public string Title { set; get; }
        public int Row { set; get; }
        public int Column { set; get; }

        public int Colspan { set; get; }
    }

    public class ExcelXMLColumn
    {
        public string Title { set; get; }
        public string Name { set; get; }
        public string DataType { set; get; }
        public bool IsGetData { set; get; }

        public int ColumnWidth { set; get; }
        public ExcelXMLColumn()
        {

        }

        public ExcelXMLColumn(string title, string name, string datatype, string isGetData)
        {
            this.Title = title;
            this.Name = name;
            this.DataType = datatype;
            if (isGetData == "false") this.IsGetData = false;
            else this.IsGetData = true;
        }
    }

    public class ExcelXmlCommon
    {
        public string SheetName { set; get; }
        public string ExcelFileName { set; get; }
        public int StartRow { set; get; }

        public string ReportName { set; get; }

        public string exportDirectory { get; set; }
    }

    public class ExcelXmlModel
    {
        public ExcelXmlCommon ExcelXmlCommon { set; get; }
        public List< ExcelXMLPara> LstParas { set; get; }
        public List<ExcelXMLColumn> LstColumn { set; get; }

        public JObject TemplateContents { set; get; }
        public ExcelXmlModel ()
        {
            LstColumn = new List<ExcelXMLColumn>();
            LstParas = new List<ExcelXMLPara>();
        }
    }
    public class ExcelHelper
    {
        public void writeColumn(ExcelWorksheet worksheet,int row, int column, object value)
        {
            //ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[workSheetName];
            worksheet.Cells[row, column].Value = value;
            
        }

        public void writePara(ExcelWorksheet worksheet, ExcelXMLPara para, bool isBold)
        {
            writeHeaderColumn(worksheet, para.Row, para.Column, para.Colspan, para.Title,isBold,0);
        }
        public void writeHeaderColumn(ExcelWorksheet worksheet,int row, int column, int colspan, object value,bool isBold,int colwidth)
        {
            worksheet.Cells[row, column].Value = value;
            worksheet.Cells[row, column, row, column + colspan - 1].Merge = true;

            worksheet.Cells[row, column, row, column + colspan - 1].Style.Font.Bold = isBold;
            worksheet.Cells[row, column, row, column + colspan - 1].Style.Font.Italic = isBold;
            if (colwidth > 0)
            {
                worksheet.Column(column).AutoFit(colwidth);
            }
        }

        public MemoryStream ExportExcel(ExcelXmlModel model, IEnumerable<object> data, ref string fileName)
        {
            var p = new ExcelPackage();
            fileName = model.ExcelXmlCommon.ExcelFileName;

            //WRITE PARAMETERS TO EXCEL FILE
            ExcelWorksheet ws = p.Workbook.Worksheets.Add(model.ExcelXmlCommon.SheetName);
            foreach(ExcelXMLPara para in model.LstParas)
            {
                writePara(ws, para, true);
            }
            //WRITE HEADER OF DATA RANGE
            int indexColumn = 0;
            int indexDataRow = model.ExcelXmlCommon.StartRow;
            foreach (ExcelXMLColumn column in model.LstColumn)
            {
                indexColumn++;
                writeHeaderColumn(ws, indexDataRow, indexColumn,1, column.Title, true,column.ColumnWidth);
            }

            foreach (Dictionary<string, object> row in data)
            {
                indexDataRow++;
                indexColumn = 0;
                foreach (ExcelXMLColumn column in model.LstColumn)
                {
                    indexColumn++;
                    if(column.IsGetData)
                    {
                        writeHeaderColumn(ws, indexDataRow, indexColumn, 1, row[column.Name], false,column.ColumnWidth);
                        //ws.Column(indexColumn).BestFit = true;
                        //ws.Column(indexColumn).AutoFit(column.ColumnWidth);
                    }
                    
                }
            }
            //ws.Cells[1, 1, indexDataRow, indexColumn].AutoFitColumns();

            //ws.Column(2).AutoFit(150);
            
            return new MemoryStream(p.GetAsByteArray());
        }




        public ExcelXmlModel GetUploadExcelXMLConfig(string xmlFilePath, string menuid)
        {
            ExcelXmlModel output = new ExcelXmlModel();

            string excel_menu = "..excel_" + menuid;
            JObject jObject = BI_Project.Helpers.Utility.JTokenHelper.GetXML2Jobject(xmlFilePath);

            output.TemplateContents = BI_Project.Helpers.Utility.JTokenHelper.GetActiveJObject(jObject,excel_menu);
            //Get Common Info
            output.ExcelXmlCommon = new ExcelXmlCommon();
            output.ExcelXmlCommon.ExcelFileName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject,
                                                        excel_menu+ ".common.filename");

            output.ExcelXmlCommon.SheetName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject,
                                                        excel_menu + ".common.sheet");

            output.ExcelXmlCommon.StartRow = int.Parse( BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject,
                                                        excel_menu + ".common.datarowstart"));

            output.ExcelXmlCommon.ReportName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject,
                                                        excel_menu + ".common.reportname");

            output.ExcelXmlCommon.exportDirectory = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject,
                                                        excel_menu + ".common.exportDirectory");
            //GET PARAMETERS
            IEnumerable<JToken> paras = BI_Project.Helpers.Utility.JTokenHelper.GetList(jObject, excel_menu + "..paras.para");
            int index = 0;
            foreach (JToken para in paras)
            {
                //XMLParaModel 
                ExcelXMLPara paraXMLModel = new ExcelXMLPara();
                paraXMLModel.Name = para.SelectToken("..name").Value<string>();
                paraXMLModel.Row = para.SelectToken("..row").Value<Int32>();
                paraXMLModel.Column = para.SelectToken("..column").Value<Int32>();
                paraXMLModel.Colspan = para.SelectToken("..colspan").Value<Int32>();
                paraXMLModel.Title = para.SelectToken("..title").Value<string>();
                
                output.LstParas.Add(paraXMLModel);
            }

            //GET COLUMNS CONFIG
            IEnumerable<JToken> columns = BI_Project.Helpers.Utility.JTokenHelper.GetList(jObject, excel_menu + "..columns.column");

            foreach(JToken column in columns)
            {
                ExcelXMLColumn columnXML = new ExcelXMLColumn();
                columnXML.Name = column.SelectToken("..name").Value<string>();
                columnXML.Title = column.SelectToken("..title").Value<string>();
                columnXML.DataType = column.SelectToken("..datatype").Value<string>();
                columnXML.IsGetData = column.SelectToken("..isgetdata").Value<bool>();
                columnXML.ColumnWidth = column.SelectToken("..width").Value<int>();
                output.LstColumn.Add(columnXML);
                
            }
            return output;
        }


        public MemoryStream Export2Excel (string xmlFilePath, IEnumerable<object> data,string menuid, ref string fileName)
        {
            ExcelXmlModel model = GetUploadExcelXMLConfig(xmlFilePath, menuid);
            return ExportExcel(model, data, ref fileName);
        }

        /// <summary>
        /// used to get additional element, that not be clared ExcelXmlModel properties
        /// </summary>
        /// <param name="model"></param>
        /// <param name="elementPath"></param>
        /// <returns></returns>
        public object GetElementFromJObject(ExcelXmlModel model, string elementPath)
        {
            object output = null;
            output = model.TemplateContents.SelectToken(".." + elementPath).Value<object>();
            return output;
        }
                
        public string GetNewFile(string pathFile)
        {
            string output = null;
            var directory = new DirectoryInfo(pathFile);
            if(directory.GetFiles().Count() > 0)
                output = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First().ToString();
            return output;
        }

    }
}