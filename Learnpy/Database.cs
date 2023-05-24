using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy
{
    public class Database
    {
        public static User User { get; set; } = new User() { Id = -1, Group = "-", Name = "Тестировщик" };
        public static string ServerName;

        public static void SetServer(string servName)
        {
            try {
                CloseConnection();
                ConnectionString = $"Data Source={servName};Initial Catalog=LearnpyDB;Integrated Security=True";
                OpenConnection();
            }
            catch {
                throw new Exception("Не удалось соединится с сервером. Проверьте название указанного сервера на наличие ошибки или подключитесь к сети Интернет");
            }
        }

        public static SqlConnection Connection { get; set; }

        public static string ConnectionString { get; set; } =
            @"Data Source=DESKTOP-JB2KS99\SQLEXPRESS;Initial Catalog=LearnpyDB;Integrated Security=True";
        public static SqlConnection OpenConnection()
        {
            try {
                Connection = new SqlConnection(ConnectionString);
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();
                return Connection;
            }
            catch {
                throw new Exception("Не удалось соединится с сервером. Проверьте название указанного сервера на наличие ошибки или подключитесь к сети Интернет");
            }
        }

        public static void CloseConnection()
        {
            if (Connection != null && Connection.State == ConnectionState.Open)
                Connection.Close();
        }
        public static void Execute(string commandText, bool quiet = true)
        {
            try {
                var connection = OpenConnection();
                SqlCommand command = new SqlCommand(commandText) {
                    Connection = connection
                };
                command.ExecuteNonQuery();
                connection.Close();

            }
            catch {

            }
        }

        public static SqlDataReader Read(string commandText, bool quiet = true)
        {
            var connection = OpenConnection();
            var command = new SqlCommand(commandText) {
                Connection = connection
            };
            var reader = command.ExecuteReader();

            return reader;
        }

        public static object[] GetReaderResults(SqlDataReader reader, int columnCount)
        {
            object[] results = new object[columnCount];

            for (int i = 0; i < columnCount; i++) {
                results[i] = reader.IsDBNull(i) ? "" : reader.GetValue(i);
            }

            return results;
        }

        public static void GiveAchievement(string achievementName)
        {
            string getIdFromName = $"select Id from Achievements where Name = '{achievementName}'";
            var reader1 = Read(getIdFromName); reader1.Read();
            int id = reader1.GetInt32(0);
            string checkIfAlreadyHas = $"select * from UserAchievement where AchievementId = '{id}' and UserId = '{User.Id}'";
            if (Read(checkIfAlreadyHas).Read()) {
                return;
            }

            string cmd = $"insert into UserAchievement (UserId, AchievementId, [Date]) values ('{User.Id}', '{id}', '{DateTime.Today.ToString("yyyy-MM-dd")}')";
            Execute(cmd);
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
    }
}
