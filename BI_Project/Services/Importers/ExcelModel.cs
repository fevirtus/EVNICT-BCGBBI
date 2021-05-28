using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Services.Importers
{
    public class ExcelModel
    {
        public ExcelModel()
        {
            this.DBLstColumns = new List<MappingExcelDB>();
            this.XmlParas = new Dictionary<string, XMLParaModel>();
        }
        public List<MappingExcelDB> DBLstColumns
        {
            set;get;
        }
        public Dictionary<string,XMLParaModel> XmlParas
        {
            set;get;
        }

        public string FolderHelpDocumentPath
        {
            set;get;
        }

        public string FolderFileNativeName { set; get; }
        public string FolderUploadedDirectory { set; get; }

        public string LangNote { set; get; }

        public string DBTableName { set; get; }

        public string ExcelSheetName { set; get; }

        public int ExcelStartRow { set; get; }
    }
}