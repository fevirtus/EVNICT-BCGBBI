
using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Web;

namespace BI_Project.Services
{
    public class ConnectOracleDB
    {
        public string ConnectString { set; get; }

        public string ERROR { set; get; }

        public OracleConnection OracleDBConnect { set; get; }
        public OracleCommand command;
        public ConnectOracleDB(string connectString)
        {
            ConnectString = connectString;
            OracleDBConnect = new OracleConnection(connectString);
            this.command = new OracleCommand();
        }

        public void OpenDBConnect()
        {
            try
            {
                //OracleDBConnect = new OracleConnection(this.ConnectString);
                this.OracleDBConnect.Open();

                command = new OracleCommand();
                command.Connection = OracleDBConnect;

            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
        }

        public void CloseDBConnect()
        {
            try
            {
                if (OracleDBConnect.State == ConnectionState.Open)
                {
                    this.OracleDBConnect.Close();
                    this.OracleDBConnect.Dispose();
                    OracleConnection.ClearAllPools();

                }
                if (null != command)
                {
                    command.Parameters.Clear();
                }
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
        }
        /// <summary>
        /// used to execute a store procedure to change value of data records
        /// as update, delete, insert
        /// </summary>
        /// <param name="spName">name of store procedure</param>
        /// <param name="dicParameters">list of parameters containt the pairs of (name,value) that
        ///     mapping with paramenters of the store procedure
        /// </param>
        /// <param name="dicParaOutputs">Note that: this param enable be null.
        /// It is the list of paramenter in which the para is declared as output parameter</param>
        /// <returns>the number of  record that be effected when the store procedure be excuted</returns>
        public int ExecSPNonQuery(string spName, Dictionary<string, object> dicParameters, ref Dictionary<string, object> dicParaOutputs, bool isCloseConnect = false)
        {
            int result = 0;
            try
            {
                this.OpenDBConnect();
                if (this.ERROR != null) throw new Exception("Can't connect to db");

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = spName;

                Dictionary<string, object> dicTemp = new Dictionary<string, object>();

                foreach (string paraname in dicParameters.Keys)
                {
                    OracleParameter para = new OracleParameter(":" + paraname, dicParameters[paraname] ?? DBNull.Value);
                    command.Parameters.Add(para);
                }
                foreach (string paraname in dicParaOutputs.Keys)
                {
                    OracleParameter para = new OracleParameter(":" + paraname, dicParaOutputs[paraname] ?? DBNull.Value);
                    para.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(para);
                    dicTemp.Add(paraname, para);


                }

                result = command.ExecuteNonQuery();

                dicParaOutputs.Clear();
                foreach (string paraname in dicTemp.Keys)
                {

                    object outvalue = command.Parameters[":" + paraname].Value;
                    dicParaOutputs.Add(paraname, outvalue);
                }

                dicTemp.Clear();


            }
            catch (Exception exc)
            {
                ERROR = exc.ToString();
            }
            finally
            {
                if (isCloseConnect == true)
                    this.CloseDBConnect();
            }

            return result;
        }



        public OracleDataReader ExecSPReader(string spName, Dictionary<string, object> dicParameters, bool isCloseConnect = false)
        {
            OracleDataReader result = null;
            try
            {

                this.OpenDBConnect();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = spName;


                //add input paras 
                foreach (string paraName in dicParameters.Keys)
                {
                    OracleParameter para = new OracleParameter(":" + paraName, dicParameters[paraName]);
                    command.Parameters.Add(para);
                }



                result = command.ExecuteReader(CommandBehavior.CloseConnection);



            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
            }

            finally
            {
                if (isCloseConnect == true)
                    this.CloseDBConnect();
            }
            return result;
        }

        public DataSet ExecSelectSP(string spName, Dictionary<string, object> dicParameters, ref Dictionary<string, object> dicParaOutputs, bool isCloseConnect = false)
        {

            DataSet result = null;
            try
            {



                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = spName;


                this.OpenDBConnect();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = spName;

                Dictionary<string, object> dicTemp = new Dictionary<string, object>();

                foreach (string paraname in dicParameters.Keys)
                {
                    OracleParameter para = new OracleParameter(":" + paraname, dicParameters[paraname]);
                    command.Parameters.Add(para);
                }
                foreach (string paraname in dicParaOutputs.Keys)
                {
                    OracleParameter para = new OracleParameter(":" + paraname, dicParaOutputs[paraname]);
                    para.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(para);
                    dicTemp.Add(paraname, para);


                }

                result = new DataSet();
                OracleDataAdapter sqlDa = new OracleDataAdapter(command);
                sqlDa.Fill(result);


                dicParaOutputs.Clear();
                foreach (string paraname in dicTemp.Keys)
                {

                    object outvalue = command.Parameters[":" + paraname].Value;
                    dicParaOutputs.Add(paraname, outvalue);
                }

                dicTemp.Clear();

            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
            }
            finally
            {
                if (isCloseConnect == true)
                    this.CloseDBConnect();
            }

            return result;
        }


        public object ExecSPReturnValue(string spName, Dictionary<string, object> dicParameters, ref Dictionary<string, object> dicParaOutputs, bool isCloseConnect = false)
        {
            object result = null;
            try
            {

                this.OpenDBConnect();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = spName;

                Dictionary<string, object> dicTemp = new Dictionary<string, object>();

                foreach (string paraname in dicParameters.Keys)
                {
                    OracleParameter para = new OracleParameter("@" + paraname, dicParameters[paraname]);
                    command.Parameters.Add(para);
                }
                foreach (string paraname in dicParaOutputs.Keys)
                {
                    OracleParameter para = new OracleParameter("@" + paraname, dicParaOutputs[paraname]);
                    para.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(para);
                    dicTemp.Add(paraname, para);


                }
                OracleParameter paraReturn = new OracleParameter("@Return_Value", -1);
                paraReturn.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(paraReturn);

                command.Connection.Open();
                command.ExecuteNonQuery();
                result = paraReturn.Value;

                dicParaOutputs.Clear();
                foreach (string paraname in dicTemp.Keys)
                {

                    object outvalue = command.Parameters[paraname].Value;
                    dicParaOutputs.Add(paraname, outvalue);
                }

                dicTemp.Clear();

            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
            }
            finally
            {
                if (isCloseConnect == true)
                    this.CloseDBConnect();
            }

            return result;
        }


    }
}