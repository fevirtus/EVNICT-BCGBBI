using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.EntityModels;
using BI_Project.Helpers.Security;
using System.Data.SqlClient;
using System.Data;

using BI_Project.Services.User;
using BI_Core.Helpers;
using bicen.Models.EntityModels;
using System.Globalization;

namespace BI_Project.Services.GBTask
{
    public class GBTaskServices : DBBaseService
    {

        public GBTaskServices(DBConnection dBConnection) : base(dBConnection)
        {

        }

        public List<EntityGBTaskModel> GetList(int? ReportRequirementId = null)
        {
            List<EntityGBTaskModel> output = new List<EntityGBTaskModel>();

            this.DBConnection.OpenDBConnect();
            //Write log
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
            try
            {
                this.DBConnection.command.CommandText = "SP_GB_TASK_GET_RPTID";
                this.DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@ReportRequirementId", (object)ReportRequirementId ?? DBNull.Value);


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityGBTaskModel entity = new EntityGBTaskModel();
                            entity.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            entity.Title = reader.GetString(reader.GetOrdinal("Title"));
                            entity.ReportRequirementId = reader.GetInt32(reader.GetOrdinal("ReportRequirementId"));
                            entity.Description = reader.GetString(reader.GetOrdinal("Description"));
                            entity.ResultFile = reader.GetString(reader.GetOrdinal("ResultFile"));
                            entity.Comment = reader.GetString(reader.GetOrdinal("Comment"));
                            entity.DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode"));
                            entity.Status = reader.GetInt32(reader.GetOrdinal("Status"));

                            if (reader.GetOrdinal("Deadline") != null)
                                entity.Deadline = reader.GetDateTime(reader.GetOrdinal("Deadline"));
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

        public List<EntityGBTaskModel> GetList(int? ReportRequirementId = null, string DepartmentCode = "")
        {
            List<EntityGBTaskModel> output = new List<EntityGBTaskModel>();

            this.DBConnection.OpenDBConnect();
            //Write log
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
            try
            {
                this.DBConnection.command.CommandText = "SP_GB_TASK_GET_DEPART_ID";
                this.DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@ReportRequirementId", (object)ReportRequirementId ?? DBNull.Value);
                DBConnection.command.Parameters.AddWithValue("@DepartmentCode", (object)DepartmentCode);

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityGBTaskModel entity = new EntityGBTaskModel();
                            entity.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            entity.Title = reader.GetString(reader.GetOrdinal("Title"));
                            entity.ReportRequirementId = reader.GetInt32(reader.GetOrdinal("ReportRequirementId"));
                            entity.Description = reader.GetString(reader.GetOrdinal("Description"));
                            entity.ResultFile = reader.GetString(reader.GetOrdinal("ResultFile"));
                            entity.Comment = reader.GetString(reader.GetOrdinal("Comment"));
                            entity.DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode"));
                            entity.Status = reader.GetInt32(reader.GetOrdinal("Status"));

                            if (reader.GetOrdinal("Deadline") != null)
                                entity.Deadline = reader.GetDateTime(reader.GetOrdinal("Deadline"));
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

        public int CreateEvaluate(EntityGBRPTEvaluateModel Model)
        {
            int output = 0;
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();




                if (Model.Description != null)
                {
                    dicParas.Add("CreatorId", Model.CreatorId);
                    dicParas.Add("DataCorrectNum", Model.DataCorrectNum);
                    dicParas.Add("DataInCorrectNum", Model.DataInCorrectNum);
                    dicParas.Add("DepartmentCode", Model.DepartmentCode);
                    dicParas.Add("DepartmentCode_Evaluate", Model.DepartmentCode_Evaluate);
                    dicParas.Add("Description", Model.Description);
                    dicParas.Add("ReportRequirementId", Model.ReportRequirementId);
                    dicParas.Add("ReportUnConfirmNum", Model.ReportUnConfirmNum);
                    dicParas.Add("TaskDoneNum", Model.TaskDoneNum);
                    dicParas.Add("TaskProcessNum", Model.TaskProcessNum);
                    dicParas.Add("Total", Model.Total);

                    if (Model.Id == 0)
                        //dicParas.Add("")
                        output = DBConnection.ExecSPNonQuery("SP_GB_TASK_Evaluate_Insert", dicParas, ref dicParaOuts, true);
                    else
                    {
                        dicParas.Add("ID", Model.Id);
                        output = DBConnection.ExecSPNonQuery("SP_GB_TASK_Evaluate_Update", dicParas, ref dicParaOuts, true);
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

        public List<EntityGBRPTEvaluateModel> GetListEvaluate(int? ReportRequirementId = null)
        {
            List<EntityGBRPTEvaluateModel> output = new List<EntityGBRPTEvaluateModel>();

            this.DBConnection.OpenDBConnect();
            //Write log
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
            try
            {
                this.DBConnection.command.CommandText = "SP_GB_TASK_SUMARY";
                this.DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@ReportRequirementId", (object)ReportRequirementId ?? DBNull.Value);


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityGBRPTEvaluateModel entity = new EntityGBRPTEvaluateModel();
                            entity.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            entity.ReportRequirementId = reader.GetInt32(reader.GetOrdinal("ReportRequirementId"));
                            entity.ReportUnConfirmNum = reader.GetInt32(reader.GetOrdinal("ReportUnConfirmNum"));
                            entity.TaskDoneNum = reader.GetInt32(reader.GetOrdinal("TaskDoneNum"));
                            entity.TaskProcessNum = reader.GetInt32(reader.GetOrdinal("TaskProcessNum"));
                            entity.Total = reader.GetInt32(reader.GetOrdinal("Total"));
                            entity.DataCorrectNum = reader.GetInt32(reader.GetOrdinal("DataCorrectNum"));
                            entity.DataInCorrectNum = reader.GetInt32(reader.GetOrdinal("DataInCorrectNum"));
                            entity.Description = reader.GetString(reader.GetOrdinal("Description"));
                            entity.DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode"));
                            entity.DepartmentCode_Evaluate = reader.GetString(reader.GetOrdinal("DepartmentCode_Evaluate"));

                            //entity.CreatorId = reader.GetInt32(reader.GetOrdinal("CreatorId"));
                            //entity.Created = reader.GetDateTime(reader.GetOrdinal("Created"));

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

        public int CreateTask(BlockDataGBTaskCreateModel Model)
        {
            int output = 0;
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();

                if (Model.Title != null)
                {
                    dicParas.Add("Title", Model.Title);
                    dicParas.Add("ReportRequirementId", Model.ReportRequirementId);
                    dicParas.Add("CreatorId", Model.CreatorId);
                    dicParas.Add("Deadline", Model.Deadline);
                    dicParas.Add("DepartmentCode", Model.DepartmentCode);
                    dicParas.Add("Description", Model.Description);

                    if (Model.Id == 0)
                        //dicParas.Add("")
                        output = DBConnection.ExecSPNonQuery("SP_GB_TASK_INSERT", dicParas, ref dicParaOuts, true);
                    else
                    {
                        dicParas.Add("ID", Model.Id);
                        output = DBConnection.ExecSPNonQuery("SP_GB_TASK_UPDATE", dicParas, ref dicParaOuts, true);
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

        public int ConfirmTask(BlockDataGBTaskCreateModel Model)
        {
            int output = 0;
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/
                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();

                //if (Model.Comment != null)
                //{

                    dicParas.Add("Status", Model.Status);
                    dicParas.Add("ResultFile", Model.ResultFile);
                    dicParas.Add("Comment", Model.Comment);
                    dicParas.Add("ID", Model.Id);
                    output = DBConnection.ExecSPNonQuery("SP_GB_TASK_CONFIRM", dicParas, ref dicParaOuts, true);
                //}

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

        public BlockDataGBTaskCreateModel GetEntityById(int id)
        {
            BlockDataGBTaskCreateModel output = new BlockDataGBTaskCreateModel();
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("ID", id);
                DataSet dataSet = DBConnection.ExecSelectSP("SP_GB_TASK_GET_BY_ID", dicParas, ref dicParaOuts, true);
                //**********************TABLE: ROLE***************************************
                DataTable table1 = dataSet.Tables[0];
                foreach (DataRow row in table1.Rows)
                {
                    output.Id = (int)row["Id"];
                    output.Title = row["Title"].ToString();
                    output.ReportRequirementId = (int)row["ReportRequirementId"];
                    output.Description = (string)row["Description"];
                    output.ResultFile = row["ResultFile"].ToString();
                    output.Comment = row["Comment"].ToString();
                    output.DepartmentCode = row["DepartmentCode"].ToString();
                    output.Deadline = (DateTime)row["Deadline"];
                    output.Status = (int)row["Status"];
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

        public int Delete(int id)
        {
            int output = 0;

            try
            {

                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/
                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("ID", id);

                //dicParas.Add("")
                output = DBConnection.ExecSPNonQuery("SP_GB_TASK_DELETE_BY_ID", dicParas, ref dicParaOuts, true);

                //STEP2:  ***************************************************************/

            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
                output = -1;
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }


            return output;
        }

        public EntityGBTaskModel FindById(int id)
        {
            EntityGBTaskModel output = new EntityGBTaskModel();
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("ID", id);
                DataSet dataSet = DBConnection.ExecSelectSP("SP_GB_TASK_GET_BY_ID", dicParas, ref dicParaOuts, true);
                //**********************TABLE: ROLE***************************************
                DataTable table1 = dataSet.Tables[0];
                foreach (DataRow row in table1.Rows)
                {
                    output.Id = (int)row["Id"];
                    output.Title = row["Title"].ToString();
                    output.ReportRequirementId = (int)row["ReportRequirementId"];
                    output.Description = (string)row["Description"];
                    output.ResultFile = row["ResultFile"].ToString();
                    output.Comment = row["Comment"].ToString();
                    output.DepartmentCode = row["DepartmentCode"].ToString();
                    output.Deadline = (DateTime)row["Deadline"];
                    output.Status = (int)row["Status"];
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