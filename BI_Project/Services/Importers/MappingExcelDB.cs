using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace BI_Project.Services.Importers
{
    public class MappingExcelDB
    {

        public string ERROR { set; get; }
        private object _value;
        public string ColumnName { set; get; }
        public string Datatype { set; get; }

        public int ExcelColumn { set; get; }
        public object Value
        {
            set
            {

                try
                {

                    //if (null == value)
                    //{
                    //    value = (object) DBNull.Value;
                    //    return;
                    //}
                    if (value.ToString() == "")
                    {
                        value = (object)DBNull.Value;
                        _value = value;
                        return;
                    }
                    switch (Datatype)
                    {
                        case "int":
                            
                            _value = Convert.ToInt32(value.ToString());
                            break;
                        case "float":
                            
                            _value = Convert.ToDecimal(value.ToString());
                            break;
                        default:
                            
                            _value = value;
                            break;
                    }
                    
                    
                    

                }
                catch (Exception ex)
                {
                    //_value = null;
                    //ERROR = ex.ToString();
                    throw new Exception(ex.Message);
                }

            }
            get
            {
                return _value;
            }
        }
        //public static object GetDataValue(string data, string Datatype)
        //{
        //        if (null == data) return data;
        //        try
        //        {

        //            if (Datatype == "int") return Int32.Parse(data);
        //            if (Datatype == "float") return float.Parse(data);
                    

        //        }
        //        catch (Exception)
        //        {
        //            return data;
        //        }

        //        return data;
           

        //}


        
    }
}