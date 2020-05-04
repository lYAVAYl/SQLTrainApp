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

        public static List<TestDatabas> GetDatabases()
        {
            List<TestDatabas> testDBs = null;

            try
            {
                using (var context = new TrainSQL_Entities())
                {
                    testDBs = context.TestDatabases.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            return testDBs;
        }

        public static object TaskChecking(string query = "", int dbID = 0)
        {
            object[] res = null;
            if (query != "" && dbID != 0)
            {
                query = query.ToLower().Trim();
                List<TestDatabas> testDBList = new List<TestDatabas>(); // Список имён тестовых БД
                bool isInsert = query.Contains("insert");
                bool isUpdate = query.Contains("update");
                bool isDelete = query.Contains("delete");
                bool isChangingQuery = isInsert || isUpdate || isDelete;
                string mainDB = null; // название БД, по которой строится DataTable
                string currTable = null;
                DataTable resultTable = null; // DataTable для вывода
                string error = null; // Сообщение об ошибке

                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        int shemeID = context.TestDatabases.FirstOrDefault(x => x.dbID == dbID).ShemeID;
                        testDBList = (from x in context.TestDatabases
                                      where x.ShemeID == shemeID
                                      select x).ToList();
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }

                if (testDBList.Count == 0) return null;
                mainDB = testDBList.FirstOrDefault(x => !char.IsDigit(x.dbName.Last())).dbName; // получение названия БД для вывода
                currTable = testDBList.FirstOrDefault(x => x.dbID == dbID).dbName;

                if (mainDB != null)
                {
                    string connectionString;
                    try
                    {
                        if (isChangingQuery)
                        {
                            connectionString = $"Data Source=DESKTOP-5D0552Q;Initial Catalog={mainDB};Integrated Security=True";
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                resultTable = GetDataTable(query, connection, connection.BeginTransaction());

                                connection.Close();
                            }
                        }
                        else
                        {
                            for (int i = 0; i < testDBList.Count && error == null; i++)
                            {
                                connectionString = $"Data Source=DESKTOP-5D0552Q;Initial Catalog={testDBList[i].dbName};Integrated Security=True";
                                DataTable checkingDT;
                                using (SqlConnection connection = new SqlConnection(connectionString))
                                {
                                    connection.Open();

                                    checkingDT = GetDataTable(query, connection, connection.BeginTransaction());

                                    if (testDBList[i].dbName == currTable)
                                    {
                                        resultTable = checkingDT;
                                    }

                                    if (checkingDT == null)
                                    {
                                        error = $"Запрос к таблице {testDBList[i].dbName} не дал результатов";
                                    }

                                    connection.Close();
                                }
                            }
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
            }

            return res;
        }


        public static string OutputDataTable(DataTable dt)
        {
            string result = "";
            if (dt != null)
            {
                foreach (DataColumn column in dt.Columns)
                    result += $"\t{column.ColumnName}";

                result += "\n";
                // перебор всех строк таблицы
                foreach (DataRow row in dt.Rows)
                {
                    // получаем все ячейки строки
                    var cells = row.ItemArray;
                    foreach (object cell in cells)
                        result += $"\t{cell}";
                    result += "\n";
                }
            }

            return result;
        }














        #region CheckQuery
        public static object IsCorrectQuery(string userQuery, TrainSQL_DAL.Task task)
        {
            object res = null;

            string rightQuery = task.RightAnswer.ToLower().Trim();
            userQuery = userQuery.ToLower().Trim();
            List<string> testDBList = new List<string>(); // Список имён тестовых БД
            bool isInsert = rightQuery.Contains("insert");
            bool isUpdate = rightQuery.Contains("update");
            bool isDelete = rightQuery.Contains("delete");
            bool isChangingQuery = isInsert || isUpdate || isDelete;
            string mainDB = null; // название БД, по которой строится DataTable
            DataTable resultTable = null; // DataTable для вывода
            string error = null; // Сообщение об ошибке

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

                        // Получить таблицу вывода
                        resultTable = GetDataTable(userQuery, connection, connection.BeginTransaction());

                        rightTable = GetDataTable(rightQuery, connection, connection.BeginTransaction());
                        if (isChangingQuery)
                        {
                            SqlTransaction transaction = connection.BeginTransaction();
                            string tbName = "";
                            SqlCommand command;

                            command = new SqlCommand(userQuery, connection);
                            command.Transaction = transaction;

                            if (userQuery.Contains("insert")
                                || userQuery.Contains("update")
                                || userQuery.Contains("delete")) command.ExecuteNonQuery();

                            tbName = GetTableName(rightQuery.ToLower());
                            command = new SqlCommand($"SELECT * FROM {tbName}", connection);
                            command.Transaction = transaction;

                            SqlDataReader reader = command.ExecuteReader();
                            userTable.Load(reader);
                            reader.Close();

                            transaction.Rollback();
                        }
                        else
                        {
                            userTable = resultTable;
                        }

                        error = AreTablesTheSame(userTable, rightTable);

                        connection.Close();
                    }

                    if (error != null)
                    {
                        if (isInsert) error = "Операция \"insert\" не выполнена";
                        else if (isUpdate) error = "Операция \"update\" не выполнена";
                        else if (isDelete) error = "Операция \"delete\" не выполнена";
                    }
                    else
                    {
                        if (!isChangingQuery)
                        {
                            int i = 1;
                            for (; i < testDBList.Count() && error == null; i++)
                            {
                                error = CheckQuery(userQuery, rightQuery, testDBList[i]);
                            }

                            if (error != null)
                                error = $"Запрос не прошёл проверку на {i+1} базе данных";
                        }
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
        private static string CheckQuery(string userQuery, string rightQuery, string dbName)
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
        private static string AreTablesTheSame(DataTable userTbl, DataTable rightTbl)
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

        #endregion


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

        public static object DeleteTask(TrainSQL_DAL.Task task = null)
        {
            object result = null;
            if (task != null)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        var delTask = context.Tasks.FirstOrDefault(x => x.TaskID == task.TaskID);
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

        public static object DeleteComplaint(Complaint theme = null)
        {
            object res = null;

            if (theme != null)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        var item = context.Complaints.FirstOrDefault(x => x.ComplaintID == theme.ComplaintID);
                        if (item != null)
                        {
                            context.Complaints.Remove(item);
                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return res;
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
