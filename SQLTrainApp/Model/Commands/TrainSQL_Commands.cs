using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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



        public static Sheme GetDbSheme(int dbID = 0)
        {
            Sheme sheme = null;

            try
            {
                using (var context = new TrainSQL_Entities())
                {
                    TestDatabas testDB = context.TestDatabases.FirstOrDefault(x => x.dbID == dbID);
                    if (testDB != null)
                    {
                        sheme = context.Shemes.FirstOrDefault(x => x.ShemeID == testDB.ShemeID);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return sheme;
        }


        public static object IsCorrectQuery(string userQuery, TrainSQL_DAL.Task task)
        {
            object res = null;

            string rightQuery = task.RightAnswer.ToLower().Trim();
            userQuery = userQuery.ToLower().Trim();
            List<string> testDBList = new List<string>() { "computer", "computer2" }; // Список имён тестовых БД
            bool isChangingQuery = userQuery.Contains("insert")
                                   || userQuery.Contains("update")
                                   || userQuery.Contains("delete");
            string mainDB = null; // название БД, по которой строится DataTable
            DataTable resultTable = null; // DataTable для вывода
            string error = null; // Сообщение об ошибке

            string changingTableName = "";
            if (isChangingQuery) changingTableName = GetTableName(userQuery);

            // Заполнение списка
            try
            {
                using (var context = new TrainSQL_Entities())
                {
                    int shemeID = context.TestDatabases.FirstOrDefault(x => x.dbID == task.dbID).ShemeID;
                    testDBList = (from x in context.TestDatabases
                                  where x.ShemeID == shemeID
                                  select x.dbName).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (testDBList == null || testDBList.Count == 0) return null;
            mainDB = testDBList.FirstOrDefault(x => !char.IsDigit(x.Last())); // получение названия БД для вывода

            if (mainDB != null)
            {
                DataTable userTable = new DataTable();
                DataTable rightTable = new DataTable();


                string connectionString = $"Data Source=DESKTOP-5D0552Q;Initial Catalog={mainDB};Integrated Security=True";
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        userTable = GetDataTable(userQuery, connection, connection.BeginTransaction());
                        rightTable = GetDataTable(rightQuery, connection, connection.BeginTransaction());

                        error = AreTablesTheSame(userTable, rightTable);

                        resultTable = userTable;

                        connection.Close();
                    }

                    if (error == null && !isChangingQuery)
                    {
                        for (int i = 1; i < testDBList.Count() && error == null; i++)
                        {
                            error = CheckQuery(userQuery, rightQuery, testDBList[i]);
                        }

                        if (error != null)
                            error = "Запрос не прошёл проверку на 2 базе данных";
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    error = "Попытка выполнить оперции над удалённым объектом (ObjectDisposedException)!\n" + ex.Message;
                }
                catch (InvalidCastException ex)
                {
                    error = "Недопустимое приведение/преобразование (InvalidCastException)!\n" + ex.Message;
                }
                catch (SqlException ex)
                {
                    error = "Ошибка SQL!\n" + ex.Message;
                }
                catch (InvalidOperationException ex)
                {
                    error = "Состояние объекта блокирует вызов метода (InvalidOperationException)!\n" + ex.Message;
                }
                catch (Exception ex)
                {
                    error = "Ошибка dремени выполнения!" + ex.Message;
                }

            }


            res = new object[] { resultTable, error };


            return res;
        }

        /// <summary>
        /// Получение названия таблицы, в которой происходят изменения
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private static string GetTableName(string query = null)
        {
            string tableName = "";
            if (query != null)
            {
                int i = 0;
                if (query.Contains("insert"))
                {
                    i = query.IndexOf("into");
                    if (i != -1) i += 5;
                }
                else if (query.Contains("update"))
                {
                    i = query.IndexOf("update");
                    if (i != -1) i += 7;
                }
                else if (query.Contains("delete"))
                {
                    i = query.IndexOf("from");
                    if (i != -1) i += 5;
                }

                if (i != -1)
                    for (; Condition(i, query); i++)
                    {
                        tableName += query[i];
                    }

            }

            return tableName;
        }
        /// <summary>
        /// Условие получения символов для названия изменяемой таблицы
        /// </summary>
        /// <param name="index"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static bool Condition(int index, string query)
        {
            return index < query.Length && (char.IsDigit(query[index]) || char.IsLetter(query[index]));
        }

        /// <summary>
        /// Проверка полученного запроса на таблицах
        /// </summary>
        /// <param name="userQuery">Запрос пользователя</param>
        /// <param name="rightQuery">Правильный запрос</param>
        /// <param name="dbName">Название </param>
        /// <returns></returns>
        public static string CheckQuery(string userQuery, string rightQuery, string dbName)
        {
            string result = null;
            DataTable userTable = new DataTable();
            DataTable rightTable = new DataTable();

            string connectionString = $"Data Source=DESKTOP-5D0552Q;Initial Catalog={dbName};Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                userTable = GetDataTable(userQuery, connection, connection.BeginTransaction());
                rightTable = GetDataTable(rightQuery, connection, connection.BeginTransaction());

                result = AreTablesTheSame(userTable, rightTable);

                connection.Close();
            }


            return result;
        }

        static DataTable GetDataTable(string query, SqlConnection connection, SqlTransaction transaction = null)
        {
            DataTable resultDT = new DataTable();
            string tbName = "";
            SqlCommand command;
            bool isTransactionOn = transaction != null;


            query = query.ToLower();
            bool isChangingQuery = query.Contains("insert")
                                   || query.Contains("update")
                                   || query.Contains("delete");

            command = new SqlCommand(query, connection);
            command.Transaction = transaction;

            if (isTransactionOn && isChangingQuery)
            {
                command.ExecuteNonQuery();

                tbName = GetTableName(query.ToLower());
                command = new SqlCommand($"SELECT * FROM {tbName}", connection);
                command.Transaction = transaction;
            }

            SqlDataReader reader = command.ExecuteReader();
            resultDT.Load(reader);
            reader.Close();

            transaction.Rollback();


            return resultDT;
        }

        /// <summary>
        /// Проверка полученного результата с ожидаемым
        /// </summary>
        /// <param name="userTbl">Результат пользователя</param>
        /// <param name="rightTbl">Ожидаемый результат</param>
        /// <returns></returns>
        public static string AreTablesTheSame(DataTable userTbl, DataTable rightTbl)
        {
            string error = "";
            int dif;

            if (userTbl.Columns.Count != rightTbl.Columns.Count)
            {
                error += "\nНеверное количество столбцов. ";

                dif = Math.Abs(userTbl.Columns.Count - rightTbl.Columns.Count);

                if (userTbl.Columns.Count > rightTbl.Columns.Count)
                {
                    error += "Больше на " + dif;
                }
                else error += "Меньше на " + dif;
            }

            if (userTbl.Rows.Count != rightTbl.Rows.Count)
            {
                error += "\nНеверное количество элементов. ";

                dif = Math.Abs(userTbl.Rows.Count - rightTbl.Rows.Count);

                if (userTbl.Rows.Count > rightTbl.Rows.Count)
                {
                    error += "Больше на " + dif;
                }
                else error += "Меньше на " + dif;

            }


            if (error == "")
            {
                for (int i = 0; i < userTbl.Rows.Count; i++)
                {
                    for (int c = 0; c < userTbl.Columns.Count; c++)
                    {
                        if (!Equals(userTbl.Rows[i][c], rightTbl.Rows[i][c]))
                            error = "\nВыбраны не правильные элементы";
                    }
                }
            }

            return error == "" ? null : error;
        }
















        #region User
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
                using (var context = new TrainSQL_Entities())
                {
                    try
                    {
                        context.Users.Add(newUser);
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
                            userProgress.Reverse();
                            userProgress = userProgress.Take(15).ToList();
                            userProgress.Reverse();

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }

            return userProgress;
        }

        public static object AddUserProgress(string login = "", int rigthAnswers = 0, int totalQuestions = 0)
        {
            object res = null;
            if (login != "")
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        if (IsUserExists(login))
                        {
                            Progress progress = new Progress()
                            {
                                Login = login,
                                TestDate = DateTime.Now,
                                RightAnswersQuantity = rigthAnswers,
                                TotalQuastionsQuantity = totalQuestions
                            };

                            if (context.Progresses.FirstOrDefault(x => x.Login == login) != null)
                            {
                                context.Progresses.Add(progress);
                            }
                            else
                            {
                                context.Progresses.Add(new Progress()
                                {
                                    Login = login,
                                    TestDate = DateTime.Now.AddMinutes(-1),
                                    RightAnswersQuantity = 0,
                                    TotalQuastionsQuantity = totalQuestions
                                });
                                context.Progresses.Add(progress);

                            }

                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    res = ex;
                }
            }

            return res;
        }
        #endregion


        #region Themes
        public static bool IsThemeExists(int themeID = 0)
        {
            bool isExists = false;
            if (themeID != 0)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        isExists = context.Themes.FirstOrDefault(x => x.ThemeID == themeID) != null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return isExists;
        }

        public static object EditORAddTheme(Theme newTheme = null)
        {
            object res = null;
            if (newTheme != null)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
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
                catch (Exception ex)
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

        public static Theme GetThemeByThemeID(int themeID)
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

        public static int GetThemeIndex(int themeID = 0)
        {
            int index = 0;
            if (themeID != 0)
            {
                using (var context = new TrainSQL_Entities())
                {
                    index = context.Themes.ToList().FindIndex(x => x.ThemeID == themeID);
                }
            }

            return index;
        }

        public static object DeleteTheme(Theme theme = null)
        {
            object result = null;
            if (theme != null)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        var delTheme = context.Themes.FirstOrDefault(x => x.ThemeID == theme.ThemeID);
                        if (delTheme != null) context.Themes.Remove(delTheme);
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    result = ex;
                }
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
        #endregion


        #region Tasks

        public static List<TrainSQL_DAL.Task> GetTestList(int count = 0)
        {
            List<TrainSQL_DAL.Task> result = null;
            if (count > 0)
            {
                result = new List<TrainSQL_DAL.Task>();
                try
                {
                    using (var context = new TrainSQL_Entities())
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

        public static bool IsTaskExists(int taskID = 0)
        {
            bool isExists = false;
            if (taskID != 0)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        isExists = context.Tasks.FirstOrDefault(x => x.TaskID == taskID) != null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return isExists;
        }

        public static object DeleteTask(TrainSQL_DAL.Task theme = null)
        {
            object result = null;
            if (theme != null)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        var delTask = context.Tasks.FirstOrDefault(x => x.TaskID == theme.TaskID);
                        if (delTask != null) context.Tasks.Remove(delTask);
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    result = ex;
                }
            }

            return result;
        }

        public static object EditORAddTask(TrainSQL_DAL.Task newTask = null)
        {
            object res = null;
            if (newTask != null)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        if (IsTaskExists(newTask.TaskID))
                        {
                            context.Tasks.FirstOrDefault(x => x.TaskID == newTask.TaskID).TaskText = newTask.TaskText;
                            context.Tasks.FirstOrDefault(x => x.TaskID == newTask.TaskID).RightAnswer = newTask.RightAnswer;
                            context.Tasks.FirstOrDefault(x => x.TaskID == newTask.TaskID).dbID = newTask.dbID;
                        }
                        else
                        {
                            context.Tasks.Add(new TrainSQL_DAL.Task()
                            {
                                TaskText = newTask.TaskText,
                                RightAnswer = newTask.RightAnswer,
                                dbID = newTask.dbID
                            });
                        }
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    res = ex;
                    MessageBox.Show(ex.ToString());
                }
            }

            return res;
        }

        public static TrainSQL_DAL.Task GetTaskByID(int taskID = 0)
        {
            TrainSQL_DAL.Task task = null;
            if (taskID != 0)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        task = context.Tasks.FirstOrDefault(x => x.TaskID == taskID);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return task;
        }
        #endregion


        #region Complaints
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
                catch (Exception ex)
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

        public static List<Complaint> GetComplaintsByLogin(string login = "")
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

        public static List<Complaint> GetComplaintsByTask(int taskID = 0)
        {
            List<Complaint> result = null;
            if (taskID != 0)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        result = context.Complaints.Where(x => x.TaskID == taskID).ToList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return result;
        }
        #endregion

    }
}
