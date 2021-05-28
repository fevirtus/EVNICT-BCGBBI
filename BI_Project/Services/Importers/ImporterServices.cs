using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.EntityModels;
using BI_Project.Helpers.Security;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json.Linq;

using System.Data.OleDb;

namespace BI_Project.Services.Importers
{
    public class ImporterServices:DBBaseService
    {
        DBConnection _dbStagingConnection;
        public string ERROR_USER { set; get; }
        public ImporterServices(DBConnection connection):base(connection)
        {


        }

        public ImporterServices(DBConnection connection,string dbStagingConnectionString):base(connection,dbStagingConnectionString)
        {
            _dbStagingConnection = new DBConnection(dbStagingConnectionString);
            this.DBConnection = connection;
        }
        public string GetOleDbConnectionString(string filePath)
        {
            return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";
        }


        //public List<MappingExcelDB> GetColumnList(string xmlFilePath, ref string uploadFolder, ref string tablename, ref int startRow, ref string sheetActive,
        //ref string helpDocumentPath, ref string note, ref string fileNativeName)
        //{
        //    List<MappingExcelDB> output = new List<MappingExcelDB>();
        //    JObject jObject = BI_Project.Helpers.Utility.JTokenHelper.GetXML2Jobject(xmlFilePath);

        //    uploadFolder = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.UploadedDirectory");
        //    tablename = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.database.tablename");
        //    string strstartRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.@started_row");
        //    startRow = Int32.Parse(strstartRow);
        //    sheetActive = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.#text");
        //    helpDocumentPath = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.HelpDocumentPath");
        //    note = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.Note.#cdata-section");
        //    fileNativeName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.FileNativeName");

        //    IEnumerable<JToken> columns = BI_Project.Helpers.Utility.JTokenHelper.GetList(jObject, "..mapping.column");


        //    int index = 0;

        //    foreach (JToken column in columns)
        //    {
        //        MappingExcelDB mappingExcel = new MappingExcelDB();
        //        mappingExcel.ColumnName = column.SelectToken("..name").Value<string>();
        //        mappingExcel.ExcelColumn = index;
        //        mappingExcel.Datatype = column.SelectToken("..datatype").Value<string>();
        //        output.Add(mappingExcel);
        //        index++;
        //    }

        //    return output;

        //}

        //public int Import2Database(int userid, string excelFilePath, string tablename, int startRow, string sheetActive,
        //    string helpDocumentPath, string note, string fileNativeName, string navetiveFile, int uploadRoleId, List<MappingExcelDB> lstMapping, string FileUploadedName)
        //{
        //    int output = 0;
        //    int index = 0, columnIndex = 0;

        //    string oleConnString = GetOleDbConnectionString(excelFilePath);
        //    OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
        //    OleDbCommand oleReadCommand = new OleDbCommand();
        //    SqlTransaction transaction = null;
        //    try
        //    {


        //        //GET EXCEL DATA
        //        string sqlOle = "select * from [" + sheetActive + "$]";
        //        oleDbConnection.Open();
        //        oleReadCommand.Connection = oleDbConnection;
        //        oleReadCommand.CommandText = sqlOle;


        //        int idHistory = 0;
        //        //******************INSERT INTO HISTORY*********************************************************
        //        this.DBConnection.OpenDBConnect();
        //        this.InsertHistory(userid, uploadRoleId, helpDocumentPath, note, navetiveFile, FileUploadedName, ref idHistory);
        //        string sqlInsertDW = "insert into " + tablename + "(";
        //        string sqlInsertDWValues = " values (";

        //        bool isCommplateGenerateSQL = false;

        //        _dbStagingConnection.OpenDBConnect();
        //        transaction = _dbStagingConnection.SQLDBConnect.BeginTransaction();
        //        _dbStagingConnection.command.Transaction = transaction;
        //        using (OleDbDataReader dataReader = oleReadCommand.ExecuteReader())
        //        {
        //            while (dataReader.Read())
        //            {
        //                index++;
        //                if (index < startRow) continue;
        //                this._dbStagingConnection.command.Parameters.Clear();
        //                columnIndex = 0;

        //                int countNullColumn = 0;
        //                foreach (MappingExcelDB column in lstMapping)
        //                {
        //                    columnIndex++;
        //                    column.Value = dataReader[column.ExcelColumn];
        //                    if (!isCommplateGenerateSQL)
        //                    {
        //                        sqlInsertDW += column.ColumnName + ",";
        //                        sqlInsertDWValues += "@" + column.ColumnName + ",";
        //                    }
        //                    //if (column.Value == null)
        //                    //    throw new Exception("Value in the column " + column.ColumnName + ", row " + index.ToString() + "  is null ");

