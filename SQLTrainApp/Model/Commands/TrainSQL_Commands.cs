using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TrainSQL_DAL;

namespace SQLTrainApp.Model.Commands
{
    public static class TrainSQL_Commands
    {
        public static bool IsUserExists(string login = "")
        {
            bool userExists = false;
            try
            {
                using (var context = new TrainSQL_Entities())
                {
                    userExists = context.Users.FirstOrDefault(x => x.Login == login) != null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return userExists;
        }
        public static bool IsEmailRegistered(string email = "")
        {
            bool emailRegistered = false;
            try
            {
                using (var context = new TrainSQL_Entities())
                {
                    emailRegistered = context.Users.FirstOrDefault(x => x.UserEmail == email) != null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return emailRegistered;
        }

        public static User FindUser(string login = "")
        {
            User user = null;
            try
            {
                using (var context = new TrainSQL_Entities())
                {
                    user = context.Users.FirstOrDefault(x => x.Login == login);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return user;
        }

        public static object AddUser(User newUser = null)
        {
            if (newUser != null)
            {
                using(var context = new TrainSQL_Entities())
                {
                    try
                    {
                        context.Users.Add(newUser);
                        context.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return ex;
                    }
                }

                return null;
            }

            return new ArgumentNullException();
        }

        public static string GetUserRole(User user)
        {
            string roleName = "";
            try
            {
                if (user != null)
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        roleName = context.Roles.FirstOrDefault(x => x.RoleID == user.RoleID).RoleName;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return roleName;
        }

        public static object ChangeUserProperty(User user = null, string columnName = "", string newProperty = "")
        {
            if (user != null)
            {
                using (var context = new TrainSQL_Entities())
                {
                    try
                    {
                        switch (columnName)
                        {
                            case nameof(user.Login):
                                context.Users.FirstOrDefault(x => x.Login == user.Login).Login = newProperty;
                                break;
                            case nameof(user.Password):
                                context.Users.FirstOrDefault(x => x.Login == user.Login).Password = newProperty;
                                break;
                            case nameof(user.RoleID):
                                context.Users.FirstOrDefault(x => x.Login == user.Login).RoleID = Convert.ToInt32(newProperty);
                                break;
                            default:
                                return new Exception();
                                break;
                        }

                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return ex;
                    }
                }

                return null;
            }

            return new ArgumentNullException();
        }
    }
}
