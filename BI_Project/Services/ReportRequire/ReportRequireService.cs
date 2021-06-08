using BI_Project.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BI_Project.Services.ReportRequire
{
    public class ReportRequireService : DBBaseService
    {
        public ReportRequireService(DBConnection connection) : base(connection)
        {
        }

        public int Insert(ReportRequireCreateModel model)
        {
            int output = 0;
            try
            {
                this.DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                                               
                dicParas.Add("Title", model.Title);
                dicParas.Add("ConfirmExpired", model.ConfirmExpired);
                dicParas.Add("Description", model.Description);
                dicParas.Add("CreatorId", model.CreatorId);
                dicParas.Add("StrReportId", model.strReportId);


                output = DBConnection.ExecSPNonQuery("SP_REQUIREMENT_INSERT", dicParas, ref dicParaOuts, true);

                if (DBConnection.ERROR != null)
                    throw new Exception(DBConnection.ERROR);

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

        public int Update(ReportRequireCreateModel model)
        {
            int output = 0;

            try
            {
                this.DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();

                dicParas.Add("REQUIREMENT_ID", model.Id);
                dicParas.Add("Title", model.Title);
                dicParas.Add("ConfirmExpired", model.ConfirmExpired);
                dicParas.Add("Description", model.Description);
                dicParas.Add("CreatorId", model.CreatorId);
                dicParas.Add("StrReportId", model.strReportId);

                output = DBConnection.ExecSPNonQuery("SP_REQUIREMENT_UPDATE", dicParas, ref dicParaOuts, true);

                if (DBConnection.ERROR != null)
                    throw new Exception(DBConnection.ERROR);
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
                output = -1;
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

                this.DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("REQUIREMENT_ID", id);

                //dicParas.Add("")
                output = DBConnection.ExecSPNonQuery("SP_REQUIREMENT_DELETE", dicParas, ref dicParaOuts, true);

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

        public List<ReportRequireModel> GetList(){
            List<ReportRequireModel> output = new List<ReportRequireModel>();
            this.DBConnection.OpenDBConnect();
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

            try
            {
                string sqlSelectDepart = "Select r.*, u.FullName  From GB_ReportRequirement r left join users u on r.CreatorId = u.UserId";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = sqlSelectDepart;


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int tt = 0;
                        while (reader.Read())
                        {
                            tt++;
                            ReportRequireModel report = new ReportRequireModel();
                            report.Stt = tt;
                            report.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            report.Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ConfirmExpired")))
                            {
                                report.ConfirmExpired = reader.GetDateTime(reader.GetOrdinal("ConfirmExpired"));

                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                            {
                                report.Description = reader.GetString(reader.GetOrdinal("Description"));

                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Status")))
                            {
                                report.Status = reader.GetInt32(reader.GetOrdinal("Status"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Created")))
                            {
                                report.Created = reader.GetDateTime(reader.GetOrdinal("Created"));
                            }                            
                            if (!reader.IsDBNull(reader.GetOrdinal("CreatorId")))
                            {
                                report.CreatorId = reader.GetInt32(reader.GetOrdinal("CreatorId"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("LastUpdated")))
                            {
                                report.LastUpdated = reader.GetDateTime(reader.GetOrdinal("LastUpdated"));
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("Modifier")))
                            {
                                report.Modifier = reader.GetInt32(reader.GetOrdinal("Modifier"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("FullName")))
                            {
                                report.FullName = reader.GetString(reader.GetOrdinal("FullName"));
                            }
                            //report.Modifier = reader.IsDBNull(reader.GetOrdinal("Modifier")) ? null : reader.GetInt32(reader.GetOrdinal("Modifier"));



                            output.Add(report);
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

        public List<ReportRequireModel> GetList2()
        {
            List<ReportRequireModel> output = new List<ReportRequireModel>();
            this.DBConnection.OpenDBConnect();
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

            try
            {
                string sqlSelectDepart = "Select r.*, u.FullName, "  +
                                         "   case  " +
                                         "       when status = 0 then N'Chưa gửi'"   +
                                         "       when status = 1 then N'Đã gửi' " +
                                         "       when status = 2 then N'Hoàn thành' " +
                                         "   end as TrangThai " +
                                         " From GB_ReportRequirement r left join users u on r.CreatorId = u.UserId " +
                                         " Where r.Status in (1,2) " + 
                                         " Order by r.Status asc, r.ConfirmExpired desc";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = sqlSelectDepart;


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int tt = 0;
                        while (reader.Read())
                        {
                            tt++;
                            ReportRequireModel report = new ReportRequireModel();
                            report.Stt = tt;
                            report.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            report.Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ConfirmExpired")))
                            {
                                report.ConfirmExpired = reader.GetDateTime(reader.GetOrdinal("ConfirmExpired"));

                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                            {
                                report.Description = reader.GetString(reader.GetOrdinal("Description"));

                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Status")))
                            {
                                report.Status = reader.GetInt32(reader.GetOrdinal("Status"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Created")))
                            {
                                report.Created = reader.GetDateTime(reader.GetOrdinal("Created"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("CreatorId")))
                            {
                                report.CreatorId = reader.GetInt32(reader.GetOrdinal("CreatorId"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("LastUpdated")))
                            {
                                report.LastUpdated = reader.GetDateTime(reader.GetOrdinal("LastUpdated"));
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("Modifier")))
                            {
                                report.Modifier = reader.GetInt32(reader.GetOrdinal("Modifier"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("FullName")))
                            {
                                report.FullName = reader.GetString(reader.GetOrdinal("FullName"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("TrangThai")))
                            {
                                report.TrangThai = reader.GetString(reader.GetOrdinal("TrangThai"));
                            }
                           

                            output.Add(report);
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

        public ReportRequireModel GetReportById(int reportId)
        {
            ReportRequireModel output = new ReportRequireModel();
            this.DBConnection.OpenDBConnect();
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

            try
            {
                string sqlSelectDepart = "Select " +
                                         "       sub.Id, sub.Title, sub.ConfirmExpired, sub.Description, sub.Status, sub.Created, sub.CreatorId, sub.LastUpdated, sub.Modifier, Replace(Replace(sub.ReportId, '<ReportId>', ''), '</ReportId>', ',') as strReportId " +
                                         "   FROM                                                " +
                                         "      (  " +
                                         "       Select " +
                                         "           Id, Title, ConfirmExpired, Description, Status, Created, CreatorId, LastUpdated, Modifier, " +
                                         "           (Select ReportId from GB_ReportConfirm where ReportRequirementId = " + reportId.ToString() + " FOR XML PATH('')) as ReportId " +
                                         "       From " +
                                         "           GB_ReportRequirement  where id = " + reportId.ToString() +
                                         "       )sub" ;
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = sqlSelectDepart;


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int tt = 0;
                        while (reader.Read())
                        {
                            tt++;
                            ReportRequireModel report = new ReportRequireModel();
                            report.Stt = tt;
                            report.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            report.Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title"));
                            if (!reader.IsDBNull(reader.GetOrdinal("ConfirmExpired")))
                            {
                                report.ConfirmExpired = reader.GetDateTime(reader.GetOrdinal("ConfirmExpired"));

                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                            {
                                report.Description = reader.GetString(reader.GetOrdinal("Description"));

                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Status")))
                            {
                                report.Status = reader.GetInt32(reader.GetOrdinal("Status"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Created")))
                            {
                                report.Created = reader.GetDateTime(reader.GetOrdinal("Created"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("CreatorId")))
                            {
                                report.CreatorId = reader.GetInt32(reader.GetOrdinal("CreatorId"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("LastUpdated")))
                            {
                                report.LastUpdated = reader.GetDateTime(reader.GetOrdinal("LastUpdated"));
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("Modifier")))
                            {
                                report.Modifier = reader.GetInt32(reader.GetOrdinal("Modifier"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("strReportId")))
                            {
                                report.strReportId = reader.GetString(reader.GetOrdinal("strReportId"));
                                report.strReportId = report.strReportId.Substring(0, report.strReportId.Length - 1);
                            }

                            output = report;
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



    }
}