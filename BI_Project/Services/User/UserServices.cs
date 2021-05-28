using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BI_Project.Models.EntityModels;
using BI_Project.Helpers.Security;
using System.Data.SqlClient;
using System.Data;

using BI_Project.Services.User;
using BI_Core.Helpers;
using bicen.Models.EntityModels;

namespace BI_Project.Services.User
{
    public class UserServices : DBBaseService
    {
        private static string USP_GET_ALL_USERS = "usp_Get_All_Users";
        private static string USP_CHECK_USER_LOG_IN = "usp_Check_User_Log_In";
        private static string USP_CHECK_USER_LOG_IN_ADMIN = "usp_Check_User_Log_In_Admin";
        private static string SP_GET_CODE_BY_DEPTID = "SP_GET_CODE_BY_DEPTID";
        public UserServices(DBConnection dBConnection) : base(dBConnection)
        {

        }

        public EntityUserModel CheckLogin(LoginModel loginModel)
        {
            EntityUserModel output = new EntityUserModel();
            bool isNotLDAP = true;
            string sqlUserLdap = "";
            try
            {


                string passwordHashed = "";
                var _salt = string.Empty;
                //sqlUserLdap = Utilities.IsAuthenticated(loginModel.UserName, loginModel.Password, loginModel.Department);
                if (sqlUserLdap == "")
                {
                    isNotLDAP = false;
                }
                //if (loginModel.Department == "P")
                //{
                //    string sqlUserSalt = "Select Salt from Users Where UserName = @UserName and deptID = 0";
                    

                //    PasswordManager pwm = new PasswordManager();

                //    DBConnection.OpenDBConnect();
                //    if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //    //STEP1:  ***************************************************************/
                //    //******************BAM MAT KHAU THEO SALT (CO TRONG CSDL) VA MAT KHAU DUOC NHAP VAO ***********/
                //    DBConnection.command.Parameters.AddWithValue("@UserName", loginModel.UserName);
                //    DBConnection.command.CommandText = sqlUserSalt;
                //    _salt = DBConnection.command.ExecuteScalar() as string;

                //    passwordHashed = pwm.IsMatch(loginModel.Password, _salt);

                //    //STEP2:  ***************************************************************/
                //    DBConnection.command.Parameters.Clear();
                //    DBConnection.command.CommandText = USP_CHECK_USER_LOG_IN_ADMIN;
                //    DBConnection.command.CommandType = CommandType.StoredProcedure;
                //    DBConnection.command.Parameters.AddWithValue("@UserName", loginModel.UserName);
                //    if (isNotLDAP)
                //    {
                //        DBConnection.command.Parameters.AddWithValue("@Password", passwordHashed);
                //    }
                //    else
                //    {
                //        DBConnection.command.Parameters.AddWithValue("@Password", DBNull.Value);
                //    }
                //}
                //else
                //{
                    string sqlUserSalt = "Select Salt from Users a , Department b Where a.UserName = @UserName and a.deptID = b.Id";
                    

                    PasswordManager pwm = new PasswordManager();

                    DBConnection.OpenDBConnect();
                    if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                    //STEP1:  ***************************************************************/
                    //******************BAM MAT KHAU THEO SALT (CO TRONG CSDL) VA MAT KHAU DUOC NHAP VAO ***********/
                    DBConnection.command.Parameters.AddWithValue("@UserName", loginModel.UserName);
                    //DBConnection.command.Parameters.AddWithValue("@Code", loginModel.Department);
                    DBConnection.command.CommandText = sqlUserSalt;
                    _salt = DBConnection.command.ExecuteScalar() as string;

                    passwordHashed = pwm.IsMatch(loginModel.Password, _salt);

                    //STEP2:  ***************************************************************/
                    DBConnection.command.Parameters.Clear();
                    DBConnection.command.CommandText = USP_CHECK_USER_LOG_IN;
                    DBConnection.command.CommandType = CommandType.StoredProcedure;
                    DBConnection.command.Parameters.AddWithValue("@UserName", loginModel.UserName);
                    //if (loginModel.Department == "P")
                    //{
                        DBConnection.command.Parameters.AddWithValue("@Code", DBNull.Value);
                    //}
                    //else
                    //{
                    //    DBConnection.command.Parameters.AddWithValue("@Code", loginModel.Department);
                    //}
                    if (isNotLDAP)
                    {
                        DBConnection.command.Parameters.AddWithValue("@Password", passwordHashed);
                    }
                    else
                    {
                        DBConnection.command.Parameters.AddWithValue("@Password", DBNull.Value);
                    }
                //}
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            output.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            output.UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? "" : reader.GetString(reader.GetOrdinal("UserName"));
                            output.Salt = reader.IsDBNull(reader.GetOrdinal("Salt")) ? "" : reader.GetString(reader.GetOrdinal("Salt"));
                            output.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email"));
                            output.Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader.GetString(reader.GetOrdinal("Phone"));
                            output.IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"));
                            output.DeptId = reader.GetInt32(reader.GetOrdinal("deptID"));
                            output.FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? "" : reader.GetString(reader.GetOrdinal("FullName"));

                        }
                    }
                }
                output.IsSuperAdmin = output.IsAdmin;
                    //&& (output.DeptId == 0);
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }


            return output;
        }

        public BlockDataMenuLeftModel GetListMenus(int userId, bool isAdmin)
        {
            BlockDataMenuLeftModel output = new BlockDataMenuLeftModel();


            try
            {
                EntityUserModel entityUser = new EntityUserModel();

                entityUser.UserId = userId;
                entityUser.IsAdmin = isAdmin;

                output = this.GetListMenus(entityUser);
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
                this.DBConnection.CloseDBConnect();
            }

            return output;
        }
        public BlockDataMenuLeftModel GetListMenus(EntityUserModel entityUser)
        {
            BlockDataMenuLeftModel output = new BlockDataMenuLeftModel();

            this.DBConnection.OpenDBConnect();
            output.EntityUserModel = entityUser;
            try
            {


                //STEP1: GET ALLOWED MENUID FOR THE CURRENT USER
                string sqlUserMenuIds = " ", sqlRoleMenuIds = "", sqlMenus = "";



                //sqlUserMenuIds = "select distinct * from UserMenu where userid=@userid";
                sqlUserMenuIds = "select MenuId from UserMenu where userid=@userid";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = sqlUserMenuIds;
                this.DBConnection.command.Parameters.AddWithValue("@userid", entityUser.UserId);
                //entityUser.LstSelectedMenu.Clear();
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int menuid = reader.GetInt32(reader.GetOrdinal("MenuId"));
                            if (!entityUser.LstSelectedMenu.Contains(menuid))
                                entityUser.LstSelectedMenu.Add(menuid);
                        }
                    }
                }

                //GET MENUID FROM ROLEMENU

                sqlRoleMenuIds = "select distinct rm.MenuId from Users as u, UserRole as ur, RoleMenu as rm where ( " +
                                "   u.UserId = @userid and u.UserId = ur.UserId and rm.RoleId = ur.RoleId )";

                this.DBConnection.command.CommandText = sqlRoleMenuIds;

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int menuid = reader.GetInt32(reader.GetOrdinal("MenuId"));
                            if (!entityUser.LstSelectedMenu.Contains(menuid))
                                entityUser.LstSelectedMenu.Add(menuid);
                        }

                    }
                }
                //STEP2: GET ALL MENU ******************************************************
                sqlMenus = "select * from Menu order by leveltree ";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = sqlMenus;
                List<EntityMenuModel> lstMenuTemp = new List<EntityMenuModel>();
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityMenuModel entityMenuModel = new EntityMenuModel();
                            entityMenuModel.FilterCommand = reader.IsDBNull(reader.GetOrdinal("FilterCommand")) ? "" : reader.GetString(reader.GetOrdinal("FilterCommand"));
                            entityMenuModel.FilterValue = reader.IsDBNull(reader.GetOrdinal("FilterValue")) ? "" : reader.GetString(reader.GetOrdinal("FilterValue"));
                            entityMenuModel.LevelTree = reader.IsDBNull(reader.GetOrdinal("LevelTree")) ? "" : reader.GetString(reader.GetOrdinal("LevelTree"));
                            entityMenuModel.MenuId = reader.IsDBNull(reader.GetOrdinal("MenuId")) ? 0 : reader.GetInt32(reader.GetOrdinal("MenuId"));
                            entityMenuModel.MenuLevel = reader.IsDBNull(reader.GetOrdinal("MenuLevel")) ? "" : reader.GetString(reader.GetOrdinal("MenuLevel"));
                            entityMenuModel.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name"));
                            entityMenuModel.ParentId = reader.IsDBNull(reader.GetOrdinal("Name")) ? 0 : reader.GetInt32(reader.GetOrdinal("ParentId"));
                            entityMenuModel.Path = reader.IsDBNull(reader.GetOrdinal("Path")) ? "" : reader.GetString(reader.GetOrdinal("Path"));
                            entityMenuModel.Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? false : reader.GetBoolean(reader.GetOrdinal("Status"));
                            entityMenuModel.IconCSS = reader.IsDBNull(reader.GetOrdinal("IconCSS")) ? "" : reader.GetString(reader.GetOrdinal("IconCSS"));
                            lstMenuTemp.Add(entityMenuModel);
                            if (entityUser.IsAdmin == true)
                            {
                                entityUser.LstSelectedMenu.Add(entityMenuModel.MenuId);
                                output.StrAllowedLeveltrees += entityMenuModel.LevelTree + ",";
                                output.StrAllowedMenuIds += entityMenuModel.MenuId + ",";
                                continue;
                            }
                            if (entityUser.LstSelectedMenu.Contains(entityMenuModel.MenuId))
                            {
                                // output.LstAllowedMenus.Add(entityMenuModel.Clone());
                                output.StrAllowedLeveltrees += entityMenuModel.LevelTree + ",";
                                output.StrAllowedMenuIds += entityMenuModel.MenuId + ",";
                            }
                        }

                    }
                }
                output.StrAllowedMenuIds = "," + output.StrAllowedMenuIds;
                output.StrAllowedLeveltrees = "," + output.StrAllowedLeveltrees;
                foreach (EntityMenuModel entity in lstMenuTemp)

                {
                    if (entityUser.LstSelectedMenu.Contains(entity.MenuId))
                    {
                        //output.LstAllowedMenus.Add(entity.Clone());
                        //output.LstAllOfMenus.Add(entity.Clone());
                        continue;
                    }

                    //else
                    //{
                    //    string currentMenuLeveltree = entity.LevelTree + "@@@";
                    //    bool added = false;
                    //    while (currentMenuLeveltree.Length > 0)
                    //    {
                    //        int pos = currentMenuLeveltree.LastIndexOf("@@@");
                    //        if (pos < 0) break;
                    //        currentMenuLeveltree = currentMenuLeveltree.Substring(0, pos);

                    //        //DAY LA TRUONG HOP CO MENU CHA NAM TRONG SO DUOC PHAN QUYEN
                    //        //THI ADD MENU HIEN TAI VAO NHOM 
                    //        if (output.StrAllowedLeveltrees.IndexOf("," + currentMenuLeveltree + ",") >= 0)
                    //        {
                    //            output.StrAllowedLeveltrees += entity.LevelTree + ",";
                    //            output.StrAllowedMenuIds += entity.MenuId.ToString() + ",";
                    //            entityUser.LstSelectedMenu.Add(entity.MenuId);
                    //            added = true;
                    //            break;
                    //        }

                    //    }


                    //}
                }

                foreach (EntityMenuModel entity in lstMenuTemp)
                {
                    if (entityUser.IsAdmin == true)
                    {
                        output.LstAllOfMenus.Add(entity.Clone());
                        continue;
                    }

                    if (entityUser.LstSelectedMenu.Contains(entity.MenuId))
                    {
                        output.LstAllOfMenus.Add(entity.Clone());
                    }

                    else
                    {
                        //KIEM TRA MENU HIEN TAI CO LA MENU CHA CUA 1 TRONG SO CAC MENU DA DUOC ADD KO
                        if (output.StrAllowedLeveltrees.Contains("," + entity.LevelTree + "@"))
                            output.LstAllOfMenus.Add(entity.Clone());
                    }

                }

                //STEP3: GET LIST OF ALLOWED MENU FOR THE CURRENT USER
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }


            return output;
        }

        public int Create(BlockDataUserCreateModel model, string passOld, string saltOld)
        {
            int output = 0;
            string _salt = "";
            PasswordManager pwm = new PasswordManager();
            

            try
            {


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParaOuts.Add("result", "");
                string LstOfMenuIds = model.StrAllowedMenus;
                string LstOfRoleIds = string.Join(",", model.LstSelectedRole);
                if (model.IsSuperAdmin)
                {
                    model.IsAdmin = true;
                    model.DeptId = 0;
                }
                if (model.Password != null)

                {
                    //if(BlockDataUserCreateModel.IsValidString(model.Password) is false)
                    //{
                    //    throw new Exception("Mật khẩu phải chứa ít nhất 8 ký tự bao gồm ít nhất 1 kí tự là chữ viết thường, 1 kí tự là chữ viết hoa và 1 kí tự là số");
                    //}
                    
                    string passwordHashed = pwm.GetPasswordHashedAndGetSalt(model.Password, out _salt);
                    model.Password = passwordHashed;
                    model.Salt = _salt;

                }
                if (model.UserId > 0 && model.UserName != null && model.Email != null && model.Phone != null)
                {
                    dicParas.Add("UserId", model.UserId);
                    dicParas.Add("UserName", model.UserName);
                    if(model.Password == null)
                    {
                        dicParas.Add("Password", passOld);
                        dicParas.Add("Salt", saltOld);
                        dicParas.Add("Email", model.Email);
                        dicParas.Add("Phone", model.Phone);
                        dicParas.Add("IsAdmin", model.IsAdmin);
                        dicParas.Add("deptID", model.DeptId);
                        dicParas.Add("LstOfMenuIds", LstOfMenuIds);
                        dicParas.Add("LstOfRoleIds", LstOfRoleIds);
                        dicParas.Add("FullName", model.FullName);
                    }
                    else
                    {
                        dicParas.Add("Password", model.Password);
                        dicParas.Add("Salt", model.Salt);
                        dicParas.Add("Email", model.Email);
                        dicParas.Add("Phone", model.Phone);
                        dicParas.Add("IsAdmin", model.IsAdmin);
                        dicParas.Add("deptID", model.DeptId);
                        dicParas.Add("LstOfMenuIds", LstOfMenuIds);
                        dicParas.Add("LstOfRoleIds", LstOfRoleIds);
                        dicParas.Add("FullName", model.FullName);
                    }
                   


                    output = DBConnection.ExecSPNonQuery("SP_USER_INSERT_OR_UPDATE", dicParas, ref dicParaOuts, true);
                }
                else if (model.UserName != null && model.Password != null && model.Email != null && model.Phone != null)
                {
                    dicParas.Add("UserId", model.UserId);
                    dicParas.Add("UserName", model.UserName);
                    dicParas.Add("Password", model.Password);
                    dicParas.Add("Salt", model.Salt);
                    dicParas.Add("Email", model.Email);
                    dicParas.Add("Phone", model.Phone);
                    dicParas.Add("IsAdmin", model.IsAdmin);
                    dicParas.Add("deptID", model.DeptId);
                    dicParas.Add("LstOfMenuIds", LstOfMenuIds);
                    dicParas.Add("LstOfRoleIds", LstOfRoleIds);
                    dicParas.Add("FullName", model.FullName);
                    output = DBConnection.ExecSPNonQuery("SP_USER_INSERT_OR_UPDATE", dicParas, ref dicParaOuts, true);
                }
                else
                {
                    output = -1;
                }
                if (DBConnection.ERROR != null)
                    throw new Exception(DBConnection.ERROR);

            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
                output = -1;
            }
            finally
            {
                if (this.IsCloseDBAfterExecute) this.DBConnection.CloseDBConnect();
            }

            return output;
        }


        public int ChangePw(BlockDataUserChangepwModel model)
        {
            int output = 0;
            string _oldsalt = "", newSalt = "";
            PasswordManager pwm = new PasswordManager();
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

                _oldsalt = string.Empty;

                string sqlUserSalt = "Select Salt from Users Where UserId = @Userid ";




                //STEP1:  ***************************************************************/
                //******************BAM MAT KHAU THEO SALT (CO TRONG CSDL) VA MAT KHAU DUOC NHAP VAO ***********/
                DBConnection.command.Parameters.AddWithValue("@Userid", model.UserId);
                DBConnection.command.CommandText = sqlUserSalt;
                _oldsalt = DBConnection.command.ExecuteScalar() as string;

                model.OldPassword = pwm.IsMatch(model.OldPassword, _oldsalt);

                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParaOuts.Add("IsChanged", 0);

                model.Password = pwm.GetPasswordHashedAndGetSalt(model.Password, out newSalt);


                dicParas.Add("UserId", model.UserId);
                dicParas.Add("OldPassword", model.OldPassword);

                dicParas.Add("OldSalt", _oldsalt);
                dicParas.Add("NewPassword", model.Password);
                dicParas.Add("NewSalt", newSalt);

                output = DBConnection.ExecSPNonQuery("SP_USER_CHANGE_PASSWORD", dicParas, ref dicParaOuts, true);
                output = (int)dicParaOuts["IsChanged"];
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }
            return output;
        }

        public List<EntityUserModel> GetList(int? depId = null)
        {
            List<EntityUserModel> output = new List<EntityUserModel>();

            this.DBConnection.OpenDBConnect();
            //Write log
            if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
            try
            {
                this.DBConnection.command.CommandText = USP_GET_ALL_USERS;
                this.DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@deptId", (object)depId ?? DBNull.Value);


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityUserModel entity = new EntityUserModel();
                            entity.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            entity.UserName = reader.GetString(reader.GetOrdinal("UserName"));
                            entity.Password = reader.GetString(reader.GetOrdinal("Password"));
                            entity.Salt = reader.GetString(reader.GetOrdinal("Salt"));
                            entity.Phone = reader.GetString(reader.GetOrdinal("Phone"));
                            entity.FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? null : reader.GetString(reader.GetOrdinal("FullName"));
                            entity.Email = reader.GetString(reader.GetOrdinal("Email"));
                            entity.DeptId = reader.GetInt32(reader.GetOrdinal("deptID"));
                            entity.DepartName = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name"));
                            entity.Code = reader.IsDBNull(reader.GetOrdinal("Code")) ? null : reader.GetString(reader.GetOrdinal("Code"));
                            entity.IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"));
                            output.Add(entity);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }


            return depId == null ? output : output.ToList();
        }




        public BlockDataUserCreateModel GetEntityById(int id)
        {
            BlockDataUserCreateModel output = new BlockDataUserCreateModel();
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("USERID", id);
                DataSet dataSet = DBConnection.ExecSelectSP("SP_USER_GET_BY_ID", dicParas, ref dicParaOuts, true);
                //**********************TABLE: ROLE***************************************
                DataTable table1 = dataSet.Tables[0];
                foreach (DataRow row in table1.Rows)
                {
                    output.Email = row["Email"].ToString();
                    output.Phone = row["Phone"].ToString();
                    output.IsAdmin = (bool)row["IsAdmin"];
                    output.UserName = (string)row["UserName"];
                    output.FullName = row["FullName"].ToString();
                    output.UserId = (int)row["UserId"];
                    output.DeptId = (int)row["deptID"];
                }
                output.IsSuperAdmin = output.IsAdmin && (output.DeptId == 0);
                //**********************TABLE: ROLEMENU ***********************************************
                DataTable table2 = dataSet.Tables[1];

                foreach (DataRow row in table2.Rows)
                {
                    output.LstSelectedMenu.Add(Int32.Parse(row["menuid"].ToString()));
                }

                foreach (DataRow row in dataSet.Tables[2].Rows)
                {
                    output.LstSelectedRole.Add(Int32.Parse(row["roleid"].ToString()));
                }
                //**********************TABLE: ROLE ***********************************************
                DataTable table3 = dataSet.Tables[2];
                foreach (DataRow row in table3.Rows)
                {
                    BI_Project.Models.EntityModels.EntityRoleModel entityRoleModel = new EntityRoleModel();
                    //entityRoleModel.Name = row["Name"].ToString();
                    entityRoleModel.RoleId = (int)row["RoleId"];

                    output.ListAllRoles.Add(entityRoleModel);
                }
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }
            return output;
        }

        public int Delete(int id)
        {
            int output = 0;

            try
            {

                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/
                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("userid", id);

                //dicParas.Add("")
                output = DBConnection.ExecSPNonQuery("SP_USER_DELETE", dicParas, ref dicParaOuts, true);

                //STEP2:  ***************************************************************/

            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
                output = -1;
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }


            return output;
        }
        public string GetCodeByDeptId(int DeptId)
        {
            string output = "";

            try
            {

                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
                //STEP1:  ***************************************************************/
                DBConnection.command.Parameters.Clear();
                DBConnection.command.CommandText = SP_GET_CODE_BY_DEPTID;
                DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@DEPARTID", DeptId);

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                             output =  reader.IsDBNull(reader.GetOrdinal("Code")) ? null : reader.GetString(reader.GetOrdinal("Code"));

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }


            return output;
        }


        public List<EntityMenuModel> GetAllowedMenuAndRoles(int userId)
        {
            List<EntityMenuModel> _listAllowedAndRoles = new List<EntityMenuModel>();
            DBConnection.OpenDBConnect();
            try
            {
                string sqlSelectMenuAndRoles = "select m.Menuid,m.Path,m.Name from UserMenu um,Menu m where um.Userid = @UserId and um.MenuId =m.MenuId " +
                              "union " +
                             " select m.MenuId,m.Path,m.Name from userRole ur, RoleMenu rm,Menu m " +
                             " where ur.UserId = @UserId and ur.RoleId = rm.RoleId and rm.MenuID = m.MenuID  ";

                DBConnection.command.Parameters.Clear();
                DBConnection.command.CommandText = sqlSelectMenuAndRoles;
                DBConnection.command.Parameters.AddWithValue("@UserId", userId);

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityMenuModel entityMenu = new EntityMenuModel();
                            entityMenu.MenuId = reader.GetInt32(reader.GetOrdinal("MenuId"));
                            entityMenu.Path = reader.IsDBNull(reader.GetOrdinal("Path")) ? "" : "/" + reader.GetString(reader.GetOrdinal("Path")).Trim() + "/" + reader.GetInt32(reader.GetOrdinal("MenuId")).ToString().Trim();
                            entityMenu.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name")).Trim();
                            _listAllowedAndRoles.Add(entityMenu);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }
            return _listAllowedAndRoles;
        }

        public EntityUserModel FindById(int userId)
        {
            EntityUserModel _user = null;
            this.DBConnection.OpenDBConnect();
            try
            {
                string sql = " select * from Users where UserId = @UserId";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = sql;
                DBConnection.command.Parameters.AddWithValue("@UserID", userId);

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            _user = new EntityUserModel()
                            {
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                Email = reader.GetString(reader.GetOrdinal("Email"))
                            };

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }

            return _user;
        }


        public EntityUserModel GetUserDepartment(int userId)
        {
            EntityUserModel output = new EntityUserModel();

            try
            {
                this.DBConnection.OpenDBConnect();
                EntityUserModel entity = new EntityUserModel();

                //Get DepartId
                string userRoleQuery = "SELECT u.UserId, u.Username, u.Phone, u.Email, u.deptID FROM Users u JOIN UserRole ur ON ur.UserId = u.UserId JOIN Role r ON r.RoleId = ur.RoleId WHERE u.UserId = @userId";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = userRoleQuery;
                this.DBConnection.command.Parameters.Add(new SqlParameter("@UserId", userId));
                using (SqlDataReader reader = this.DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            entity.UserId = reader.GetInt32(reader.GetOrdinal("Userid"));
                            entity.UserName = reader.GetString(reader.GetOrdinal("Username"));
                            entity.Phone = reader.GetString(reader.GetOrdinal("Phone"));
                            entity.Email = reader.GetString(reader.GetOrdinal("Email"));
                            entity.DeptId = reader.GetInt32(reader.GetOrdinal("deptID"));
                        }
                    }
                }

                //get DepartmentName
                string userDepartQuery = "SELECT * FROM Department WHERE Id = @DepartId";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = userDepartQuery;
                this.DBConnection.command.Parameters.Add(new SqlParameter("@DepartId", entity.DeptId));

                using (SqlDataReader dataReader = this.DBConnection.command.ExecuteReader())
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            output.DeptId = dataReader.GetInt32(dataReader.GetOrdinal("Id"));
                            output.DepartName = dataReader.GetString(dataReader.GetOrdinal("Name"));
                        }
                    }
                }
                output.UserId = entity.UserId;
                output.UserName = entity.UserName;
                output.Phone = entity.Phone;
                output.Email = entity.Email;

            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }
            return output;
        }
        public int UpdateLogUserLogin(EntityUserTimeModel model)
        {
            int output = 0;

            try
            {
                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("UserId", model.UserId);
                dicParas.Add("UserName", model.UserName);
                dicParas.Add("PKID", model.PKID);
                dicParas.Add("TimeLogin", model.TimeLogin);
                dicParas.Add("Date", model.Date);
                output = DBConnection.ExecSPNonQuery("SP_USER_LOG_LOGIN", dicParas, ref dicParaOuts, true);

                if (DBConnection.ERROR != null)
                    throw new Exception(DBConnection.ERROR);
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
                output = -1;
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }

            return output;
        }
        public int UpdateLogUserLogout(EntityUserTimeModel model)
        {
            int output = 0;

            try
            {
                DBConnection.OpenDBConnect();
                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("PKID", model.PKID);
                output = DBConnection.ExecSPNonQuery("SP_USER_LOG_UPDATE", dicParas, ref dicParaOuts, true);

                if (DBConnection.ERROR != null)
                    throw new Exception(DBConnection.ERROR);
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
                output = -1;
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }

            return output;
        }
        public int UpdateLogUserDashboard(EntityUserTimeModel model)
        {
            int output = 0;

            try
            {
                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("UserId", model.UserId);
                dicParas.Add("PKID", model.PKID);
                dicParas.Add("UserName", model.UserName);
                dicParas.Add("Dashboard", model.Dashboard);
                dicParas.Add("TimeLogin", model.TimeLogin);
                dicParas.Add("Date", model.Date);
                output = DBConnection.ExecSPNonQuery("SP_USER_LOG_DASHBOARD", dicParas, ref dicParaOuts, true);

                if (DBConnection.ERROR != null)
                    throw new Exception(DBConnection.ERROR);
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
                output = -1;
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }

            return output;
        }
    }
}