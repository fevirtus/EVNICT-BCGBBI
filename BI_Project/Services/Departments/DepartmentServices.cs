using BI_Project.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BI_Project.Services.Departments
{
    public class DepartmentServices : DBBaseService
    {
        public DepartmentServices(DBConnection connection) : base(connection)
        {
        }

        public List<EntityDepartmentModel> GetList()
        {
            List<EntityDepartmentModel> output = new List<EntityDepartmentModel>();
            this.DBConnection.OpenDBConnect();
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

            try
            {
                string sqlSelectDepart = " select * from Department ";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = sqlSelectDepart;


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityDepartmentModel entityRole = new EntityDepartmentModel();
                            entityRole.DepartId = reader.GetInt32(reader.GetOrdinal("Id"));
                            entityRole.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name"));
                            entityRole.Code = reader.IsDBNull(reader.GetOrdinal("Code")) ? null : reader.GetString(reader.GetOrdinal("Code"));
                            entityRole.Filter01 = reader.IsDBNull(reader.GetOrdinal("Filter01")) ? null : reader.GetString(reader.GetOrdinal("Filter01"));

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

        public List<EntityDepartmentModel> GetListAdminLogin(string code)
        {
            List<EntityDepartmentModel> output = new List<EntityDepartmentModel>();
            this.DBConnection.OpenDBConnect();
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

            try
            {
                string sqlSelectDepart = " select * from Department where Code = @Code  ";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.Parameters.AddWithValue("@Code", code);
                this.DBConnection.command.CommandText = sqlSelectDepart;


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityDepartmentModel entityRole = new EntityDepartmentModel();
                            entityRole.DepartId = reader.GetInt32(reader.GetOrdinal("Id"));
                            entityRole.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name"));
                            entityRole.Filter01 = reader.IsDBNull(reader.GetOrdinal("Filter01")) ? null : reader.GetString(reader.GetOrdinal("Filter01"));
                            entityRole.Code = reader.IsDBNull(reader.GetOrdinal("Code")) ? null : reader.GetString(reader.GetOrdinal("Code"));
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



        public EntityDepartmentModel GetEntityById(int? id = null)
        {
            EntityDepartmentModel output = new EntityDepartmentModel();
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("DEPARTID", id);
                DataSet dataSet = DBConnection.ExecSelectSP("SP_DEPART_GET_BY_ID", dicParas, ref dicParaOuts, true);
                //**********************TABLE: ROLE***************************************
                DataTable table1 = dataSet.Tables[0];
                foreach (DataRow row in table1.Rows)
                {
                    output.Name = row["Name"].ToString();                  
                    output.DepartId = Int32.Parse(row["Id"].ToString());
                    output.Filter01 = row["Filter01"].ToString();
                    output.Code = row["Code"].ToString();
                }
                //**********************TABLE: ROLEMENU ***********************************************
                //DataTable table2 = dataSet.Tables[1];

                //foreach (DataRow row in table2.Rows)
                //{
                //    output.ListRoleMenus.Add(Int32.Parse(row["menuid"].ToString()));
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

        public int CreateDepart(EntityDepartmentModel departModel)
        {
            int output = 0;

            try
            {


                DBConnection.OpenDBConnect();
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();

                if (departModel.Name != null)
                {
                    dicParas.Add("Name", departModel.Name);
                    dicParas.Add("Filter01", departModel.Filter01);
                    dicParas.Add("Code", departModel.Code);
                    dicParas.Add("Filter02", departModel.Filter02);

                    if (departModel.DepartId == 0)
                        //dicParas.Add("")
                        output = DBConnection.ExecSPNonQuery("SP_DEPARTMENT_INSERT", dicParas, ref dicParaOuts, true);
                    else
                    {
                        dicParas.Add("DepartId", departModel.DepartId);
                        output = DBConnection.ExecSPNonQuery("SP_DEPARTMENT_UPDATE", dicParas, ref dicParaOuts, true);
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
                dicParas.Add("DEPARTID", id);

                //dicParas.Add("")
                output = DBConnection.ExecSPNonQuery("SP_DEPARTMENT_DELETE", dicParas, ref dicParaOuts, true);

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