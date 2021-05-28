using BI_Project.Models.EntityModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using OracleInternal.SqlAndPlsqlParser;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using BI_Project.Helpers;
using BI_Project.Helpers.Utility;
using System.IO;
using OfficeOpenXml;
using System.Data.SqlClient;
using bicen.Models.EntityModels;
using System.Net;
using Newtonsoft.Json;

namespace BI_Project.Services.Importers
{
    public class EVNCategoryServices : DBBaseService
    {
        //ConnectOracleDB _dbDwhConnection;
        public string ERROR_USER { set; get; }
        public EVNCategoryServices(DBConnection connection) : base(connection)
        {
        }

        public string GetOleDbConnectionString(string filePath)
        {
            return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";
        }

        public ExcelModel GetXMLConfig(string xmlFilePath)
        {
            ExcelModel output = new ExcelModel();
            JObject jObject = BI_Project.Helpers.Utility.JTokenHelper.GetXML2Jobject(xmlFilePath);

            output.FolderUploadedDirectory = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.UploadedDirectory");
            output.DBTableName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.database.tablename");
            string strstartRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.@started_row");
            output.ExcelStartRow = Int32.Parse(strstartRow);
            string strnumberRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell.@number_row");
            //number_row = Int32.Parse(strnumberRow);

            string strnumberCell = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell.@number_cell");
            //number_cell = Int32.Parse(strnumberCell);
            output.ExcelSheetName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.#text");
            output.FolderHelpDocumentPath = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.HelpDocumentPath");
            output.LangNote = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.Note.#cdata-section");
            output.FolderFileNativeName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.FileNativeName");

            //LAY CAC COT TUONG UNG
            IEnumerable<JToken> columns = BI_Project.Helpers.Utility.JTokenHelper.GetList(jObject, "..mapping.column");
            int index = 0;
            foreach (JToken column in columns)
            {
                MappingExcelDB mappingExcel = new MappingExcelDB();
                mappingExcel.ColumnName = column.SelectToken("..name").Value<string>();
                mappingExcel.ExcelColumn = index;
                mappingExcel.Datatype = column.SelectToken("..datatype").Value<string>();
                output.DBLstColumns.Add(mappingExcel);
                index++;
            }

            //LAY CAC THAM SO PARAMETERS
            IEnumerable<JToken> paras = BI_Project.Helpers.Utility.JTokenHelper.GetList(jObject, "..paras.para");
            index = 0;
            foreach (JToken para in paras)
            {
                //XMLParaModel 
                XMLParaModel paraXMLModel = new XMLParaModel();
                paraXMLModel.Name = para.SelectToken("..name").Value<string>();
                paraXMLModel.Row = para.SelectToken("..row").Value<Int32>();
                //paraXMLModel.Column = para.SelectToken("..column").Value<Int32>();
                paraXMLModel.DataType = para.SelectToken("..datatype").Value<string>();
                paraXMLModel.Active = para.SelectToken("..active").Value<string>();
                paraXMLModel.Value = para.SelectToken("..value").Value<string>();

                output.XmlParas.Add(paraXMLModel.Name, paraXMLModel);
            }

            return output;
        }
        public List<MappingExcelDB> GetColumnList(string xmlFilePath, ref string uploadFolder, ref string tablename, ref int startRow, ref string sheetActive,
            ref string helpDocumentPath, ref string note, ref string fileNativeName) //, ref int number_row, ref int number_cell, ref List<string> lstCellName
        {
            List<MappingExcelDB> output = new List<MappingExcelDB>();
            JObject jObject = BI_Project.Helpers.Utility.JTokenHelper.GetXML2Jobject(xmlFilePath);

            uploadFolder = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.UploadedDirectory");
            tablename = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.database.tablename");
            string strstartRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.@started_row");
            startRow = Int32.Parse(strstartRow);
            string strnumberRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell.@number_row");
            //number_row = Int32.Parse(strnumberRow);

            string strnumberCell = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell.@number_cell");
            //number_cell = Int32.Parse(strnumberCell);
            sheetActive = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.#text");
            helpDocumentPath = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.HelpDocumentPath");
            note = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.Note.#cdata-section");
            fileNativeName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.FileNativeName");

            //get list cell name
            //string cellName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell_name");
            //string[] lstName = null;
            //if (cellName != null && cellName != "")
            //{
            //    lstName = cellName.Split(',');
            //    for (int i = 0; i < lstName.Length; i++)
            //    {
            //        lstCellName.Add(lstName[i].ToString());
            //    }
            //}

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



        public int ImportToDatabase(int currentYear, int userid, string excelFilePath, string tablename, int startRow, string sheetActive,
            string helpDocumentPath, string note, string fileNativeName, string navetiveFile, int uploadRoleId, List<MappingExcelDB> lstMapping,
            string FileUploadedName) //, int number_row, int number_cell, List<string> lstCellName
        {
            //List<int> data = GetDataInLine(excelFilePath, sheetActive, number_cell, fileNativeName, number_row);
            int output = 0;
            int index = 0, columnIndex = 0;
            Char charRange = ':';
            int Year = 0;
            int Month = 0;
            //Dictionary<string, int> line = new Dictionary<string, int>();
            //if (data != null)
            //{
            //    for (int i = 0; i < lstCellName.Count; i++)
            //    {
            //        line.Add(lstCellName[i], data[i]);
            //    }
            //}


            string oleConnString = GetOleDbConnectionString(excelFilePath);
            OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
            OleDbCommand oleReadCommand = new OleDbCommand();
            OracleTransaction transaction = null;
            try
            {


                //GET EXCEL DATA
                string sqlOle = "select * from [" + sheetActive + "$]";
                oleDbConnection.Open();
                oleReadCommand.Connection = oleDbConnection;
                oleReadCommand.CommandText = sqlOle;


                int idHistory = 0;
                //******************INSERT INTO HISTORY*********************************************************
                //this.ConnectOracleDB.OpenDBConnect();6
                this.InsertHistory(userid, uploadRoleId, helpDocumentPath, note, navetiveFile, FileUploadedName, ref idHistory);




                ConnectOracleDB.OpenDBConnect();
                transaction = ConnectOracleDB.OracleDBConnect.BeginTransaction();
                ConnectOracleDB.command.Transaction = transaction;
                using (OleDbDataReader dataReader = oleReadCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {

                        index++;
                        string sqlInsertDW = "insert into " + tablename + "(";
                        string sqlInsertDWValues = " values (";
                        string _sqlCheck = "Delete from  " + tablename + " WHERE  Year = " + Year;

                        bool isCommplateGenerateSQL = false;
                        if (index == 1)
                        {
                            string time = dataReader[0].ToString();
                            int startIndex = time.IndexOf(charRange);
                            if (time.ToString() == "")
                            {
                                throw new Exception("Time is null or not correct format.");
                            }
                            int endIndex = time.Length;
                            int length = endIndex - startIndex - 1;
                            int number = Int32.Parse(time.Substring(startIndex + 1, length));
                            if (number > 12) Year = number;
                            else
                                Month = number;


                            //Delete data before insert with condition year
                            //_sqlCheck = Year > 0 ? "Delete from  " + tablename + " WHERE  Year = " + Year :
                            //    "Delete from  " + tablename + " WHERE  MONTH = " + Month;                          
                            //this.ConnectOracleDB.command.CommandText = _sqlCheck;
                            //this.ConnectOracleDB.command.ExecuteNonQuery();
                        }


                        if (index < startRow) continue;

                        ConnectOracleDB.command.Parameters.Clear();
                        columnIndex = 0;

                        int countNullColumn = 0;
                        foreach (MappingExcelDB column in lstMapping)
                        {
                            //var test = dataReader.
                            columnIndex++;
                            column.Value = dataReader[column.ExcelColumn];
                            if (!isCommplateGenerateSQL)
                            {
                                sqlInsertDW += column.ColumnName + ",";
                                sqlInsertDWValues += ":" + column.ColumnName + ",";
                            }

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
                            ConnectOracleDB.command.Parameters.Add(new OracleParameter(":" + column.ColumnName, column.Value ?? (object)DBNull.Value));
                        }

                        //if (line != null)
                        //{
                        //    foreach (var cell in line)
                        //    {
                        //        if (!isCommplateGenerateSQL)
                        //        {
                        //            sqlInsertDW += cell.Key + ",";
                        //            sqlInsertDWValues += "@" + cell.Key + ",";
                        //        }
                        //        this._dbDwhConnection.command.Parameters.Add(new OracleParameter("@" + cell.Key, cell.Value));
                        //    }
                        //}

                        if (!isCommplateGenerateSQL)
                        {
                            sqlInsertDW = Year > 0 ? sqlInsertDW.Trim(',') + ",Year)" : sqlInsertDW.Trim(',') + ",Month)";
                            sqlInsertDWValues = Year > 0 ? sqlInsertDWValues.Trim(',') + ",:Year)" : sqlInsertDWValues.Trim(',') + ", :Month)";
                            if (Year > 0)
                                ConnectOracleDB.command.Parameters.Add(new OracleParameter(":Year", Year));
                            else
                                ConnectOracleDB.command.Parameters.Add(new OracleParameter(":Month", Month));
                            isCommplateGenerateSQL = true;
                        }

                        //*****************************INSERT INTO DATABASE******************************************************
                        if (countNullColumn == lstMapping.Count) break;
                        else
                        {
                            this.ConnectOracleDB.command.CommandText = sqlInsertDW + sqlInsertDWValues;
                            this.ConnectOracleDB.command.CommandType = CommandType.Text;
                            this.ConnectOracleDB.command.ExecuteNonQuery();
                            ConnectOracleDB.command.Parameters.Clear();
                        }
                    }
                }
                transaction.Commit();
                //******************UPDATE ENDDATE HISTORY***************************************************************
                //UpdateHistory(idHistory, index);

            }
            catch (Exception ex)
            {
                if (null != transaction) transaction.Rollback();
                this.ERROR = "ERROR in line=" + index.ToString() + ", column=" + columnIndex + ":::::" + ex.ToString();
                this.ERROR_USER = "line=" + index.ToString() + ", column=" + columnIndex;
            }

            finally
            {
                ConnectOracleDB.CloseDBConnect();
                oleReadCommand.Dispose();
                oleDbConnection.Close();

                ConnectOracleDB.CloseDBConnect();
            }
            return output;
        }

        public void Import2Database(int userid, string excelFilePath, int uploadRoleId, ExcelModel lstMapping, string excelFileName)
        {
            int index = 0;
            string strUserName = "AT";
            string strDepartmentCode = "P0";
            try
            {
                DataTable dt = new DataTable("Temp_ProfileResul");
                dt.Columns.Add(new DataColumn("STUDYID", typeof(string)));
                dt.Columns.Add(new DataColumn("EXPANSIONRESULTID", typeof(string)));
                dt.Columns.Add(new DataColumn("DT", typeof(string)));
                dt.Columns.Add(new DataColumn("TOTAL", typeof(string)));
                using (var rd = new StreamReader(excelFilePath))
                {
                    bool bf = true;
                    while (!rd.EndOfStream)
                    {
                        var splits = rd.ReadLine().Split(',');
                        if (bf)
                        {
                            bf = false;
                            continue;
                        }
                        else
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = splits[0];
                            dr[1] = splits[1];
                            dr[2] = splits[2];
                            dr[3] = splits[3];
                            dt.Rows.Add(dr);
                            index++;
                        }                        
                    }
                }


                /**************************************************INSERT HISTORY**************************************/
                this.DBConnection.OpenDBConnect();
                int idHistory = 0;
                this.InsertHistory(userid, uploadRoleId, lstMapping.FolderHelpDocumentPath, lstMapping.LangNote, lstMapping.FolderFileNativeName, excelFileName, ref idHistory);


                //lấy thông tin user                
                this.DBConnection.command.CommandText = "SELECT * FROM [dbo].[Users] u, dbo.Department d WHERE u.deptID = d.id AND UserId = " + userid.ToString();
                this.DBConnection.command.CommandType = CommandType.Text;
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            strUserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? "" : reader.GetString(reader.GetOrdinal("UserName"));
                            //output.DeptId = reader.GetInt32(reader.GetOrdinal("deptID"));
                            strDepartmentCode = reader.IsDBNull(reader.GetOrdinal("Code")) ? "" : reader.GetString(reader.GetOrdinal("Code"));
                        }
                    }
                }

