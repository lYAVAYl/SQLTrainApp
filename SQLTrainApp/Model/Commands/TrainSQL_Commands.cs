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
    }
}
