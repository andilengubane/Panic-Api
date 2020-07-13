using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace RoadCover.Helper
{
   public class MySQLConnection
    {
        private MySQLConnection() { }

        private string host = ConfigurationSettings.AppSettings["Host"].ToString();
        private string databaseName = ConfigurationSettings.AppSettings["Database"].ToString();
        private string port = ConfigurationSettings.AppSettings["Port"].ToString();
        private string username = ConfigurationSettings.AppSettings["UserName"].ToString();
        private string password = ConfigurationSettings.AppSettings["Password"].ToString();

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }
        public string Password { get; set; }
        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }
        private static MySQLConnection _instance = null;
        public static MySQLConnection Instance()
        {
            if (_instance == null)
                _instance = new MySQLConnection();
            return _instance;
        }

        public bool IsConnect()
        {
            if (Connection == null)
            {
                if (String.IsNullOrEmpty(databaseName))
                    return false;
                string connstring = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}", host, port, databaseName, username, password);
                connection = new MySqlConnection(connstring);
                connection.Open();
            }

            return true;
        }

        public void Close()
        {
            connection.Close();
        }
    }
}