                #region Đẩy dữ liệu
                SqlConnection destinationConnection = new SqlConnection(this.DBConnection.ConnectString);
                destinationConnection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = destinationConnection;
                cmd.CommandType = CommandType.Text;
                string strSQL = "";
                //xóa bảng tạm
                strSQL = "DELETE FROM Temp_ProfileResul";
                cmd.CommandText = strSQL;
                cmd.ExecuteNonQuery();
                //insert vào bảng tạm
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                {
                    bulkCopy.DestinationTableName = "Temp_ProfileResul";
                    bulkCopy.WriteToServer(dt);
                }                
                //chuyển dữ liệu từ bảng phụ qua bảng chính
                strSQL = "DELETE FROM Temp2_StatisticsResul";
                cmd.CommandText = strSQL;
                cmd.ExecuteNonQuery();
                strSQL = "INSERT INTO Temp2_StatisticsResul(DEPARTMENTID, PROFILEID, DT, TOTAL)";
                strSQL += " SELECT '" + strDepartmentCode + "', RTRIM(SUBSTRING(EXPANSIONRESULTID, 0, CHARINDEX(' -',EXPANSIONRESULTID))) [PROFILEID],";
                strSQL += " CONVERT(datetime, REPLACE(dt+':00', '24:00:00','23:59:59'),20) AS DT,";
                strSQL += " CAST(IIF(CHARINDEX('E',TOTAL)>0, SUBSTRING(TOTAL, 0, CHARINDEX('E',TOTAL)),TOTAL) AS DECIMAL(20,6)) ";
                strSQL += "FROM Temp_ProfileResul t";
                cmd.CommandText = strSQL;
                cmd.ExecuteNonQuery();
                strSQL = "DELETE c from LRS_StatisticsResul c WHERE EXISTS (SELECT * FROM Temp2_StatisticsResul where DEPARTMENTID=c.DEPARTMENTID and PROFILEID=c.PROFILEID AND DT=c.DT)";
                cmd.CommandText = strSQL;
                cmd.ExecuteNonQuery();
                strSQL = "INSERT INTO LRS_StatisticsResul(DEPARTMENTID,PROFILEID,DT,TOTAL, CREATEDATE, CREATEUSER)";
                strSQL += " SELECT DEPARTMENTID, PROFILEID, DT,TOTAL, GETDATE(),'" + strUserName + "' from Temp2_StatisticsResul";
                cmd.CommandText = strSQL;
                cmd.ExecuteNonQuery();
                destinationConnection.Close();
                #endregion

