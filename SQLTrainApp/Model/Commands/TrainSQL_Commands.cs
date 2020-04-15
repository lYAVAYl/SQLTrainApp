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
                if (login != "")
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        userExists = context.Users.First(x => x.Login == login) != null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return userExists;
        }

        public static User FindUser(string login = "")
        {
            User user = null;
            try
            {
                if (login != "")
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        user = context.Users.FirstOrDefault(x => x.Login == login);
                    }
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
