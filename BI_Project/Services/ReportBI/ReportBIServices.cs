using BI_Project.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BI_Project.Services.ReportBi
{
    public class ReportBIServices : DBBaseService
    {
        public ReportBIServices(DBConnection connection) : base(connection)
        {

        }
        public List<EntityReportBIModel> GetList()
        {
            List<EntityReportBIModel> output = new List<EntityReportBIModel>();

            this.DBConnection.OpenDBConnect();
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

            try
            {
                string sqlSelectDepart = " select * from GB_ReportBI where IsDelete=0 order by AreaId,DepartmentId,ReportCode";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = sqlSelectDepart;


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityReportBIModel entityRole = new EntityReportBIModel();
                            entityRole.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            entityRole.ReportCode = reader.IsDBNull(reader.GetOrdinal("ReportCode")) ? null : reader.GetString(reader.GetOrdinal("ReportCode"));
                            entityRole.ReportName = reader.IsDBNull(reader.GetOrdinal("ReportName")) ? null : reader.GetString(reader.GetOrdinal("ReportName"));
                            entityRole.UrlLink = reader.IsDBNull(reader.GetOrdinal("UrlLink")) ? null : reader.GetString(reader.GetOrdinal("UrlLink"));
                            entityRole.Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"));
                            entityRole.Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? false : reader.GetBoolean(reader.GetOrdinal("Status"));
                            entityRole.AreaId = reader.IsDBNull(reader.GetOrdinal("AreaId")) ? 0: reader.GetInt32(reader.GetOrdinal("AreaId"));
                            entityRole.DepartmentId = reader.IsDBNull(reader.GetOrdinal("DepartmentId")) ? 0 : reader.GetInt32(reader.GetOrdinal("DepartmentId"));
                            entityRole.DepartmentCode = reader.IsDBNull(reader.GetOrdinal("DepartmentCode")) ? null : reader.GetString(reader.GetOrdinal("DepartmentCode"));
                            entityRole.Cycle = reader.IsDBNull(reader.GetOrdinal("Cycle")) ? null : reader.GetString(reader.GetOrdinal("Cycle"));
                            if(!reader.IsDBNull(reader.GetOrdinal("Cycle")))
                            {
                                switch (entityRole.Cycle)
                                {
                                    case "WEEKLY":
                                        entityRole.CycleName = "TUẦN";
                                        break;
                                    case "MONTH":
                                        entityRole.CycleName = "THÁNG";
                                        break;
                                    case "QUARTER":
                                        entityRole.CycleName = "QUÝ";
                                        break;
                                    case "YEAR":
                                        entityRole.CycleName = "NĂM";
                                        break;
                                    default:
                                        entityRole.CycleName = "";
                                        break;
                                }
                            }
                            output.Add(entityRole);
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

        public EntityReportBIModel GetEntityById(int? id = null)
        {
            EntityReportBIModel output = new EntityReportBIModel();
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("REPORT_ID", id);
                DataSet dataSet = DBConnection.ExecSelectSP("SP_REPORT_BI_GET_BY_ID", dicParas, ref dicParaOuts, true);
                //**********************TABLE: ROLE***************************************
                DataTable table1 = dataSet.Tables[0];
                foreach (DataRow row in table1.Rows)
                {
                    output.Id = Int32.Parse(row["Id"].ToString());
                    output.ReportName = row.IsNull("ReportName") ? null:row["ReportName"].ToString();                  
                    output.ReportCode = row.IsNull("ReportCode") ?null: row["ReportCode"].ToString();
                    output.UrlLink = row.IsNull("UrlLink") ?null: row["UrlLink"].ToString();
                    output.Description = row.IsNull("Description") ?null: row["Description"].ToString();
                    output.Status = row.IsNull("Status") ? false:Boolean.Parse(row["Status"].ToString());
                    output.AreaId = row.IsNull("AreaId") ? 0: Int32.Parse(row["AreaId"].ToString());
                    output.DepartmentId = row.IsNull("DepartmentId") ? 0: Int32.Parse(row["DepartmentId"].ToString());
                    output.DepartmentCode = row.IsNull("DepartmentCode") ?null: row["DepartmentCode"].ToString();
                    output.Cycle = row.IsNull("Cycle") ? null:row["Cycle"].ToString();
                    output.DataAuto = row.IsNull("DataAuto") ? false : Boolean.Parse(row["DataAuto"].ToString());

                    if (!row.IsNull("Cycle"))
                    {
                        switch (output.Cycle)
                        {
                            case "WEEKLY":
                                output.CycleName = "TUẦN";
                                break;
                            case "MONTH":
                                output.CycleName = "THÁNG";
                                break;
                            case "QUARTER":
                                output.CycleName = "QUÝ";
                                break;
                            case "YEAR":
                                output.CycleName = "NĂM";
                                break;
                            default:
                                output.CycleName = "";
                                break;
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

        public int CapNhatReportBi(EntityReportBIModel requirementModel)
        {
            int output = 0;

            try
            {


                DBConnection.OpenDBConnect();
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();

                if (requirementModel.ReportCode != null)
                {
                    dicParas.Add("ReportCode", requirementModel.ReportCode);
                    dicParas.Add("ReportName", requirementModel.ReportName);
                    dicParas.Add("UrlLink", requirementModel.UrlLink);
                    dicParas.Add("Description", requirementModel.Description);
                    dicParas.Add("Status", requirementModel.Status);
                    dicParas.Add("AreaId", requirementModel.AreaId);
                    dicParas.Add("DepartmentCode", requirementModel.DepartmentCode);
                    dicParas.Add("DepartmentId", requirementModel.DepartmentId);
                    dicParas.Add("Cycle", requirementModel.Cycle);
                    dicParas.Add("DataAuto", requirementModel.DataAuto);

                    if (requirementModel.Id == 0)
                        //dicParas.Add("")
                        output = DBConnection.ExecSPNonQuery("SP_REPORT_BI_INSERT", dicParas, ref dicParaOuts, true);
                    else
                    {
                        dicParas.Add("REPORT_ID", requirementModel.Id);
                        output = DBConnection.ExecSPNonQuery("SP_REPORT_BI_UPDATE", dicParas, ref dicParaOuts, true);
                    }
                }               
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


        public int Delete(int id)
        {
            int output = 0;

            try
            {
                
                DBConnection.OpenDBConnect();
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("REPORT_ID", id);

                //dicParas.Add("")
                output = DBConnection.ExecSPNonQuery("SP_REPORT_BI_DELETE", dicParas, ref dicParaOuts, true);

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
    }
}