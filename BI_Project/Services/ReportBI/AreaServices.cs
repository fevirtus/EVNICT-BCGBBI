using BI_Project.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BI_Project.Services.ReportBi
{
    public class AreaServices : DBBaseService
    {
        public AreaServices(DBConnection connection) : base(connection)
        {

        }
        public List<EntityAreaModel> GetList()
        {
            List<EntityAreaModel> output = new List<EntityAreaModel>();
            this.DBConnection.OpenDBConnect();
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

            try
            {
                string sqlSelectDepart = " select * from GB_Area where IsDelete=0";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = sqlSelectDepart;


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityAreaModel entityRole = new EntityAreaModel();
                            entityRole.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            entityRole.Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"));
                            entityRole.ParentId = reader.IsDBNull(reader.GetOrdinal("ParentId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ParentId"));
                            entityRole.AreaName = reader.IsDBNull(reader.GetOrdinal("AreaName")) ? null : reader.GetString(reader.GetOrdinal("AreaName"));
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

      
       
    }
}