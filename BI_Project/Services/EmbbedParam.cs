using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace bicen.Services
{
    public class EmbbedParam
    {

        public string menu { get; set; }
        public string Site_Root { get; set; }
        public string pathTableau { get; set; }
        public string ticket { get; set; }
        public string leftMenuId { get; set; }
        public int id { get; set; }
        public string leftParentMenuId { get; set; }
        public string leftParentParentMenuId { get; set; }
        public string filter { get; set; }
        public string username { get; set; }
        /***********************************/
        public Boolean GetFilter()
        {
            filter = "";
            SqlConnection conSQLS = null;
            string sqlCmd = "";
            SqlCommand cmd = null;
            //UserService User = new User();

            try
            {
                conSQLS = new SqlConnection(ConfigurationManager.AppSettings["connect_string"].ToString());
                //sqlCmd = "SELECT FilterCommand,FilterValue FROM Menu WHERE UserName = @UserName";
                sqlCmd = "SELECT FilterCommand,FilterValue FROM Menu WHERE MenuId = @MID";
                //sqlCmd = "SELECT FilterCommand,FilterValue FROM Sys_Menu";
                using (cmd = new SqlCommand(sqlCmd, conSQLS))
                {
                    conSQLS.Open();
                    cmd.Parameters.AddWithValue("@MID", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("FilterValue")))
                                    filter = reader.GetString(reader.GetOrdinal("FilterValue"));
                                if (!reader.IsDBNull(reader.GetOrdinal("FilterCommand")))
                                    sqlCmd = reader.GetString(reader.GetOrdinal("FilterCommand"));
                            }
                        }
                    }
                }

                //Process the filter value
                if (filter.Length > 0)
                {

                    using (cmd = new SqlCommand(sqlCmd, conSQLS))
                    {
                        //conSQLS.Open();
                        cmd.Parameters.AddWithValue("@UserName", username);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                if (reader.Read())
                                {
                                    if (!reader.IsDBNull(0))
                                        filter += reader.GetString(0);
                                }
                            }
                        }
                    }
                }

                conSQLS.Close();
                conSQLS.Dispose();
                sqlCmd = "";
                return filter.Length > 0;
            }
            catch (Exception e)
            {
                filter = "";
                return false;
            }
        }
        /****************************************************/
    }//CLASS
}