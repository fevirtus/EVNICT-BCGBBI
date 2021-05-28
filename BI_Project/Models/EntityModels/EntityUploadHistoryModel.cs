using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;


namespace BI_Project.Models.EntityModels
{
    public class EntityUploadHistoryModel
    {
        public int UserId { get; set; }
        public int UploadRoleId { get; set; }

        public DateTime StartTime { set; get; }

        public DateTime EndTime { set; get; }

        public string HelpDocumentPath { set; get; }

        public int Week { get; set; }
        public int Month { get; set; }
        public int Quarter { get; set; }
        public int Year { get; set; }

        public string Note { set; get; }
        public string FileNativeName { set; get; }

        public int NumberInsertedRow { set; get; }

        public string FileUploadedName { set; get; }
        public string UserName { set; get; }

        public static implicit operator EntityUploadHistoryModel (DataRow dr)
        {
            EntityUploadHistoryModel model = new EntityUploadHistoryModel();

            try
            {
                model.EndTime = (DateTime)dr["EndTime"];
                model.FileNativeName = (string)dr["FileNativeName"];
                model.HelpDocumentPath = (string)dr["HelpDocumentPath"];
                model.Month = (int)dr["Month"];
                model.Note = (string)dr["Note"];
                 
                try
                {
                    if (null == dr["NumberInsertedRow"])
                        model.NumberInsertedRow = 0;
                    else
                        model.NumberInsertedRow = (int)dr["NumberInsertedRow"];
                }
                catch(Exception)
                {

                }
                
                model.Quarter = (int)dr["Quarter"];
                model.StartTime = (DateTime)dr["StartTime"];
                model.UserId = (int)dr["UserId"];
                model.Week = (int)dr["Week"];
                model.Year = (int)dr["Year"];
                model.UserName = (string)dr["UserName"];
                model.FileUploadedName = (string)dr["FileUploadedName"];
                
            }
            catch(Exception)
            {

            }
            return model;
        }

        
    }
}