        //                    try
        //                    {
        //                        if (column.Value.ToString().Trim() == "")
        //                        {
        //                            countNullColumn++;
        //                        }
        //                    }
        //                    catch (Exception)
        //                    {
        //                        countNullColumn++;
        //                    }
        //                    this._dbStagingConnection.command.Parameters.Add(new SqlParameter("@" + column.ColumnName, column.Value ?? (object)DBNull.Value));
        //                }
        //                if (!isCommplateGenerateSQL)
        //                {
        //                    sqlInsertDW = sqlInsertDW.Trim(',') + ")";
        //                    sqlInsertDWValues = sqlInsertDWValues.Trim(',') + ")";
        //                    isCommplateGenerateSQL = true;
        //                }

        //                //*****************************INSERT INTO DATABASE******************************************************
        //                //if (countNullColumn == lstMapping.Count) throw new Exception("The row is null");
        //                //this._dbStagingConnection.command.CommandText = sqlInsertDW + sqlInsertDWValues;
        //                //this._dbStagingConnection.command.CommandType = CommandType.Text;
        //                //this._dbStagingConnection.command.ExecuteNonQuery();
        //                if (countNullColumn == lstMapping.Count) break;
        //                else
        //                {
        //                    this._dbStagingConnection.command.CommandText = sqlInsertDW + sqlInsertDWValues;
        //                    this._dbStagingConnection.command.CommandType = CommandType.Text;
        //                    this._dbStagingConnection.command.ExecuteNonQuery();
        //                }                       


        //            }

        //        }
        //        transaction.Commit();
        //        //******************UPDATE ENDDATE HISTORY***************************************************************
        //        this.UpdateHistory(idHistory, index);

        //    }
        //    catch (Exception ex)
        //    {
        //        if (null != transaction) transaction.Rollback();
        //        this.ERROR = "ERROR in line=" + index.ToString() + ", column=" + columnIndex + ":::::" + ex.ToString();
        //        this.ERROR_USER = "line=" + index.ToString() + ", column=" + columnIndex;
        //    }

        //    finally
        //    {
        //        this.DBConnection.CloseDBConnect();
        //        oleReadCommand.Dispose();
        //        oleDbConnection.Close();
        //        oleDbConnection.Dispose();
        //        _dbStagingConnection.CloseDBConnect();
        //    }
        //    return output;
        //}


        /*****************************************************GET DATA DOANH SO*********************************/
        public List<MappingExcelDB> GetColumnList(string xmlFilePath, ref string uploadFolder, ref string tablename, ref int startRow, ref string sheetActive,
            ref string helpDocumentPath, ref string note, ref string fileNativeName, ref int number_row, ref int number_cell, ref List<string> lstCellName)
        {
            List<MappingExcelDB> output = new List<MappingExcelDB>();
            JObject jObject = BI_Project.Helpers.Utility.JTokenHelper.GetXML2Jobject(xmlFilePath);

            uploadFolder = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.UploadedDirectory");
            tablename = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.database.tablename");
            string strstartRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.@started_row");
            startRow = Int32.Parse(strstartRow);
            string strnumberRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell.@number_row");
            number_row = Int32.Parse(strnumberRow);

            string strnumberCell = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell.@number_cell");
            number_cell = Int32.Parse(strnumberCell);
            sheetActive = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.#text");
            helpDocumentPath = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.HelpDocumentPath");
            note = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.Note.#cdata-section");
            fileNativeName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.FileNativeName");

            //get list cell name
            string cellName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell_name");
            string[] lstName = null;
            if (cellName != null && cellName != "")
            {
                lstName = cellName.Split(',');
                for (int i = 0; i < lstName.Length; i++)
                {
                    lstCellName.Add(lstName[i].ToString());
                }
            }

            IEnumerable<JToken> columns = BI_Project.Helpers.Utility.JTokenHelper.GetList(jObject, "..mapping.column");


            int index = 0;

            foreach (JToken column in columns)
            {
                MappingExcelDB mappingExcel = new MappingExcelDB();
                mappingExcel.ColumnName = column.SelectToken("..name").Value<string>();
                mappingExcel.ExcelColumn = index;
                mappingExcel.Datatype = column.SelectToken("..datatype").Value<string>();
                output.Add(mappingExcel);
                index++;
            }

            return output;

        }
       


