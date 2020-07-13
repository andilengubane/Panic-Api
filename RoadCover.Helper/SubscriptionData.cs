using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace RoadCover.Helper
{
    public class SubscriptionData
    {
        public bool isMySQLConnection = Convert.ToBoolean(ConfigurationSettings.AppSettings["IsMySQL"].ToString());
        private SqlConnection _connSlave = new SqlConnection(ConfigurationSettings.AppSettings.Get("SlaveDB"));
        private SqlConnection _connMaster = new SqlConnection(ConfigurationSettings.AppSettings.Get("MasterDB"));

        public  DataTable insertCancelSubscription(int subscriptionId, string cellphoneNumber, string responseMessage)
        {
            var s = new DataTable();
            if (isMySQLConnection)
            {
                DataTable AgentsTable = new DataTable();
                try
                {
                    var dbCon = MySQLConnection.Instance();
                    dbCon.DatabaseName = "DataBase Name";

                    if (dbCon.IsConnect())
                    {
                        string query = string.Format("INSERT INTO clientdetails_cancelsubscription(SubscriptionId, CellphoneNumber, ResponseMessage, DateLogged) VALUES ({0},{1},{2},{3});", "'" + subscriptionId + "'", "'" + cellphoneNumber + "'", "'" + responseMessage + "'", "'" + DateTime.Now + "'");
                        var cmd = new MySqlCommand(query, dbCon.Connection);
                        cmd.ExecuteNonQuery();
                        dbCon.Close();
                    }
                }
                catch (Exception ex)
                {
                    //TODO add events logger
                }
                return AgentsTable;
            }
            return s;
        }
    }
}
