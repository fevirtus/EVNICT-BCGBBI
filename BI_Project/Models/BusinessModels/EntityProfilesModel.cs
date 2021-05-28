using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;


namespace BI_Project.Models.EntityModels
{
    public class EntityProfilesModel
    {
        public string PROFILEID { get; set; }
        public string DESCR { get; set; }
        public string PARENT_PROFILEID { set; get; }
        public string PROFILE_TYPE { set; get; }
        public int DISPORDER { get; set; }
        
    }


    public class EntityQuanLyMau
    {
        public string DEPARTMENTID { get; set; }
        public string PROFILEID { get; set; }
        public string STRATUMID { get; set; }        
        public int YEAR_LRS { get; set; }
        public int DESIGNPOPN { get; set; }
        public decimal DESIGNPOPENERGY { get; set; }
        public int DESIGNNSAMPLE { get; set; }
        public string USERCOMMENT { get; set; }

    }


    //tìm kiếm
    public class EntityTimKiemModel
    {
        public string MA_DVIQLY { get; set; }
        public int THANG { get; set; }
        public int NAM { get; set; }
        public int LUYKE { get; set; }
    }


}