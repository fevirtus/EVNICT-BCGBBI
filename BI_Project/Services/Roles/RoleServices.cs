using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.EntityModels;
using BI_Project.Helpers.Security;
using System.Data.SqlClient;

using System.Data;
namespace BI_Project.Services.Roles
{
    public class RoleServices : DBBaseService
    {
        private static string USP_GET_ROLES_BY_ORGID = "usp_Get_Roles_By_OrgId";
        public RoleServices(DBConnection dBConnection):base(dBConnection)
        {

        }

        public int Create(BlockDataRoleCreateModel model)
        {
            int output = 0;
            string strLstOfMenuIds = "";
            try
            {

                strLstOfMenuIds = model.StrAllowedMenus;
                DBConnection.OpenDBConnect();
                //STEP1:  ***************************************************************/
                

                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                if(model.Name != null && model.Description != null && strLstOfMenuIds != null)
                {
                    dicParas.Add("Name", model.Name);
                    //dicParas.Add("Description", model.Description);
                    dicParas.Add("Description", String.IsNullOrEmpty(model.Description) ? "" : model.Description);
                    dicParas.Add("LstOfMenuIds", strLstOfMenuIds);
                    dicParas.Add("LstReports", model.StrAllowedReportBI);
                    dicParas.Add("DepartId", model.DeptID);
                    if (model.RoleId == 0)
                        //dicParas.Add("")
                        output = DBConnection.ExecSPNonQuery("SP_ROLE_INSERT", dicParas, ref dicParaOuts, true);
                    else
                    {
                        dicParas.Add("RoleId", model.RoleId);
                        output = DBConnection.ExecSPNonQuery("SP_ROLE_UPDATE", dicParas, ref dicParaOuts, true);
                    }
                }
                
                //STEP2:  ***************************************************************/
                
            }
            catch(Exception ex)
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
                dicParas.Add("ROLEID", id);
                
                    //dicParas.Add("")
                    output = DBConnection.ExecSPNonQuery("SP_ROLE_DELETE", dicParas, ref dicParaOuts, true);
                
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

        public List<EntityRoleModel> GetList(int? deptId = null)
        {
            List<EntityRoleModel> output = new  List<EntityRoleModel>();
            this.DBConnection.OpenDBConnect();
            if (this.DBConnection.ERROR != null)
            {
                this.DBConnection.CloseDBConnect();
                throw new Exception("Can't connect to db");
            };

            try
            {
                DBConnection.command.Parameters.Clear();
                DBConnection.command.CommandText = USP_GET_ROLES_BY_ORGID;
                DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@deptID", (object)deptId ?? DBNull.Value);



                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityRoleModel entityRole = new EntityRoleModel();
                            entityRole.RoleId = reader.GetInt32(reader.GetOrdinal("RoleId"));
                            entityRole.Name = reader.GetString(reader.GetOrdinal("Name"));
                            entityRole.Description = reader.GetString(reader.GetOrdinal("Description"));
                            entityRole.DepartmentName = reader.GetString(reader.GetOrdinal("OrganizationName"));
                            entityRole.DepartId = reader.GetInt32(reader.GetOrdinal("DepartId"));
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

        public List<EntityReportRoleModel> GetListReportRole(int RoleId)
        {
            List<EntityReportRoleModel> output = new List<EntityReportRoleModel>();
            this.DBConnection.OpenDBConnect();
            if (this.DBConnection.ERROR != null)
            {
                this.DBConnection.CloseDBConnect();
                throw new Exception("Can't connect to db");
            };

            try
            {
                DBConnection.command.Parameters.Clear();
                DBConnection.command.CommandText = "Select * from GB_RoleReport where RoleId=@RoleId";
                DBConnection.command.CommandType = CommandType.Text;
                DBConnection.command.Parameters.AddWithValue("@RoleId", RoleId);
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityReportRoleModel entityRole = new EntityReportRoleModel();
                            entityRole.RoleId = reader.GetInt32(reader.GetOrdinal("RoleId"));
                            entityRole.ReportId = reader.GetInt32(reader.GetOrdinal("ReportBIId"));                         
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
        public List<EntityRoleModel> GetListDept(int? deptId = null)
        {
            List<EntityRoleModel> output = new List<EntityRoleModel>();
            this.DBConnection.OpenDBConnect();
            if (this.DBConnection.ERROR != null)
            {
                this.DBConnection.CloseDBConnect();
                throw new Exception("Can't connect to db");
            };

            try
            {
                DBConnection.command.Parameters.Clear();
                DBConnection.command.CommandText = USP_GET_ROLES_BY_ORGID;
                DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@deptID",deptId);



                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityRoleModel entityRole = new EntityRoleModel();
                            entityRole.RoleId = reader.GetInt32(reader.GetOrdinal("RoleId"));
                            entityRole.Name = reader.GetString(reader.GetOrdinal("Name"));
                            entityRole.Description = reader.GetString(reader.GetOrdinal("Description"));
                            entityRole.DepartmentName = reader.GetString(reader.GetOrdinal("OrganizationName"));
                            entityRole.DepartId = reader.GetInt32(reader.GetOrdinal("DepartId"));
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

        public BlockDataRoleCreateModel GetEntityById(int id)
        {
            BlockDataRoleCreateModel output = new BlockDataRoleCreateModel();
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("ROLEID", id);
                DataSet dataSet  = DBConnection.ExecSelectSP("SP_ROLE_GET_BY_ID", dicParas, ref dicParaOuts, true);
                //**********************TABLE: ROLE***************************************
                DataTable table1 = dataSet.Tables[0];
                foreach (DataRow row in table1.Rows)
                {
                    output.Name = row["Name"].ToString();
                    output.Description = row["Description"].ToString();
                    output.RoleId = Int32.Parse( row["RoleId"].ToString());
                    output.DeptID = Int32.Parse(row["DepartId"].ToString());
                }
                //**********************TABLE: ROLEMENU ***********************************************
                DataTable table2 = dataSet.Tables[1];

                foreach(DataRow row in table2.Rows)
                {
                    output.ListRoleMenus.Add(Int32.Parse(row["menuid"].ToString()));
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