using bicen.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bicen.ViewModels
{
    public class DVKH
    {
        public List<DNT_QMKL_QD41> DNT_QMKL_QD41 { get; set; } = new List<DNT_QMKL_QD41>();
        public List<DNT_QMKLTN_HA1820> DNT_QMKLTN_HA1820 { get; set; } = new List<DNT_QMKLTN_HA1820>();
        public List<DNT_QMKLTN_NVK> DNT_QMKLTN_NVK { get; set; } = new List<DNT_QMKLTN_NVK>();
        public List<DNT_QMKLTN_QD2081> DNT_QMKLTN_QD2081 { get; set; } = new List<DNT_QMKLTN_QD2081>();
        public List<DNT_THCDIEN_PL71> DNT_THCDIEN_PL71 { get; set; } = new List<DNT_THCDIEN_PL71>();
        public List<DNT_THCDIEN_PL72> DNT_THCDIEN_PL72 { get; set; } = new List<DNT_THCDIEN_PL72>();
 
    }
}