        public int Import2Database(int userid, string excelFilePath, string tablename, int startRow, string sheetActive,
            string helpDocumentPath, string note, string fileNativeName, string navetiveFile, int uploadRoleId, List<MappingExcelDB> lstMapping,
            string FileUploadedName, int number_row, int number_cell, List<string> lstCellName)
        {
            List<int> data = GetDataInLine(excelFilePath, sheetActive, number_cell, fileNativeName, number_row);
            int output = 0;
            int index = 0, columnIndex = 0;

            Dictionary<string, int> line = new Dictionary<string, int>();
            if (data != null)
            {
                for (int i = 0; i < lstCellName.Count; i++)
                {
                    line.Add(lstCellName[i], data[i]);
                }
            }


            string oleConnString = GetOleDbConnectionString(excelFilePath);
            OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
            OleDbCommand oleReadCommand = new OleDbCommand();
            SqlTransaction transaction = null;
            try
            {


                //GET EXCEL DATA
                string sqlOle = "select * from [" + sheetActive + "$]";
                oleDbConnection.Open();
                oleReadCommand.Connection = oleDbConnection;
                oleReadCommand.CommandText = sqlOle;


                int idHistory = 0;
                //******************INSERT INTO HISTORY*********************************************************
                this.DBConnection.OpenDBConnect();
                this.InsertHistory(userid, uploadRoleId, helpDocumentPath, note, navetiveFile, FileUploadedName, ref idHistory);
                string sqlInsertDW = "insert into " + tablename + "(";
                string sqlInsertDWValues = " values (";

                bool isCommplateGenerateSQL = false;

                _dbStagingConnection.OpenDBConnect();
                transaction = _dbStagingConnection.SQLDBConnect.BeginTransaction();
                _dbStagingConnection.command.Transaction = transaction;
                using (OleDbDataReader dataReader = oleReadCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        index++;
                        if (index < startRow) continue;
                        //if (index > lstMapping.Count) continue;
                        this._dbStagingConnection.command.Parameters.Clear();
                        columnIndex = 0;

                        int countNullColumn = 0;
                        foreach (MappingExcelDB column in lstMapping)
                        {
                            columnIndex++;
                            column.Value = dataReader[column.ExcelColumn];
                            if (!isCommplateGenerateSQL)
                            {
                                sqlInsertDW += column.ColumnName + ",";
                                sqlInsertDWValues += "@" + column.ColumnName + ",";
                            }
                            //if (column.Value == null)
                            //    throw new Exception("Value in the column " + column.ColumnName + ", row " + index.ToString() + "  is null ");

                            try
                            {
                                if (column.Value.ToString().Trim() == "")
                                {
                                    countNullColumn++;
                                }
                            }
                            catch (Exception)
                            {
                                countNullColumn++;
                            }
                            this._dbStagingConnection.command.Parameters.Add(new SqlParameter("@" + column.ColumnName, column.Value ?? (object)DBNull.Value));
                        }

                        if (line != null)
                        {
                            foreach (var cell in line)
                            {
                                if (!isCommplateGenerateSQL)
                                {
                                    sqlInsertDW += cell.Key + ",";
                                    sqlInsertDWValues += "@" + cell.Key + ",";
                                }
                                this._dbStagingConnection.command.Parameters.Add(new SqlParameter("@" + cell.Key, cell.Value));
                            }
                        }

                        if (!isCommplateGenerateSQL)
                        {
                            sqlInsertDW = sqlInsertDW.Trim(',') + ")";
                            sqlInsertDWValues = sqlInsertDWValues.Trim(',') + ")";
                            isCommplateGenerateSQL = true;
                        }

                        //*****************************INSERT INTO DATABASE******************************************************
                        //if (countNullColumn == lstMapping.Count) throw new Exception("The row is null");
                        //this._dbStagingConnection.command.CommandText = sqlInsertDW + sqlInsertDWValues;
                        //this._dbStagingConnection.command.CommandType = CommandType.Text;
                        //this._dbStagingConnection.command.ExecuteNonQuery();
                        if (countNullColumn == lstMapping.Count) break;                        
                        else
                        {
                            this._dbStagingConnection.command.CommandText = sqlInsertDW + sqlInsertDWValues;
                            this._dbStagingConnection.command.CommandType = CommandType.Text;
                            this._dbStagingConnection.command.ExecuteNonQuery();
                        }


                    }

                }
                transaction.Commit();
                //******************UPDATE ENDDATE HISTORY***************************************************************
                this.UpdateHistory(idHistory, index);

            }
            catch (Exception ex)
            {
                if (null != transaction) transaction.Rollback();
                this.ERROR = "ERROR in line=" + index.ToString() + ", column=" + columnIndex + ":::::" + ex.ToString();
                this.ERROR_USER = "line=" + index.ToString() + ", column=" + columnIndex;
            }

            finally
            {
                this.DBConnection.CloseDBConnect();
                oleReadCommand.Dispose();
                oleDbConnection.Close();
                oleDbConnection.Dispose();
                _dbStagingConnection.CloseDBConnect();
            }
            return output;
        }

