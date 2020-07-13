using System;
using System.Data;
using Utils.Logger;
using System.Configuration;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Helper
{
    public class DataAccess
    {
        #region Properties

        private static string DbConnection = ConfigurationManager.ConnectionStrings["VerificationDb"].ConnectionString;

        #endregion
        public enum DjangoDatabase
        {
            Slave,
            Master
        }
        #region Methods
        /// <summary>
        /// Executes a query based on a passed query.
        /// </summary>
        /// <param name="query">A <see cref="string"/>string that contains the database query to execute.</param>
        /// <returns>A <see cref="System.Data.DataTable"/>DataTable object containing the results of the query that was executed, or an empty DataTable.</returns>
        public DataTable ExecuteData(string query)
        {
            using (DataTable dtData = new DataTable())
            {
                using (SqlConnection conn = new SqlConnection(DbConnection))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            conn.Open();
                            cmd.CommandTimeout = 1200;
                            sda.Fill(dtData);
                            conn.Close();
                        }
                    }
                }
                return dtData;
            }
        }

        /// <summary>
        /// Executes a DML command (INSERT, UPDATE, DELETE, etc) against the database based on a passed query.
        /// </summary>
        /// <param name="query">A <see cref="string"/>string containing the DML command to execute. [Optional]</param>
        public static void ExecuteNoReader(string query, List<SqlParameter> parameters)
        {
            using (SqlConnection conn = new SqlConnection(DbConnection))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    LoadSqlCommandObject(cmd, parameters);

                    conn.Open();
                    cmd.CommandTimeout = 1200;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public static void ExecuteNoReader(string query)
        {
            using (SqlConnection conn = new SqlConnection(DbConnection))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.CommandTimeout = 1200;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Executes a stored procedure based on the name and parameters passed.
        /// </summary>
        /// <param name="spquery">A <see cref="string"/>string containing the name of the stored procedure to execute.</param>
        /// <param name="parametersToLoad">A <see cref="System.Collections.Generic.List{System.Data.SqlClient.SqlParameter}"/>list of SqlParameters to add to the passed SqlCommand. [Optional]</param>
        public static void ExecuteSpNoReader(string spquery, List<SqlParameter> spParams = null)
        {
            using (SqlConnection conn = new SqlConnection(DbConnection))
            {
                using (SqlCommand cmd = new SqlCommand(spquery, conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    LoadSqlCommandObject(cmd, spParams);
                    cmd.CommandTimeout = 1200;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Executes a stored procedure based on the name and sale ID passed.
        /// </summary>
        /// <param name="spquery">A <see cref="string"/>string containing the name of the stored procedure to execute.</param>
        /// <param name="sourcesaleid">A <see cref="string"/>string containing the sale ID of the sale to process.</param>
        public static void ExecuteSpNoReader(string spquery, string sourcesaleid)
        {
            using (SqlConnection conn = new SqlConnection(DbConnection))
            {
                using (SqlCommand cmd = new SqlCommand(spquery, conn))
                {
                    cmd.Parameters.AddWithValue("@SourceSaleID", sourcesaleid);
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.CommandTimeout = 1200;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Executes a query against the database based on the string argument passed.
        /// </summary>
        /// <param name="query">A <see cref="string"/>string containing the query string to execute.</param>
        /// <returns>A <see cref="System.DataTable"/>DataTable containing the data from the query, or an empty DataTable.</returns>
        public static DataSet GetData(string query)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["VerificationDb"].ConnectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        cmd.CommandTimeout = 1200;
                        using (DataSet ds = new DataSet())
                        {
                            sda.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executes a query against the database based on the string argument passed.
        /// </summary>
        /// <param name="query">A <see cref="string"/>string containing the query string to execute. [Optional]</param>
        /// <returns>A <see cref="System.DataTable"/>DataTable containing the data from the query, or an empty DataTable.</returns>
        public static DataTable ExecuteDataReader(string query, List<SqlParameter> parameters)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["VerificationDb"].ConnectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // please test this operates correctly.                        
                        LoadSqlCommandObject(cmd, parameters);
                        cmd.CommandTimeout = 1200;
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }

        public static DataTable ExecuteSpDataReader(string query, List<SqlParameter> parameters)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["VerificationDb"].ConnectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // please test this operates correctly.                        
                        LoadSqlCommandObject(cmd, parameters);
                        cmd.CommandTimeout = 1200;
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }
        public static DataTable ExecuteDataReader(string query)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["VerificationDb"].ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            cmd.CommandTimeout = 1200;
                            sda.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Takes a list of SqlParamater objects and binds them to a SqlCommand.
        /// </summary>
        /// <param name="cmdToLoad">The <see cref="System.Data.SqlCommand"/>SqlCommand object that will have its SqlParameter collection initialized</param>
        /// <param name="parametersToLoad">A <see cref="System.Collections.Generic.List{System.Data.SqlClient.SqlParameter}"/>list of SqlParameters to add to the passed SqlCommand. [Optional]</param>
        private static void LoadSqlCommandObject(SqlCommand cmdToLoad, List<SqlParameter> parametersToLoad = null)
        {
            try
            {
                // Check if the parameter list is null, and if not, add parameters!
                // Simple, ne? (Famous last words...)
                if (parametersToLoad != null)
                {
                    cmdToLoad.Parameters.AddRange(parametersToLoad.ToArray());
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log("", "LoadSqlCommandObject", ex.InnerException.Message, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        private static void LoadSqlCommandObject(MySqlCommand cmdToLoad, List<MySqlParameter> parametersToLoad = null)
        {
            try
            {
                // Check if the parameter list is null, and if not, add parameters!
                // Simple, ne? (Famous last words...)
                if (parametersToLoad != null)
                {
                    cmdToLoad.Parameters.AddRange(parametersToLoad.ToArray());
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log("", "LoadSqlCommandObject", ex.InnerException.Message, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
        /// <summary>
        /// Takes a list of SqlParamater objects and binds them to a SqlCommand.
        /// </summary>
        /// <param name="cmdToLoad">The <see cref="MySql.Data.SqlCommand"/>SqlCommand object that will have its SqlParameter collection initialized</param>
        /// <param name="parametersToLoad">A <see cref="System.Collections.Generic.List{System.Data.SqlClient.SqlParameter}"/>list of SqlParameters to add to the passed SqlCommand. [Optional]</param>
        private static void LoadSqlCommandObject(MySqlCommand cmdToLoad, List<SqlParameter> parametersToLoad = null)
        {
            try
            {
                // Check if the parameter list is null, and if not, add parameters!
                // Simple, ne? (Famous last words...)
                if (parametersToLoad != null)
                {
                    cmdToLoad.Parameters.AddRange(parametersToLoad.ToArray());
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                //TODO: Add logger here!
                throw ex;
            }
        }
        /// <summary>
        /// Executes a query against the Django application database based on the string argument passed.
        /// </summary>
        /// <param name="djangoquery">A <see cref="string"/>string containing the query string to execute.</param>
        /// <returns>A <see cref="bool"/>boolean value denoting the success of the database call.</returns>
        public static bool ExecuteDjangoBool(string djangoquery, List<MySqlParameter> parameters, string campaigncode)
        {
            bool isSuccess = false;
            try
            {
                using (MySqlConnection mysqlconn = new MySqlConnection(GetConnection(campaigncode, DjangoDatabase.Master)))
                {
                    using (MySqlCommand mysqlcmd = new MySqlCommand(djangoquery, mysqlconn))
                    {
                        LoadSqlCommandObject(mysqlcmd, parameters);

                        mysqlconn.Open();
                        mysqlcmd.CommandTimeout = 1200;
                        mysqlcmd.ExecuteNonQuery();
                        mysqlconn.Close();
                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Add logger here!
                throw ex;
                isSuccess = false;
            }

            return isSuccess;
        }
        public static void ExecuteDjangoQuery(string query, string campaigncode)
        {
            try
            {
                using (MySqlConnection mysqlconn = new MySqlConnection(GetConnection(campaigncode, DjangoDatabase.Master)))
                {
                    using (MySqlCommand mysqlcmd = new MySqlCommand(query, mysqlconn))
                    {
                        mysqlconn.Open();
                        mysqlcmd.ExecuteNonQuery();
                        mysqlconn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Add logger here!
                throw ex;
            }
        }
        public static DataTable ExecuteDjangoQueryReturnReader(string query, string campaigncode)
        {
            try
            {
                using (MySqlConnection mysqlconn = new MySqlConnection(GetConnection(campaigncode, DjangoDatabase.Master)))
                {
                    using (MySqlCommand mysqlcmd = new MySqlCommand(query, mysqlconn))
                    {
                        using (MySqlDataAdapter msda = new MySqlDataAdapter(mysqlcmd))
                        {
                            using (DataTable dtData = new DataTable())
                            {
                                mysqlconn.Open();
                                mysqlcmd.CommandTimeout = 1200;
                                msda.Fill(dtData);
                                mysqlconn.Close();
                                return dtData;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Add logger here!
                throw ex;
            }
        }

        public static DataTable ExecuteDjangoQueryReturnReader(string query, string campaignCode, List<MySqlParameter> sqlParameters)
        {
            try
            {
                using (MySqlConnection mySqlConn = new MySqlConnection(GetConnection(campaignCode, DjangoDatabase.Master)))
                {
                    using (MySqlCommand mySqlCmd = new MySqlCommand(query, mySqlConn))
                    {
                        LoadSqlCommandObject(mySqlCmd, sqlParameters);

                        using (MySqlDataAdapter mySqlAdapter = new MySqlDataAdapter(mySqlCmd))
                        {
                            using (DataTable dtData = new DataTable())
                            {
                                mySqlConn.Open();
                                mySqlCmd.CommandTimeout = 1200;
                                mySqlAdapter.Fill(dtData);
                                mySqlConn.Close();
                                return dtData;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Add logger here!
                throw;
                evLogger.LogEvent($"RCWESTBA: Files not export to sftp {ex}", System.Diagnostics.EventLogEntryType.FailureAudit);

            }
        }

        public static string GetConnection(string campaigname, DjangoDatabase djangoDb)
        {
            string connection = string.Empty;
            using (DataTable dtDataCampaign = ExecuteDataReader("Select * FROM CampaignSettings WHERE Campaign ='" + campaigname + "'"))
            {
                if (dtDataCampaign.Rows.Count > 0)
                {
                    switch (djangoDb)
                    {
                        case DjangoDatabase.Slave:
                            connection = "server=" + dtDataCampaign.Rows[0]["SlaveDbIPSource"].ToString() + ";user id=" + dtDataCampaign.Rows[0]["SlaveDbUser"].ToString() + ";password=" + dtDataCampaign.Rows[0]["SlaveDbPass"].ToString() + ";database=" + dtDataCampaign.Rows[0]["DbSource"].ToString() + ";port=" + dtDataCampaign.Rows[0]["SlaveDbPort"].ToString() + "";
                            break;
                        case DjangoDatabase.Master:
                            connection = "server=" + dtDataCampaign.Rows[0]["MasterDbIPSource"].ToString() + ";user id=" + dtDataCampaign.Rows[0]["MasterDbUser"].ToString() + ";password=" + dtDataCampaign.Rows[0]["MasterDbPass"].ToString() + ";database=" + dtDataCampaign.Rows[0]["DbSource"].ToString() + ";port=" + dtDataCampaign.Rows[0]["MasterDbPort"].ToString() + "";
                            break;
                    }
                }
            }
            return connection;
        }
        public static string GetSlaveDbConnections(string campaigname)
        {
            return DjangoDb.SlaveDb = "server=192.168.66.254;user id=reports;password=r3p0rts;database=" + GetDbSource(campaigname) + ";port=3308";
        }
        public static string GetMasterDbConnections(string campaigname)
        {
            return DjangoDb.MasterDb = "server=192.168.130.33;user id=iat_clientdtl;password=clientp455;database=" + GetDbSource(campaigname) + "";
        }
        private static string GetDbSource(string campaigname)
        {
            using (DataTable dtData = ExecuteDataReader("Select DbSource FROM CampaignSettings WHERE Campaign ='" + campaigname + "'"))
            {
                return dtData.Rows[0]["DbSource"].ToString();
            }
        }
        public static class DjangoDb
        {
            public static string SlaveDb { get; set; }
            public static string MasterDb { get; set; }
        }
    }
    #endregion
}