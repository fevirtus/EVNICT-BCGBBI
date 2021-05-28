using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Models.EntityModels
{
    public class CMIS_KDDN12
    {
        public string MA_NN { get; set; }
        public string MUC_NN { get; set; }
        public decimal SAN_LUONG { get; set; }
        public decimal SO_HDONG { get; set; }
        public string TEN_NN { get; set; }
    }

    public class LRS_RESUL
    {
        public string PROFILEID { get; set; }
        public string DESCR { get; set; }
        public string PROFILE_TYPE { set; get; }
        public decimal Total { get; set; }
        public int THANG { get; set; }
        public int NAM { get; set; }
        public string LOAI_BC { get; set; }
        public decimal SAN_LUONG { get; set; }
        public decimal SO_HDONG { get; set; }
    }

    public class ANHXA_LRS_CMIS
    {
        public string PROFILEID { get; set; }
        public string MA_NN { get; set; }
    }

}