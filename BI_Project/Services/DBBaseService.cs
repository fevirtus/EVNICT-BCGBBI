using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Helpers.Security;
using System.Data.SqlClient;

namespace BI_Project.Services
{
    public class DBBaseService
    {
        public string ERROR { set; get; }
        public DBConnection DBConnection { set; get; }
        public ConnectOracleDB ConnectOracleDB { get; set; }
        public bool IsCloseDBAfterExecute { set; get; }

        public DBBaseService(DBConnection dBConnection)
        {
            DBConnection = dBConnection;
           IsCloseDBAfterExecute = true;
        }

        public DBBaseService(DBConnection connection, string dbStagingConnectionString)
        {

        }
        public DBBaseService (ConnectOracleDB connectOracleDB)
        {
            ConnectOracleDB = connectOracleDB;
            IsCloseDBAfterExecute = true;
        }

        public DBBaseService(ConnectOracleDB connectOracleDB, DBConnection dBConnection)
        {
            ConnectOracleDB = connectOracleDB;
            this.DBConnection = dBConnection;
            IsCloseDBAfterExecute = true;
        }

        public bool CheckPermission(int menuid,int userid,int roleid, bool isCheckRole)
        {
            bool output = true;
            return output;
        }

        //public object GetReaderValue(SqlDataReader dataReader, string columnName)
        //{
        //    object output = null;
        //    int index = dataReader.GetOrdinal(columnName);
        //    if (dataReader.IsDBNull(index)) { return output; }
        //    output = dataReader[index];
        //    return output;

        //}

        public object GetReaderValue(SqlDataReader dataReader, string columnName)
        {
            object output = null;
            int index = dataReader.GetOrdinal(columnName);
            if (dataReader.IsDBNull(index)) { return output; }
            output = dataReader[index];
            return output;

        }
    }


}