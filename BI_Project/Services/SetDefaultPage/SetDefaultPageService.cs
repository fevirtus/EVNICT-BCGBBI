using BI_Project.Models.EntityModels;
using BI_Project.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BI_SUN.Services.SetDefaultPage
{
    public class SetDefaultPageService : DBBaseService
    {
        public SetDefaultPageService(DBConnection dBConnection) : base(dBConnection)
        {

        }

        public List<EntityUserMenuModel> GetListDefaultPage(int userId)
        {
            List<EntityUserMenuModel> _listAllDefaultPage = new List<EntityUserMenuModel>();
            DBConnection.OpenDBConnect();
            try
            {

                string sqlSelectListDefaultPage = "SELECT  UserId, a.[MenuId],b.Path,b.Name,[IsDefaultPage] FROM [dbo].[UserMenu] a inner join [dbo].[Menu] b ON a.MenuId = b.MenuId and b.Path is not null and UserId = @UserId";

                DBConnection.command.CommandText = sqlSelectListDefaultPage;
                DBConnection.command.Parameters.AddWithValue("@UserId", userId);
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityUserMenuModel entityUserMenu = new EntityUserMenuModel();
                            entityUserMenu.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            entityUserMenu.MenuId = reader.GetInt32(reader.GetOrdinal("MenuId"));
                            entityUserMenu.Path = reader.GetString(reader.GetOrdinal("Path")).ToString().Trim();
                            entityUserMenu.Name = reader.GetString(reader.GetOrdinal("Name")).ToString().Trim();
                            entityUserMenu.IsDefaultPage = reader.IsDBNull(reader.GetOrdinal("IsDefaultPage")) ? false : reader.GetBoolean(reader.GetOrdinal("IsDefaultPage"));

                            _listAllDefaultPage.Add(entityUserMenu);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }

            return _listAllDefaultPage;
        }

        // Update IsDefaultPage = true
        public void UpdatePageDefault(int userId, int menuId)
        {
            DBConnection.OpenDBConnect();
            try
            {
                bool isDefault = true;
                string sqlUpdateIsDefaultPage = "Update UserMenu set IsDefaultPage = 'false' Where UserId =@UserId";
                DBConnection.command.CommandText = sqlUpdateIsDefaultPage;
                DBConnection.command.Parameters.AddWithValue("@UserId", userId);
                DBConnection.command.ExecuteNonQuery();
                DBConnection.command.Parameters.Clear();

                string sqlUpdatePage = "Update  UserMenu set IsDefaultPage= @IsDefaultPage where MenuId = @MenuId and UserId = @UserId";
                DBConnection.command.CommandText = sqlUpdatePage;
                DBConnection.command.Parameters.AddWithValue("@UserId", userId);
                DBConnection.command.Parameters.AddWithValue("@MenuId", menuId);


                DBConnection.command.Parameters.AddWithValue("@IsDefaultPage", isDefault);

                DBConnection.command.ExecuteNonQuery();
                DBConnection.command.Parameters.Clear();
            }

            catch (Exception ex)
            {
                string Error = ex.ToString();
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }
        }


        public List<EntityDepartmentModel> GetListDepartments(string strMaDV, int cap = 2)
        {
            List<EntityDepartmentModel> _listAll = new List<EntityDepartmentModel>();
            DBConnection.OpenDBConnect();
            try
            {

                string sqlSelectListKH = "SELECT  * from Department where Code like '" + strMaDV + "%'";
                if (cap == 1)  //chỉ lấy 1 cấp
                {
                    sqlSelectListKH += " and Code <> 'P'";
                }
                DBConnection.command.CommandText = sqlSelectListKH;
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityDepartmentModel entityUserMenu = new EntityDepartmentModel();
                            entityUserMenu.DepartId = reader.GetInt32(reader.GetOrdinal("Id"));
                            entityUserMenu.Name = reader.GetString(reader.GetOrdinal("Name")).ToString().Trim();
                            entityUserMenu.Filter01 = reader.GetString(reader.GetOrdinal("Filter01")).ToString().Trim();
                            entityUserMenu.Code = reader.GetString(reader.GetOrdinal("Code")).ToString().Trim();
                            _listAll.Add(entityUserMenu);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }

            return _listAll;
        }



        public void UpdateQuanLyMau(List<EntityQuanLyMau> model)
        {
            DBConnection.OpenDBConnect();
            try
            {
                string sqlSelectListKH = "MERGE LRS_STRATUMDESIGN AS dangky ";
                sqlSelectListKH += " USING (SELECT @DEPARTMENTID DEPARTMENTID,@PROFILEID PROFILEID,@STRATUMID STRATUMID,@YEAR_LRS YEAR_LRS, ";
                sqlSelectListKH += " @DESIGNPOPN DESIGNPOPN,@DESIGNNSAMPLE DESIGNNSAMPLE,@USERCOMMENT USERCOMMENT, @DESIGNPOPENERGY DESIGNPOPENERGY) AS dulieu ";
                sqlSelectListKH += " ON dangky.[DEPARTMENTID] = dulieu.DEPARTMENTID AND dangky.[PROFILEID] = dulieu.PROFILEID ";
                sqlSelectListKH += " AND dangky.[STRATUMID] = dulieu.STRATUMID AND dangky.[YEAR_LRS] = dulieu.YEAR_LRS ";
                sqlSelectListKH += " WHEN MATCHED THEN UPDATE SET ";
                sqlSelectListKH += " dangky.DESIGNPOPN = dulieu.DESIGNPOPN, dangky.DESIGNNSAMPLE = dulieu.DESIGNNSAMPLE, ";
                sqlSelectListKH += " dangky.DESIGNPOPENERGY = dulieu.DESIGNPOPENERGY, dangky.USERCOMMENT = dulieu.USERCOMMENT ";
                sqlSelectListKH += " WHEN NOT MATCHED THEN ";
                sqlSelectListKH += " INSERT (DEPARTMENTID,PROFILEID,STRATUMID,YEAR_LRS,DESIGNPOPN,DESIGNPOPENERGY,DESIGNNSAMPLE,USERCOMMENT) ";
                sqlSelectListKH += " VALUES(dulieu.DEPARTMENTID,dulieu.PROFILEID,dulieu.STRATUMID,dulieu.YEAR_LRS,dulieu.DESIGNPOPN,dulieu.DESIGNPOPENERGY,dulieu.DESIGNNSAMPLE,dulieu.USERCOMMENT);";

                DBConnection.command.CommandText = sqlSelectListKH;

                foreach (var mau in model)
                {
                    DBConnection.command.Parameters.Clear();
                    DBConnection.command.Parameters.AddWithValue("@DEPARTMENTID", mau.DEPARTMENTID);
                    DBConnection.command.Parameters.AddWithValue("@PROFILEID", mau.PROFILEID);  
                    DBConnection.command.Parameters.AddWithValue("@STRATUMID", "A");
                    DBConnection.command.Parameters.AddWithValue("@YEAR_LRS", mau.YEAR_LRS);
                    DBConnection.command.Parameters.AddWithValue("@DESIGNPOPN", mau.DESIGNPOPN);
                    DBConnection.command.Parameters.AddWithValue("@DESIGNPOPENERGY", mau.DESIGNPOPENERGY);
                    DBConnection.command.Parameters.AddWithValue("@DESIGNNSAMPLE", mau.DESIGNNSAMPLE);
                    DBConnection.command.Parameters.AddWithValue("@USERCOMMENT", mau.USERCOMMENT == null ? " ": mau.USERCOMMENT);                    
                    DBConnection.command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }
            
        }

        public List<EntityQuanLyMau> GetListQuanLyMau(string MaDV, int iNam = 0)
        {
            List<EntityQuanLyMau> _listAll = new List<EntityQuanLyMau>();
            DBConnection.OpenDBConnect();
            try
            {
                string sqlSelectListKH = "SELECT ISNULL(d.DEPARTMENTID, @DEPARTMENTID ) DEPARTMENTID, l.PROFILEID PROFILEID, ";
                sqlSelectListKH += " ISNULL(d.STRATUMID, 'A' ) STRATUMID, @YEAR_LRS YEAR_LRS, ISNULL(d.DESIGNPOPN, 0) DESIGNPOPN, ISNULL(d.DESIGNPOPENERGY, 0 ) DESIGNPOPENERGY,";
                sqlSelectListKH += " ISNULL(d.DESIGNNSAMPLE, 0) DESIGNNSAMPLE, ISNULL(d.USERCOMMENT, '' ) USERCOMMENT";
                sqlSelectListKH += " FROM  LRS_PROFILELIST l LEFT JOIN (SELECT * FROM LRS_STRATUMDESIGN WHERE DEPARTMENTID = @DEPARTMENTID AND year_lrs = @YEAR_LRS) d ON  d.PROFILEID = l.PROFILEID ";
                sqlSelectListKH += " where PROFILE_TYPE = 'NHOM' ORDER BY DISPORDER";
                DBConnection.command.CommandText = sqlSelectListKH;
                DBConnection.command.Parameters.AddWithValue("@DEPARTMENTID", MaDV);
                DBConnection.command.Parameters.AddWithValue("@YEAR_LRS", iNam);
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityQuanLyMau entityMau = new EntityQuanLyMau();
                            entityMau.DEPARTMENTID = reader.GetString(reader.GetOrdinal("DEPARTMENTID"));
                            entityMau.PROFILEID = reader.GetString(reader.GetOrdinal("PROFILEID"));
                            entityMau.STRATUMID = reader.GetString(reader.GetOrdinal("STRATUMID"));
                            entityMau.YEAR_LRS = reader.GetInt32(reader.GetOrdinal("YEAR_LRS"));
                            entityMau.DESIGNPOPN = reader.GetInt32(reader.GetOrdinal("DESIGNPOPN"));
                            entityMau.DESIGNPOPENERGY = reader.GetDecimal(reader.GetOrdinal("DESIGNPOPENERGY"));
                            entityMau.DESIGNNSAMPLE = reader.GetInt32(reader.GetOrdinal("DESIGNNSAMPLE"));
                            entityMau.USERCOMMENT = reader.IsDBNull(reader.GetOrdinal("USERCOMMENT")) ? "" : reader.GetString(reader.GetOrdinal("USERCOMMENT"));
                            _listAll.Add(entityMau);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }

            return _listAll;
        }



    }

}



