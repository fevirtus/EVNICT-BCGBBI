using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Services.Importers
{
    public class XMLParaModel
    {
        public string Name { set; get; }
        public string Resource { set; get; }

        public int Row { set; get; }
        public int Column { set; get; }
        public string Active { set; get; }
        public string ERROR { get; set; }
        private object _value;

        public string DataType { set; get; }
        public object Value
        {
            set
            {

                try
                {

                    if (null == value)
                    {
                        value = (object)DBNull.Value;
                        return;
                    }
                    switch (DataType)
                    {
                        case "int":

                            _value = Convert.ToInt32(value.ToString());
                            break;
                        case "float":

                            _value = Decimal.Parse(value.ToString());
                            break;
                        case "date":
                            _value = DateTime.Parse(value.ToString());
                            break;
                        default:

                            _value = value;
                            break;
                    }




                }
                catch (Exception ex)
                {
                    _value = null;
                    ERROR = ex.ToString();
                }

            }
            get
            {
                return _value;
            }
        }
    }
}