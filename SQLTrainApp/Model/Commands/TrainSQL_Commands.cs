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

        public static List<Progress> GetUserProgress(string login = "")
        {
            List<Progress> userProgress = null;

            if (login != "")
            {
                if (IsUserExists(login))
                {
                    try
                    {
                        using (var context = new TrainSQL_Entities())
                        {
                            userProgress = (from el in context.Progresses
                                            where el.Login == login
                                            orderby el.TestDate
                                            select el).ToList();

                            if (userProgress.Count > 14)
                            {
                                userProgress = userProgress.Take(15).ToList();
                            }
                                
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }

            }

            return userProgress;
        }

        public static bool IsThemeExists(int themeID = 0)
        {
            bool isExists = false;
            if (themeID != 0)
            {
                try
                {
                    using(var context = new TrainSQL_Entities())
                    {
                        isExists = context.Themes.FirstOrDefault(x => x.ThemeID == themeID) != null;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return isExists;
        }

        public static Theme GetTheme(int themeID)
        {
            Theme theme = null;
            if (themeID != 0
                && IsThemeExists(themeID))
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        theme = context.Themes.FirstOrDefault(x => x.ThemeID == themeID);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return theme;

        }

        public static object EditORAddTheme(Theme newTheme = null)
        {
            object res = null;
            if (newTheme != null)
            {
                try
                {
                    using(var context = new TrainSQL_Entities())
                    {
                        if (IsThemeExists(newTheme.ThemeID))
                        {
                            context.Themes.FirstOrDefault(x => x.ThemeID == newTheme.ThemeID).ThemeName = newTheme.ThemeName;
                            context.Themes.FirstOrDefault(x => x.ThemeID == newTheme.ThemeID).Theory = newTheme.Theory;
                        }
                        else
                        {
                            context.Themes.Add(newTheme);
                        }
                        context.SaveChanges();
                    }
                }
                catch(Exception ex)
                {
                    res = ex;
                    MessageBox.Show(ex.ToString());
                }
            }

            return res;
        }

        public static Theme ShowTheme(int index = -1)
        {
            Theme theme = null;
            if (index > -1)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        if (index < context.Themes.Count())
                            theme = context.Themes.ToList()[index];
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return theme;
        }

        public static object SendComplaint(Complaint complaint = null)
        {
            if (complaint != null)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        complaint.ComplaintID = context.Complaints.ToList().Last().ComplaintID + 1;
                        context.Complaints.Add(complaint);
                        context.SaveChanges();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return ex;
                }
            }

            return null;
        }

        public static List<Complaint> GetAllComplaints()
        {
            List<Complaint> result = null;
            try
            {
                using (var context = new TrainSQL_Entities())
                {
                    result = context.Complaints.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }

        public static List<Complaint> GetUsersCompliants(string login = "")
        {
            List<Complaint> result = null;
            if (login != "")
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        result = context.Complaints.Where(x => x.Login == login).ToList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return result;
        }

        public static List<TrainSQL_DAL.Task> GetAllTasks()
        {
            List<TrainSQL_DAL.Task> result = null;

            try
            {
                using (var context = new TrainSQL_Entities())
                {
                    result = context.Tasks.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }

        public static List<Theme> GetAllThemes()
        {
            List<Theme> result = null;

            try
            {
                using (var context = new TrainSQL_Entities())
                {
                    result = context.Themes.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }

        public static List<TrainSQL_DAL.Task> GetTestList(int count = 0)
        {
            List<TrainSQL_DAL.Task> result = null;
            if(count > 0)
            {
                result = new List<TrainSQL_DAL.Task>();
                try
                {
                    using(var context = new TrainSQL_Entities())
                    {
                        int rand = new Random().Next(context.Tasks.ToList().Count());
                        TrainSQL_DAL.Task task = context.Tasks.ToList()[rand];
                        result.Add(task);

                        while (result.Count < 5)
                        {
                            rand = new Random().Next(context.Tasks.ToList().Count());
                            task = context.Tasks.ToList()[rand];

                            if (!result.Contains(task))
                                result.Add(task);
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return result;
        }

        public static Sheme GetDbSheme(int dbID = 0)
        {
            Sheme sheme = null;

            try
            {
                using(var context = new TrainSQL_Entities())
                {
                    TestDatabas testDB = context.TestDatabases.FirstOrDefault(x => x.dbID == dbID);
                    if (testDB != null)
                    {
                        sheme = context.Shemes.FirstOrDefault(x => x.ShemeID == testDB.ShemeID);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return sheme;
        }
    }
}
