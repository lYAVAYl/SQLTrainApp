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
        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        private static SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = "DESKTOP-L3ANRNO\\SQLEXPRESS01",
            IntegratedSecurity = true
        };

        /// <summary>
        /// Проверка на существование пользователя по логину
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns></returns>
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

        /// <summary>
        /// Проверка, зарегистрирован ли пользователь с таким email
        /// </summary>
        /// <param name="email">email пользователя</param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить 
        /// </summary>
        /// <param name="dbID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить список тестовых баз данных
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Проверка задания
        /// </summary>
        /// <param name="query">Запрос пользователя</param>
        /// <param name="dbID">ID тестовой базы данных, к который нужно выполнить запрос</param>
        /// <returns></returns>
        public static object TaskChecking(string query = "", int dbID = 0)
        {
            // Результат теста запроса
            object[] res = null;

            // Проверка, не является ли запрос пустой строкой
            if (query != "" && dbID != 0)
            {
                query = query.ToLower().Trim();

                // Список имён тестовых БД
                List<TestDatabas> testDBList = new List<TestDatabas>(); 

                // Есть ли в запросе слово для редактирования таблицы
                bool isInsert = query.Contains("insert");
                bool isUpdate = query.Contains("update");
                bool isDelete = query.Contains("delete");

                // Условие, изменят ли запрос пользователя таблицу или нет
                bool isChangingQuery = isInsert || isUpdate || isDelete;

                // Название БД, по которой строится DataTable
                string mainDB = null;

                // Текущая таблица
                string currTable = null;

                // DataTable для вывода
                DataTable resultTable = null;

                // Сообщение об ошибке
                string error = null; 

                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        // ID схемы тестовой БД
                        int shemeID = context.TestDatabases.FirstOrDefault(x => x.dbID == dbID).ShemeID;

                        // Список тестовых БД
                        // есть несколько БД с одинаковой схемой, но разными данными,
                        // что позволяет проверять корректность запроса, а не сам результат его выполнения
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
                // Получить название БД для вывода результата пользователю
                mainDB = testDBList.FirstOrDefault(x => !char.IsDigit(x.dbName.Last())).dbName;
                currTable = testDBList.FirstOrDefault(x => x.dbID == dbID).dbName;

                if (mainDB != null)
                {
                    try
                    {
                        // Проверка, меняет ли запрос таблицу в БД
                        if (isChangingQuery)
                        {
                            connectionBuilder.InitialCatalog = mainDB;
                            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
                            {
                                connection.Open();

                                resultTable = GetDataTable(query, connection);

                                connection.Close();
                            }
                        }
                        else
                        {
                            for (int i = 0; i < testDBList.Count && error == null; i++)
                            {
                                connectionBuilder.InitialCatalog = testDBList[i].dbName;

                                DataTable checkingDT;
                                using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
                                {
                                    connection.Open();

                                    checkingDT = GetDataTable(query, connection);

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
                        error = "Ошибка времени выполнения!" + ex.Message;
                    }

                }

                res = new object[] { resultTable, error };
            }

            return res;
        }
        



        #region CheckQuery
        /// <summary>
        /// Проверка запроса пользователя с ожидаемым 
        /// </summary>
        /// <param name="userQuery"></param>
        /// <param name="task"></param>
        /// <returns></returns>
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

            if (userQuery.Length != 0)
            {
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

                    connectionBuilder.InitialCatalog = mainDB;
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
                        {
                            connection.Open();

                            // Получить таблицу вывода
                            resultTable = GetDataTable(userQuery, connection);
                            // Получить ожидаемую тублицу
                            rightTable = GetDataTable(rightQuery, connection);

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
                            if (isInsert) error = "Операция \"insert\" выполнена неверно или вовсе не выполнена";
                            else if (isUpdate) error = "Операция \"update\" выполнена неверно или вовсе не выполнена";
                            else if (isDelete) error = "Операция \"delete\" выполнена неверно или вовсе не выполнена";
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
                                    error = $"Запрос не прошёл проверку на {i + 1} базе данных";
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
            // DataTable пользователя
            DataTable userTable = new DataTable();
            // DataTable от правильного запроса
            DataTable rightTable = new DataTable();

            connectionBuilder.InitialCatalog = dbName;
            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                connection.Open();

                userTable = GetDataTable(userQuery, connection);
                rightTable = GetDataTable(rightQuery, connection);

                result = AreTablesTheSame(userTable, rightTable);

                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Получить результат запроса в виде таблицы
        /// </summary>
        /// <param name="query">Запрос</param>
        /// <param name="connection">Строка подключения</param>
        /// <param name="transaction">Транзакция</param>
        /// <returns></returns>
        static DataTable GetDataTable(string query, SqlConnection connection)
        {
            DataTable resultDT = new DataTable(); // Результирующая таблица
            string tbName = ""; // Название таблицы, к которой выполняется запрос
            SqlCommand command; // Команда выборки
            SqlTransaction transaction = null; // Транзакция
            query = query.ToLower(); // Перевод запроса в нижний регистр
            // Проверка, стоит ли выполнять запрос в транзакции
            bool isChangingQuery = query.Contains("insert")
                                   || query.Contains("update")
                                   || query.Contains("delete");

            command = new SqlCommand(query, connection);

            // Запрос менят таблицу в БД -> проверка через транзакцию
            if (isChangingQuery)
            {
                transaction = connection.BeginTransaction(); // Новая транзакция
                command.Transaction = transaction; // Присвоение транзакции

                command.ExecuteNonQuery(); // Выполнение изменения таблицы

                tbName = GetTableName(query); // Получение названия отредактированной таблицы
                command = new SqlCommand($"SELECT * FROM {tbName}", connection);
                command.Transaction = transaction;
            }

            SqlDataReader reader = command.ExecuteReader(); // Выполнение выборки
            resultDT.Load(reader); // Загрузка результатов в таблицу
            reader.Close();

            transaction?.Rollback(); // Откат транзакции

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

            // Сравнение кол-ва столбцов запроса пользователя с ожидаемым кол-вом столбцов
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

            // Сравнение кол-ва строк запроса пользователя с ожидаемым кол-вом строк
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

            // Если ошибки нет, значит кол-во столбцов и строк совпадает
            if (error == "")
            {
                // Проверка самих элементов полученного запроса с ожидаемыми элементами
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
        /// <summary>
        /// Посик пользователя по логину
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns></returns>
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

        /// <summary>
        /// Добавление польхователя
        /// </summary>
        /// <param name="newUser">Новый пользователь</param>
        /// <returns></returns>
        public static object AddUser(User newUser = null)
        {
            if (newUser != null)
            {
                using (var context = new TrainSQL_Entities())
                {
                    try
                    {
                        context.Users.Add(newUser);
                        AddUserProgress(newUser.Login, 0, 5);
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

        /// <summary>
        /// Роль пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить прогресс пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static List<Progress> GetUserProgress(string login = "")
        {
            List<Progress> userProgress = null;

            if (login != "")
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        if (context.Progresses.FirstOrDefault(x => x.Login == login) != null)
                        {
                            if (context.Progresses.Where(x => x.Login == login).Count() > 14)
                            {
                                userProgress = (from el in context.Progresses
                                                where el.Login == login
                                                orderby el.TestDate descending
                                                select el).Take(15).ToList();
                                userProgress.Reverse();
                            }
                            else
                            {
                                userProgress = (from el in context.Progresses
                                                where el.Login == login
                                                orderby el.TestDate
                                                select el).ToList();
                            }
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

        /// <summary>
        /// Добавить прогресс пользователю
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="rigthAnswers">Кол-во верно решённых заданий</param>
        /// <param name="totalQuestions">Общее число заданий в тесте</param>
        /// <returns></returns>
        public static object AddUserProgress(string login = "", int rigthAnswers = 0, int totalQuestions = 0)
        {
            object res = null;
            if (login != "")
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        Progress progress = new Progress()
                        {
                            Login = login,
                            TestDate = DateTime.Now,
                            RightAnswersQuantity = rigthAnswers,
                            TotalQuastionsQuantity = totalQuestions
                        };

                        context.Progresses.Add(progress);


                        context.SaveChanges();

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
        /// <summary>
        /// Существует ли тема
        /// </summary>
        /// <param name="themeID">ID темы</param>
        /// <returns></returns>
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

        /// <summary>
        /// Редактировать/Добавить тему
        /// </summary>
        /// <param name="newTheme">Новая тема</param>
        /// <returns></returns>
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

        /// <summary>
        /// Показать тему
        /// </summary>
        /// <param name="index">Индекс темы</param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить тему по ID
        /// </summary>
        /// <param name="themeID">ID темы</param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить индекс темы
        /// </summary>
        /// <param name="themeID">ID темы</param>
        /// <returns></returns>
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

        /// <summary>
        /// Удалить тему
        /// </summary>
        /// <param name="theme">Тема</param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить список тем
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Получить список случайных заданий для теста
        /// </summary>
        /// <param name="count">Кол-во заданий в тесте</param>
        /// <returns></returns>
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
                        // Случайный номер задания
                        int rand = new Random().Next(context.Tasks.ToList().Count());
                        // Само задание
                        TrainSQL_DAL.Task task = context.Tasks.ToList()[rand];
                        // Добавить задание в список заданий
                        result.Add(task);

                        while (result.Count < 5)
                        {
                            // Получить новое задание
                            rand = new Random().Next(context.Tasks.ToList().Count());
                            task = context.Tasks.ToList()[rand];

                            // Если задания нет в списке, то добавить
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

        /// <summary>
        /// Получить список всех заданий
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Существет ли задание
        /// </summary>
        /// <param name="taskID">ID задания</param>
        /// <returns></returns>
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

        /// <summary>
        /// Удалить задание
        /// </summary>
        /// <param name="task">Задание</param>
        /// <returns></returns>
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

        /// <summary>
        /// Редактировать/Добавить задание
        /// </summary>
        /// <param name="newTask">Задание</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Получить задание по ID
        /// </summary>
        /// <param name="taskID">ID задания</param>
        /// <returns></returns>
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
        /// <summary>
        /// Отправить жалобу
        /// </summary>
        /// <param name="complaint">Жалоба</param>
        /// <returns></returns>
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

        /// <summary>
        /// Удалить жалобу
        /// </summary>
        /// <param name="complaint">Жалоба</param>
        /// <returns></returns>
        public static object DeleteComplaint(Complaint complaint = null)
        {
            object res = null;

            if (complaint != null)
            {
                try
                {
                    using (var context = new TrainSQL_Entities())
                    {
                        var item = context.Complaints.FirstOrDefault(x => x.ComplaintID == complaint.ComplaintID);
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

        /// <summary>
        /// Получить список всех жалоб
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Получить список жалоб от 1 пользователя
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить жалобы по заданию
        /// </summary>
        /// <param name="taskID">ID задания</param>
        /// <returns></returns>
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
