using BI_Project.Services;
using BI_Project.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BI_Project.Services.BCGB
{
    public class BCGBServices : DBBaseService
    {
        public BCGBServices(DBConnection dBConnection) : base(dBConnection)
        {

        }

        public List<EntityReportBIModel> GetList(string DeptID, int? ReportRequirementId = null)
        {
            List<EntityReportBIModel> output = new List<EntityReportBIModel>();

            this.DBConnection.OpenDBConnect();
            //Write log
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
            try
            {
                this.DBConnection.command.CommandText = "SP_BCGB_GET_RPTID";
                this.DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@ReportRequirementId", (object)ReportRequirementId ?? DBNull.Value);
                DBConnection.command.Parameters.AddWithValue("@DeptID", (object)DeptID ?? DBNull.Value);


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityReportBIModel entity = new EntityReportBIModel();
                            if (!reader.IsDBNull(reader.GetOrdinal("Id")))
                                entity.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ReportName")))
                                entity.ReportName = reader.GetString(reader.GetOrdinal("ReportName"));
                            if (!reader.IsDBNull(reader.GetOrdinal("DepartmentCode")))
                                entity.DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ConfirmStatus")))
                                entity.ConfirmStatus = reader.GetBoolean(reader.GetOrdinal("ConfirmStatus"));
                            if (!reader.IsDBNull(reader.GetOrdinal("DataStatus")))
                                entity.DataStatus = reader.GetBoolean(reader.GetOrdinal("DataStatus"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ReportRequirementId")))
                                entity.ReportRequirementId = reader.GetInt32(reader.GetOrdinal("ReportRequirementId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                                entity.Description = reader.GetString(reader.GetOrdinal("Description"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ConfirmDate")))
                                entity.ConfirmDate = reader.GetDateTime(reader.GetOrdinal("ConfirmDate"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ReportPathFile")))
                                entity.ReportPathFile = reader.GetString(reader.GetOrdinal("ReportPathFile"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Created")))
                                entity.Created = reader.GetDateTime(reader.GetOrdinal("Created"));

                            output.Add(entity);
                        }
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
            }
            return ReportRequirementId == null ? output : output.ToList();
        }


        public List<EntityReportBIModel> GetListReportBI(int? ReportRequirementId = null)
        {
            List<EntityReportBIModel> output = new List<EntityReportBIModel>();

            this.DBConnection.OpenDBConnect();
            //Write log
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
            try
            {
                this.DBConnection.command.CommandText = "SP_BCGB_LISTREPORTBI_RPTID";
                this.DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@ReportRequirementId", (object)ReportRequirementId ?? DBNull.Value);


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityReportBIModel entity = new EntityReportBIModel();
                            entity.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ReportName")))
                                entity.ReportName = reader.GetString(reader.GetOrdinal("ReportName"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ReportCode")))
                                entity.ReportCode = reader.GetString(reader.GetOrdinal("ReportCode"));
                            if (!reader.IsDBNull(reader.GetOrdinal("UrlLink")))
                                entity.UrlLink = reader.GetString(reader.GetOrdinal("UrlLink"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                                entity.Description = reader.GetString(reader.GetOrdinal("Description"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Status")))
                                entity.Status = reader.GetBoolean(reader.GetOrdinal("Status"));
                            if (!reader.IsDBNull(reader.GetOrdinal("DepartmentCode")))
                                entity.DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode"));
                            if (!reader.IsDBNull(reader.GetOrdinal("AreaId")))
                                entity.AreaId = reader.GetInt32(reader.GetOrdinal("AreaId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Cycle")))
                                entity.Cycle = reader.GetString(reader.GetOrdinal("Cycle"));
                            if (!reader.IsDBNull(reader.GetOrdinal("DepartmentId")))
                                entity.DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("IsDelete")))
                                entity.IsDelete = reader.GetBoolean(reader.GetOrdinal("IsDelete"));
                            output.Add(entity);
                        }
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
            }
            return ReportRequirementId == null ? output : output.ToList();
        }

        public BlockDataBCGBDetailModel GetEntityById(int id)
        {
            BlockDataBCGBDetailModel output = new BlockDataBCGBDetailModel();
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("ID", id);
                DataSet dataSet = DBConnection.ExecSelectSP("SP_BCGB_GET_BY_ID", dicParas, ref dicParaOuts, true);
                //**********************TABLE: ROLE***************************************
                DataTable table1 = dataSet.Tables[0];
                foreach (DataRow row in table1.Rows)
                {
                    output.Id = (int)row["Id"];
                    output.Title = row["Title"].ToString();
                    output.ConfirmExpired = (DateTime)row["ConfirmExpired"];
                    output.Description = (string)row["Description"];
                    output.Status = (int)row["Status"];
                    output.Created = (DateTime)row["Created"];
                    output.CreatorId = (int)row["CreatorId"];
                    output.LastUpdated = (DateTime)row["LastUpdated"];
                    output.Modifier = (int)row["Modifier"];
                }

            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }
            return output;
        }

        public EntityReportConfirmModel getRCById(int ReportRequireId, int ReporBI_Id)
        {
            EntityReportConfirmModel output = new EntityReportConfirmModel();
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

                this.DBConnection.command.CommandText = "SP_BCGB_GET_REPORT_CONFIRM_BYID";
                this.DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@ReportRequireId", (object)ReportRequireId ?? DBNull.Value);
                DBConnection.command.Parameters.AddWithValue("@ReporBIId", (object)ReporBI_Id ?? DBNull.Value);

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("Id")))
                                output.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ReportId")))
                                output.ReportId = reader.GetInt32(reader.GetOrdinal("ReportId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ReportRequirementId")))
                                output.ReportRequirementId = reader.GetInt32(reader.GetOrdinal("ReportRequirementId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                                output.Description = reader.GetString(reader.GetOrdinal("Description"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ConfirmStatus")))
                                output.ConfirmStatus = reader.GetBoolean(reader.GetOrdinal("ConfirmStatus"));
                            if (!reader.IsDBNull(reader.GetOrdinal("DataStatus")))
                                output.DataStatus = reader.GetBoolean(reader.GetOrdinal("DataStatus"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Created")))
                                output.Created = reader.GetDateTime(reader.GetOrdinal("Created"));
                            if (!reader.IsDBNull(reader.GetOrdinal("CreatorId")))
                                output.CreatorId = reader.GetInt32(reader.GetOrdinal("CreatorId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ReportPathFile")))
                                output.ReportPathFile = reader.GetString(reader.GetOrdinal("ReportPathFile"));
                            if (!reader.IsDBNull(reader.GetOrdinal("DepartmentId")))
                                output.DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Cycle")))
                                output.Cycle = reader.GetInt32(reader.GetOrdinal("Cycle"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Year")))
                                output.Year = reader.GetInt32(reader.GetOrdinal("Year"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ConfirmDate")))
                                output.ConfirmDate = reader.GetDateTime(reader.GetOrdinal("ConfirmDate"));
                        }
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
            }
            return output;
        }

        public List<EntityReportConfirmHistoryModel> GetListReportConfirmHistory(int ReportRequirementId , int ReporBIId)
        {
            List<EntityReportConfirmHistoryModel> output = new List<EntityReportConfirmHistoryModel>();

            this.DBConnection.OpenDBConnect();
            //Write log
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
            try
            {
                this.DBConnection.command.CommandText = "SP_BCGB_LIST_REPORT_CONFIRM_HISTORY_RPTID";
                this.DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@ReportRequirementId", (object)ReportRequirementId ?? DBNull.Value);
                DBConnection.command.Parameters.AddWithValue("@ReportId", (object)ReporBIId ?? DBNull.Value);


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityReportConfirmHistoryModel entity = new EntityReportConfirmHistoryModel();
                            entity.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ReportId")))
                                entity.ReportId = reader.GetInt32(reader.GetOrdinal("ReportId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ReportRequirementId")))
                                entity.ReportRequirementId = reader.GetInt32(reader.GetOrdinal("ReportRequirementId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                                entity.Description = reader.GetString(reader.GetOrdinal("Description"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ConfirmStatus")))
                                entity.ConfirmStatus = reader.GetBoolean(reader.GetOrdinal("ConfirmStatus"));
                            if (!reader.IsDBNull(reader.GetOrdinal("DataStatus")))
                                entity.DataStatus = reader.GetBoolean(reader.GetOrdinal("DataStatus"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Created")))
                                entity.Created = reader.GetDateTime(reader.GetOrdinal("Created"));
                            if (!reader.IsDBNull(reader.GetOrdinal("CreatorId")))
                                entity.CreatorId = reader.GetInt32(reader.GetOrdinal("CreatorId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ReportPathFile")))
                                entity.ReportPathFile = reader.GetString(reader.GetOrdinal("ReportPathFile"));
                            if (!reader.IsDBNull(reader.GetOrdinal("DepartmentId")))
                                entity.DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Cycle")))
                                entity.Cycle = reader.GetInt32(reader.GetOrdinal("Cycle"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Year")))
                                entity.Year = reader.GetInt32(reader.GetOrdinal("Year"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ConfirmDate")))
                                entity.ConfirmDate = reader.GetDateTime(reader.GetOrdinal("ConfirmDate"));
                            output.Add(entity);
                        }
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
            }
            return ReportRequirementId == null ? output : output.ToList();
        }

        public EntityReportRequirementModel FindById(int id)
        {
            EntityReportRequirementModel output = new EntityReportRequirementModel();
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("ID", id);
                DataSet dataSet = DBConnection.ExecSelectSP("SP_ReportRequirement_GET_BY_ID", dicParas, ref dicParaOuts, true);
                //**********************TABLE: ROLE***************************************
                DataTable table1 = dataSet.Tables[0];
                foreach (DataRow row in table1.Rows)
                {
                    output.Id = (int)row["Id"];
                    output.Title = row["Title"].ToString();
                    output.ConfirmExpired = (DateTime)row["ConfirmExpired"];
                    output.Description = (string)row["Description"];
                    output.Status = (int)row["Status"];
                    output.Created = (DateTime)row["Created"];
                    output.CreatorId = (int)row["CreatorId"];
                    output.LastUpdated = (DateTime)row["LastUpdated"];
                    output.Modifier = (int)row["Modifier"];
                }

            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }
            return output;
        }

        public int UpdateReport(EntityReportConfirmModel Model)
        {
            int output = 0;
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/

                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();

                if (Model.ReportId != null || Model.ReportRequirementId != null)
                {
                    dicParas.Add("Description", Model.Description);
                    dicParas.Add("ReportRequirementId", Model.ReportRequirementId);
                    dicParas.Add("DataStatus", Model.DataStatus);
                    dicParas.Add("CreatorId", Model.CreatorId);
                    dicParas.Add("ReportPathFile", Model.ReportPathFile);
                    dicParas.Add("DepartmentId", Model.DepartmentId);
                    dicParas.Add("ReportId", Model.ReportId);
                    dicParas.Add("ConfirmDate", Model.ConfirmDate);
                    output = DBConnection.ExecSPNonQuery("SP_GB_ReportConfirm_Update", dicParas, ref dicParaOuts, true);                    
                }
            }
            catch (Exception ex)
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