        public List<int> GetDataInLine(string excelFilePath, string sheetActive, int number_cell, string FileUploadedName, int number_row)
        {
            List<int> output = new List<int>();
            int index = 0, columnIndex = 0;

            string oleConnString = GetOleDbConnectionString(excelFilePath);
            OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
            OleDbCommand oleReadCommand = new OleDbCommand();
            SqlTransaction transaction = null;
            try
            {
                
                //GET EXCEL DATA
                string sqlOle = "select * from [" + sheetActive + "$]";
                oleDbConnection.Open();
                oleReadCommand.Connection = oleDbConnection;
                oleReadCommand.CommandText = sqlOle;


                using (OleDbDataReader dataReader = oleReadCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        index++;
                        if (index < number_row) continue;                     
                        columnIndex = 1;

                        for(int i = 0; i < number_cell; i++)
                        {
                            MappingExcelDB column = new MappingExcelDB();
                            column.Value = dataReader[column.ExcelColumn + columnIndex];
                            output.Add(Int32.Parse(column.Value.ToString()));
                            columnIndex += 2;
                        }
                        break;
                    }

                }               
              
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }

            finally
            {
                this.DBConnection.CloseDBConnect();
                oleReadCommand.Dispose();
                oleDbConnection.Close();
                oleDbConnection.Dispose();               
            }
            return output;
        }


        public int InsertHistory(int userId, int uploadRoleId, string helpDocPath, string note, string navetiveFile,string FileUploadedName, ref int id)
        {
            int output = 0;
            Dictionary<string, object> dicInputParas = new Dictionary<string, object>();
            Dictionary<string, object> dicOutputParas = new Dictionary<string, object>();

            dicInputParas.Add("UserId", userId);
            dicInputParas.Add("UploadRoleId", uploadRoleId);
            dicInputParas.Add("HelpDocumentPath", helpDocPath);
            dicInputParas.Add("Note", note);
            dicInputParas.Add("FileNativeName", navetiveFile);
            dicInputParas.Add("FileUploadedName", FileUploadedName);
            dicOutputParas.Add("Id", id);

            output = this.DBConnection.ExecSPNonQuery("SP_UPLOAD_HISTORY_INSERT", dicInputParas, ref dicOutputParas, false);

            id = (int)dicOutputParas["Id"];

            return output;
        }

        public int UpdateHistory(int id,int numberInsertedRow)
        {
            int output = 0;
            Dictionary<string, object> dicInputParas = new Dictionary<string, object>();
            Dictionary<string, object> dicOutputParas = new Dictionary<string, object>();
            dicInputParas.Add("ID",id);
            dicInputParas.Add("NumberInsertedRow", numberInsertedRow);
            output = this.DBConnection.ExecSPNonQuery("SP_UPLOAD_HISTORY_UPDATE", dicInputParas,ref dicOutputParas, false);
            return output;
        }


        public List<EntityUploadHistoryModel> GetHistoryList(int currentpage, int noRecordPerPage,string permissionID, ref int noPages, ref int noRecord,int month, int year)
        {

           // DBConnection.OpenDBConnect();
            //STEP1:  ***************************************************************/
            Dictionary<string, object> dicParas = new Dictionary<string, object>();
            Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
            List<EntityUploadHistoryModel> output = new List<EntityUploadHistoryModel>();
            int intpermissionID = 0;
            try
            {
                try
                {
                    intpermissionID = Int32.Parse(permissionID);
                }
                catch(Exception)
                {
                    intpermissionID = 0;
                }
                dicParas.Add("NO_RECORD_PERPAGE", noRecordPerPage);
                dicParas.Add("CURRENT_PAGE", currentpage);
                dicParas.Add("PERMISSION_ID", intpermissionID);
                dicParas.Add("YEAR", year);
                dicParas.Add("MONTH", month);

                dicParaOuts.Add("NO_PAGES", noPages);
                dicParaOuts.Add("NO_RECORDS", noRecord);
                DataSet ds = this.DBConnection.ExecSelectSP("SP_UPLOAD_HISTORY_PAGING", dicParas, ref dicParaOuts, true);

                if (this.DBConnection.ERROR != null) throw new Exception(this.DBConnection.ERROR);

                noPages = (int)dicParaOuts["NO_PAGES"];
                noRecord = (int)dicParaOuts["NO_RECORDS"];

                DataTable table = ds.Tables[0];
                foreach(DataRow dr in table.Rows)
                {
                    EntityUploadHistoryModel model = (EntityUploadHistoryModel)dr;

                    output.Add(model);
                }
            }
            catch(Exception ex)
            {
                this.ERROR = ex.ToString();
            }

            finally
            {
                this.DBConnection.CloseDBConnect();
            }
            return output;
        }
    }
}