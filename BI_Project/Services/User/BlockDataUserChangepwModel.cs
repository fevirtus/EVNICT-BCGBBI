using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.EntityModels;
namespace BI_Project.Services.User
{
    public class BlockDataUserChangepwModel 
    {

        public string Password { get; set; }

        public string OldPassword { get; set; }

        public string ConfirmPassword { set; get; }

        public int UserId { set; get; }
        public BlockDataUserChangepwModel():base()
        {
            
        }
        
    }
}