                UpdateHistory(idHistory, index);
                this.DBConnection.CloseDBConnect();
            }
            catch (Exception ex)
            {
                this.ERROR = "Lỗi khi nhận dữ liệu: "  + ex.Message; 
            }
            //return output;            
        }

        //import fie excel biểu đồ tổng hệ thống
        public void ImportTotalDatabase(int userid, string excelFilePath, int uploadRoleId, ExcelModel lstMapping, string excelFileName)
        {
            int index = 0;
            string strUserName = "AT";
            string strDepartmentCode = "P0";
            try
            {
                //GET EXCEL DATA
                string oleConnString = GetOleDbConnectionString(excelFilePath);
                string sqlOle = "select * from [" + lstMapping.ExcelSheetName + "$]";                
                var adapter = new OleDbDataAdapter(sqlOle, oleConnString);
                var ds = new DataSet();
                adapter.Fill(ds, "Temp2_StatisticsResul");
                //kiểm tra các cột đảm bảo chuẩn (2 cột là DT và TOTAL)
                DataTable dt = ds.Tables[0];
                DataColumn dc;
                dc = dt.Columns[0];
                if (dc.ColumnName != "DT")
                {
                    this.ERROR = "Lỗi file: Cột 1 phải là DT";
                    return;
                }
                if (dc.DataType != typeof(DateTime))
                {
                    this.ERROR = "Lỗi file: Cột 1 phải là kiểu ngày giờ";
                    return;
                }
                dc = dt.Columns[1];
                if (dc.ColumnName != "TOTAL")
                {
                    this.ERROR = "Lỗi file: Cột 2 phải là TOTAL";
                    return;
                }
                if (dc.DataType != typeof(decimal) && dc.DataType != typeof(double))
                {
                    this.ERROR = "Lỗi file: Cột 2 phải là kiểu số";
                    return;
                }
                
                /**************************************************INSERT HISTORY**************************************/
                this.DBConnection.OpenDBConnect();
                int idHistory = 0;
                this.InsertHistory(userid, uploadRoleId, lstMapping.FolderHelpDocumentPath, lstMapping.LangNote, lstMapping.FolderFileNativeName, excelFileName, ref idHistory);


                //lấy thông tin user                
                this.DBConnection.command.CommandText = "SELECT * FROM [dbo].[Users] u, dbo.Department d WHERE u.deptID = d.id AND UserId = " + userid.ToString();
                this.DBConnection.command.CommandType = CommandType.Text;
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            strUserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? "" : reader.GetString(reader.GetOrdinal("UserName"));
                            //output.DeptId = reader.GetInt32(reader.GetOrdinal("deptID"));
                            strDepartmentCode = reader.IsDBNull(reader.GetOrdinal("Code")) ? "" : reader.GetString(reader.GetOrdinal("Code"));
                        }
                    }
                }

                //thêm 2 cột                
                dc = new DataColumn("DEPARTMENTID", typeof(string));
                dc.DefaultValue = strDepartmentCode;
                dt.Columns.Add(dc);
                dc = new DataColumn("PROFILEID", typeof(string));
                dc.DefaultValue = "DV";
                dt.Columns.Add(dc);
                //order
                dt.Columns["DEPARTMENTID"].SetOrdinal(0);
                dt.Columns["PROFILEID"].SetOrdinal(1);
                dt.Columns["DT"].SetOrdinal(2);
                dt.Columns["TOTAL"].SetOrdinal(3);

                #region Đẩy dữ liệu
                SqlConnection destinationConnection = new SqlConnection(this.DBConnection.ConnectString);
                destinationConnection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = destinationConnection;
                cmd.CommandType = CommandType.Text;
                string strSQL = "";
                //xóa bảng tạm
                strSQL = "DELETE FROM Temp2_StatisticsResul";
                cmd.CommandText = strSQL;
                cmd.ExecuteNonQuery();
                //insert vào bảng tạm
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                {
                    bulkCopy.DestinationTableName = "Temp2_StatisticsResul";
                    bulkCopy.WriteToServer(dt);
                }
                //chuyển dữ liệu từ bảng phụ qua bảng chính                
                strSQL = "DELETE c from LRS_StatisticsResul c WHERE EXISTS (SELECT * FROM Temp2_StatisticsResul where DEPARTMENTID=c.DEPARTMENTID and PROFILEID=c.PROFILEID AND DT=c.DT)";
                cmd.CommandText = strSQL;
                cmd.ExecuteNonQuery();
                strSQL = "INSERT INTO LRS_StatisticsResul(DEPARTMENTID,PROFILEID,DT,TOTAL, CREATEDATE, CREATEUSER)";
                strSQL += " SELECT DEPARTMENTID, PROFILEID, DT,TOTAL, GETDATE(),'" + strUserName + "' from Temp2_StatisticsResul";
                cmd.CommandText = strSQL;
                cmd.ExecuteNonQuery();
                destinationConnection.Close();
                #endregion

                UpdateHistory(idHistory, index);
                this.DBConnection.CloseDBConnect();
            }
            catch (Exception ex)
            {
                this.ERROR = "Lỗi khi nhận dữ liệu: " + ex.Message;
            }
            //return output;            
        }

        public void ImportPLDatabase(int userid, string excelFilePath, int uploadRoleId, ExcelModel lstMapping, string excelFileName)
        {

            int index = 0, columnIndex = 0, count = 0; int year = 0;
            //int index = 0, columnIndex = 0, count = 0; int year = 0;




            string oleConnString = GetOleDbConnectionString(excelFilePath);
            OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
            OleDbCommand oleReadCommand = new OleDbCommand();
            OracleTransaction transaction = null;
            try
            {

                //int output = 0;

                //string sqlInserPara2tDWValues = null;
                //string sqlInsertPara2DW = null;

                //string oleConnString = GetOleDbConnectionString(excelFilePath);
                //OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
                //OleDbCommand oleReadCommand = new OleDbCommand();
                //OracleTransaction transaction = null;

                //GET EXCEL DATA
                string sqlOle = "select * from [" + lstMapping.ExcelSheetName + "$]";
                oleDbConnection.Open();
                oleReadCommand.Connection = oleDbConnection;
                oleReadCommand.CommandText = sqlOle;

                ConnectOracleDB.OpenDBConnect();
                transaction = ConnectOracleDB.OracleDBConnect.BeginTransaction();
                ConnectOracleDB.command.Transaction = transaction;
                ConnectOracleDB.command.Parameters.Clear();



                /**************************************************INSERT HISTORY**************************************/
                this.DBConnection.OpenDBConnect();
                int idHistory = 0;
                this.InsertHistory(userid, uploadRoleId, lstMapping.FolderHelpDocumentPath, lstMapping.LangNote, lstMapping.FolderFileNativeName, excelFileName, ref idHistory);

                this.DBConnection.CloseDBConnect();
                string deleteRecordQuery = "DELETE FROM " + lstMapping.DBTableName + " WHERE ";
                string sqlInsertDW = "insert into " + lstMapping.DBTableName + "(";
                string sqlInsertDWValues = " values (";



                bool isCommplateGenerateSQL = false;

                using (OleDbDataReader dataReader = oleReadCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        index++;
                        if (index == 1) continue;
                        if (index == 2)
                        {
                            var madviqly = dataReader[0].ToString().Substring(0, 2);
                            var _month = dataReader[1].ToString();
                            var _year = dataReader[2].ToString();
                            deleteRecordQuery += "ma_dviqly like '%" + madviqly + "%'" + " " + "AND " + "THANG_BC " + "= " + _month + " " + "AND " + "NAM_BC " + "= " + _year;
                            this.ConnectOracleDB.command.CommandText = deleteRecordQuery;
                            this.ConnectOracleDB.command.ExecuteNonQuery();
                        }
                        if (index >= lstMapping.ExcelStartRow)
                        {
                            columnIndex = 0;

                            int countNullColumn = 0;
                            foreach (MappingExcelDB column in lstMapping.DBLstColumns)
                            {
                                //var test = dataReader.
                                columnIndex++;
                                column.Value = dataReader[column.ExcelColumn];
                                if (!isCommplateGenerateSQL)
                                {
                                    sqlInsertDW += column.ColumnName + ",";
                                    sqlInsertDWValues += ":" + column.ColumnName + ",";
                                }

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
                                ConnectOracleDB.command.Parameters.Add(new OracleParameter(":" + column.ColumnName, column.Value ?? (object)DBNull.Value));
                            }


                            if (!isCommplateGenerateSQL)
                            {
                                sqlInsertDW = sqlInsertDW.Trim(',') + ")";
                                sqlInsertDWValues = sqlInsertDWValues.Trim(',') + ")";
                                isCommplateGenerateSQL = true;
                            }

                            //*****************************INSERT INTO DATABASE******************************************************
                            if (countNullColumn == lstMapping.DBLstColumns.Count()) break;
                            else
                            {
                                this.ConnectOracleDB.command.CommandText = sqlInsertDW + sqlInsertDWValues;
                                this.ConnectOracleDB.command.CommandType = CommandType.Text;
                                this.ConnectOracleDB.command.ExecuteNonQuery();
                                ConnectOracleDB.command.Parameters.Clear();
                            }
                        }

                    }

                }
                transaction.Commit();
                UpdateHistory(idHistory, index);

            }
            catch (Exception ex)
            {
                this.ERROR = "tại dòng số :  " + (index + 1).ToString() + ", Cột số :  " + (columnIndex) + "  " + ex.Message; ;
            }
            finally
            {
                ConnectOracleDB.CloseDBConnect();
                oleReadCommand.Dispose();
                oleDbConnection.Close();
                oleDbConnection.Dispose();
            }
            //return output;            
        }

        public List<int> GetDataInLine(string excelFilePath, string sheetActive, int number_cell, string FileUploadedName, int number_row)
        {
            List<int> output = new List<int>();
            int index = 0, columnIndex = 0;

            string oleConnString = GetOleDbConnectionString(excelFilePath);
            OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
            OleDbCommand oleReadCommand = new OleDbCommand();
            OracleTransaction transaction = null;
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

                        for (int i = 0; i < number_cell; i++)
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
                this.ConnectOracleDB.CloseDBConnect();
                oleReadCommand.Dispose();
                oleDbConnection.Close();
                oleDbConnection.Dispose();
            }
            return output;
        }


        public int InsertHistory(int userId, int uploadRoleId, string helpDocPath, string note, string navetiveFile, string FileUploadedName, ref int id)
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

        public int UpdateHistory(int id, int numberInsertedRow)
        {
            int output = 0;
            Dictionary<string, object> dicInputParas = new Dictionary<string, object>();
            Dictionary<string, object> dicOutputParas = new Dictionary<string, object>();
            dicInputParas.Add("ID", id);
            dicInputParas.Add("NumberInsertedRow", numberInsertedRow);
            output = this.DBConnection.ExecSPNonQuery("SP_UPLOAD_HISTORY_UPDATE", dicInputParas, ref dicOutputParas, false);
            return output;
        }


        public List<EntityProfilesModel> GetProfileList(int userid, int currentpage, int noRecordPerPage, string permissionID, ref int noPages, ref int noRecord)
        {

            // DBConnection.OpenDBConnect();
            //STEP1:  ***************************************************************/
            List<EntityProfilesModel> output = new List<EntityProfilesModel>();
            int intpermissionID = 0;
            try
            {
                try
                {
                    intpermissionID = Int32.Parse(permissionID);
                }
                catch (Exception)
                {
                    intpermissionID = 0;
                }
                this.DBConnection.OpenDBConnect();
                this.DBConnection.command.CommandText = "SELECT * FROM LRS_PROFILELIST";
                this.DBConnection.command.CommandType = CommandType.Text;
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityProfilesModel model = new EntityProfilesModel();
                            model.PROFILEID = reader.IsDBNull(reader.GetOrdinal("PROFILEID")) ? "" : reader.GetString(reader.GetOrdinal("PROFILEID"));
                            model.DESCR = reader.IsDBNull(reader.GetOrdinal("DESCR")) ? "" : reader.GetString(reader.GetOrdinal("DESCR"));
                            model.PARENT_PROFILEID = reader.IsDBNull(reader.GetOrdinal("PARENT_PROFILEID")) ? "" : reader.GetString(reader.GetOrdinal("PARENT_PROFILEID"));
                            model.PROFILE_TYPE = reader.IsDBNull(reader.GetOrdinal("PROFILE_TYPE")) ? "" : reader.GetString(reader.GetOrdinal("PROFILE_TYPE"));
                            model.DISPORDER = 0; // Convert.ToInt16(reader.IsDBNull(reader.GetOrdinal("DISPORDER")) ? "0" : reader.GetString(reader.GetOrdinal("DISPORDER")));
                            output.Add(model);
                        }
                    }
                }
                this.DBConnection.CloseDBConnect();
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            return output;
        }



        public List<LRS_RESUL> GetProfileResulList(string MaDV, int Thang, int Nam, int LoaiBC)
        {

            // DBConnection.OpenDBConnect();
            //STEP1:  ***************************************************************/
            List<LRS_RESUL> output = new List<LRS_RESUL>();
            List<ANHXA_LRS_CMIS> anhxaCMIS = new List<ANHXA_LRS_CMIS>();

            try
            {
                this.DBConnection.OpenDBConnect();
                string strSQL = "SELECT PROFILEID, SUM(TOTAL) AS TOTAL FROM [dbo].[LRS_TGIAN_BDIEN] ";                
                strSQL += " WHERE DEPARTMENTID='" + MaDV + "' AND THANG " + (LoaiBC==0 ? "=":"<=") + Thang.ToString() + " AND NAM=" + Nam.ToString() ;
                strSQL += " AND LEN(PROFILEID)=4  GROUP BY PROFILEID ORDER BY PROFILEID";
                this.DBConnection.command.CommandText = strSQL;
                this.DBConnection.command.CommandType = CommandType.Text;
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            LRS_RESUL model = new LRS_RESUL();
                            //public decimal SAN_LUONG { get; set; }
                            //public decimal SO_HDONG { get; set; }
                            model.PROFILEID = reader.IsDBNull(reader.GetOrdinal("PROFILEID")) ? "" : reader.GetString(reader.GetOrdinal("PROFILEID"));
                            model.Total = reader.IsDBNull(reader.GetOrdinal("TOTAL")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TOTAL"));
                            output.Add(model);
                        }
                    }
                }
                //lấy ánh xạ ngành nghề CMIS
                this.DBConnection.command.CommandText = "SELECT * FROM [dbo].[LRS_PROFILELIST_CMIS]";
                this.DBConnection.command.CommandType = CommandType.Text;
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ANHXA_LRS_CMIS model = new ANHXA_LRS_CMIS();
                            model.PROFILEID = reader.IsDBNull(reader.GetOrdinal("PROFILEID")) ? "" : reader.GetString(reader.GetOrdinal("PROFILEID"));
                            model.MA_NN = reader.IsDBNull(reader.GetOrdinal("MANN")) ? "" : reader.GetString(reader.GetOrdinal("MANN"));
                            anhxaCMIS.Add(model);
                        }
                    }
                }

                //lấy địa chỉ CMIS3 của TCT đã chọn
                this.DBConnection.command.CommandText = "SELECT * FROM [dbo].[LRS_Config] where DepartmentID='" + MaDV + "' and ConfigName='CMIS'";
                this.DBConnection.command.CommandType = CommandType.Text;
                string strDiaChiCMIS = "";
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            strDiaChiCMIS = reader.IsDBNull(reader.GetOrdinal("ConfigValue")) ? "" : reader.GetString(reader.GetOrdinal("ConfigValue"));
                        }
                    }
                }


                this.DBConnection.CloseDBConnect();

                //lấy thêm từ CMIS
                var listCMIS = getKD12_CMIS3(MaDV, Thang, Nam, LoaiBC, strDiaChiCMIS);
                foreach(var kqLRs in output)
                {
                    //ds nn CMIS
                    var listNN = anhxaCMIS.Where(o => o.PROFILEID == kqLRs.PROFILEID).Select(i => i.MA_NN).ToList();
                    //kqLRs.SO_HDONG 
                    kqLRs.SAN_LUONG = listCMIS.Where(o => listNN.Contains(o.MA_NN)).Select(i => i.SAN_LUONG).DefaultIfEmpty(0).Sum();
                    kqLRs.SAN_LUONG = kqLRs.SAN_LUONG / 1000;
                }

            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            return output;
        }


        public List<CMIS_KDDN12> getKD12_CMIS3(string MaDV, int Thang, int Nam, int LoaiBC, string strDiaChiCMIS)
        {
            //lấy thêm bên CMIS qua để có tình hình thực tế
            List<CMIS_KDDN12> dsCMIS = new List<CMIS_KDDN12>();

            #region Kết nối tới CMIS lấy dữ liệu sử dụng
            try
            {
                var strURL_CMISInterface = strDiaChiCMIS + "/ServiceBCaoThang-BCaoThang-context-root/resources/serviceBCaoThang/baoCaoKinhDoanh";
                dynamic product = new JObject();
                product.MA_DVIQLY = MaDV;
                product.LOAI_BCAO = LoaiBC;
                product.NAM = Nam;
                product.THANG = Thang;
                product.SO_HIEU = "KDDN 12";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL_CMISInterface);
                request.Method = "POST";

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(product.ToString());

                request.ContentLength = byteArray.Length;
                request.ContentType = @"application/json";

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                try
                {
                    WebResponse webResponse = request.GetResponse();
                    using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        var response = responseReader.ReadToEnd();
                        if (!response.Contains("<ERROR>")) //OK, có dữ liệu
                        {
                            response = response.Replace("{\"DATA\":", "");
                            response = response.Replace("}]}", "}]");
                            var dynObj = JsonConvert.DeserializeObject(response);
                            if (dynObj != null)
                            {
                                string arrKH = dynObj.ToString();
                                if (!arrKH.Contains("NODATA"))
                                {
                                    dsCMIS = (List<CMIS_KDDN12>)JsonConvert.DeserializeObject(arrKH, (typeof(List<CMIS_KDDN12>)));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex1)
                {
                }
            }
            catch (Exception ex2)
            {
            }
            #endregion
            return dsCMIS;
        }




        public List<Dictionary<string, object>> GetStoreData(ExcelXmlModel model)
        {
            List<Dictionary<string, object>> output = new List<Dictionary<string, object>>();
            try
            {
                string storeName = "shop_name";
                string proceName = null;
                //get store name to choose store procedure
                List<ExcelXMLColumn> lstColumn = new List<ExcelXMLColumn>();
                lstColumn = model.LstColumn;
                foreach (var column in lstColumn)
                {
                    if (column.Name.Contains(storeName)) { proceName = "pws_getmetadata_excel"; break; }
                    else proceName = "pws_getdata_excel";
                }

                output = GetStoreData(model, "report_name_para", "p_recordset", proceName);

            }
            catch (Exception ex)
            {

            }
            return output;
        }


        public List<Dictionary<string, object>> GetStoreData(ExcelXmlModel model, string paraInput, string paraOutput, string procedureName)
        {
            //Dictionary<string, object> dicParas = new Dictionary<string, object>();
            //Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
            List<Dictionary<string, object>> lst_store = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> lst_kpi = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> ouput = new List<Dictionary<string, object>>();



            try
            {

                this.ConnectOracleDB.command.Parameters.Clear();

                var op = new OracleParameter(paraInput, OracleDbType.Varchar2);
                op.Direction = ParameterDirection.Input;
                op.Value = model.ExcelXmlCommon.ReportName;
                this.ConnectOracleDB.command.Parameters.Add(op);

                this.ConnectOracleDB.command.Parameters.Add(paraOutput, OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                this.ConnectOracleDB.command.CommandText = procedureName;
                this.ConnectOracleDB.command.CommandType = CommandType.StoredProcedure;

                OracleDataReader objReader = this.ConnectOracleDB.command.ExecuteReader();
                while (objReader.Read())

                {
                    var columns = Enumerable.Range(0, objReader.FieldCount).Select(objReader.GetName).ToList();


                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    for (int i = 0; i < columns.Count; i++)
                    {
                        foreach (ExcelXMLColumn xmlColumn in model.LstColumn)
                        {
                            if (!xmlColumn.IsGetData) continue;

                            if (xmlColumn.Name != columns[i].ToLower()) continue;
                            if ((objReader[xmlColumn.Name] != null) && (xmlColumn.IsGetData))
                            {
                                dic.Add(xmlColumn.Name, objReader[xmlColumn.Name]);
                            }
                        }
                    }
                    ouput.Add(dic);
                    //objReader.NextResult();

                }

                objReader.Close();
                objReader.Dispose();

            }
            catch (Exception ex)
            {


            }
            finally
            {
                this.ConnectOracleDB.CloseDBConnect();
            }
            return ouput;
        }


        public List<Dictionary<string, object>> GetStoreDataInventory(ExcelXmlModel model, int month, int year)
        {
            //Dictionary<string, object> dicParas = new Dictionary<string, object>();
            //Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
            List<Dictionary<string, object>> lst_store = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> lst_kpi = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> ouput = new List<Dictionary<string, object>>();




            try
            {

                this.ConnectOracleDB.command.Parameters.Clear();

                var time = month.ToString() + "-" + year.ToString().Substring(2, 2);
                if (month < 10) time = '0' + time;


                var dateTime = new OracleParameter("dateTime", OracleDbType.Varchar2);

                dateTime.Direction = ParameterDirection.Input;
                dateTime.Value = time;
                this.ConnectOracleDB.command.Parameters.Add(dateTime);

                this.ConnectOracleDB.command.Parameters.Add("p_recordset", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                this.ConnectOracleDB.command.CommandText = "PWS_GETINVENTORYDATA_EXCEL";
                this.ConnectOracleDB.command.CommandType = CommandType.StoredProcedure;

                OracleDataReader objReader = this.ConnectOracleDB.command.ExecuteReader();
                while (objReader.Read())

                {
                    var columns = Enumerable.Range(0, objReader.FieldCount).Select(objReader.GetName).ToList();


                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    for (int i = 0; i < columns.Count; i++)
                    {

                        foreach (ExcelXMLColumn xmlColumn in model.LstColumn)
                        {
                            if (!xmlColumn.IsGetData) continue;

                            if (xmlColumn.Name.ToLower() != columns[i].ToLower()) continue;
                            if ((objReader[xmlColumn.Name] != null) && (xmlColumn.IsGetData))
                            {
                                dic.Add(xmlColumn.Name, objReader[xmlColumn.Name]);
                            }
                        }

                    }



                    ouput.Add(dic);
                    //objReader.NextResult();

                }

                objReader.Close();
                objReader.Dispose();

            }
            catch (Exception ex)
            {


            }
            finally
            {
                this.ConnectOracleDB.CloseDBConnect();
            }
            return ouput;
        }

        /// <summary>
        /// Get Store Name, Indicator
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetExcelConfigData(ExcelXmlModel model)
        {
            //Dictionary<string, object> dicParas = new Dictionary<string, object>();
            //Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
            List<Dictionary<string, object>> lst_store = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> lst_kpi = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> ouput = new List<Dictionary<string, object>>();




            try
            {
                string procedureName = null;
                if (model.ExcelXmlCommon.exportDirectory.Equals("PL_CongTy")) procedureName = "pws_getreporsetupcompany_excel";
                else procedureName = "pws_getreporsetupstore_excel";

                this.ConnectOracleDB.command.Parameters.Clear();

                var op = new OracleParameter("PK_SRPTNBR_para", OracleDbType.Varchar2);
                op.Direction = ParameterDirection.Input;
                op.Value = model.ExcelXmlCommon.ReportName;
                this.ConnectOracleDB.command.Parameters.Add(op);

                this.ConnectOracleDB.command.Parameters.Add("p_recordset", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                this.ConnectOracleDB.command.CommandText = procedureName;
                this.ConnectOracleDB.command.CommandType = CommandType.StoredProcedure;

                OracleDataReader objReader = this.ConnectOracleDB.command.ExecuteReader();
                while (objReader.Read())

                {
                    var columns = Enumerable.Range(0, objReader.FieldCount).Select(objReader.GetName).ToList();


                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    for (int i = 0; i < columns.Count; i++)
                    {
                        foreach (ExcelXMLColumn xmlColumn in model.LstColumn)
                        {
                            if (!xmlColumn.IsGetData) continue;

                            if (xmlColumn.Name != columns[i].ToLower()) continue;
                            if ((objReader[xmlColumn.Name] != null) && (xmlColumn.IsGetData))
                            {
                                dic.Add(xmlColumn.Name, objReader[xmlColumn.Name]);
                            }
                        }

                    }



                    ouput.Add(dic);
                    //objReader.NextResult();

                }

                objReader.Close();
                objReader.Dispose();

            }
            catch (Exception ex)
            {


            }
            finally
            {
                this.ConnectOracleDB.CloseDBConnect();
            }
            return ouput;
        }

        public MemoryStream ExportExcelWithConfigReport(string id, string filePath, ExcelXmlModel model)
        {

            List<Dictionary<string, object>> data = GetExcelConfigData(model);

            ExcelHelper excelHelper = new ExcelHelper();
            string excelFileName = null;

            MemoryStream memoryStream = excelHelper.ExportExcel(model, data, ref excelFileName);
            //save file excel exported          
            var package = new ExcelPackage(memoryStream);

            FileInfo Path = new FileInfo(filePath);
            package.SaveAs(Path);
            return memoryStream;
        }
    }
}