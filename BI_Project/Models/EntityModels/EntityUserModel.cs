using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace BI_Project.Models.EntityModels
{
    public class EntityUserModel
    {
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}", ErrorMessage = "Password minimum 8 characters and contain at least one uppercase, one lowercase, one number.")]

        //public static bool isvalidstring(string password)
        //{
        //    string pattern = @"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[a-z]).*$";
        //    return regex.ismatch(password, pattern);
        //}
        [Required]
        public string Password { get; set; }
        public string Salt { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        //[EmailAddress(ErrorMessage = "Định dạng email không đúng")]
        public string Email { get; set; }
        [Required]
        //[MaxLength(12)]
        //[MinLength(1)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "UPRN must be numeric")]
        public string Phone { get; set; }
        [Required]
        public bool IsAdmin { get; set; }

        public List<EntityUserRoleModel> LstUserRoles { get; set; }

        public List<EntityRoleModel> lstRoles { get; set; }
        public List<EntityMenuModel> LstMenus { get; set; }

        public string DepartName { get; set; }
        public int DeptId { get; set; }

        public bool IsSuperAdmin { get; set; }
        public string Code { get; set; }
        public List<Int32> LstSelectedRole { get; set; }   // Danh sách các Role đã chọn từ select2
        public List<Int32> LstSelectedMenu { get; set; }
        public string FullName { get; set; }

        public EntityUserModel()
        {
            LstMenus = new List<EntityMenuModel>();
            LstUserRoles = new List<EntityUserRoleModel>();
            LstSelectedRole = new List<int>();
            LstSelectedMenu = new List<int>();
        }
    }

   



}