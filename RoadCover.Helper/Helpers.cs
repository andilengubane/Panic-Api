using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Helper
{
    public class Helpers
    {
        public static string GetCampaignSchema(string campaignName)
        {
            string sql = "getCampaignSchema";
            string schema = String.Empty;

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["verificationDb"].ConnectionString.ToString();

            SqlDataAdapter adp = new SqlDataAdapter(sql, conn);

            adp.SelectCommand.CommandTimeout = 1000;
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.SelectCommand.Parameters.Add("@CampaignName", SqlDbType.VarChar).Value = campaignName;

            DataTable dt = new DataTable();

            try
            {
                adp.Fill(dt);
            }
            catch (Exception ex)
            {
                Log("", "GetCampaignSchema", ex.InnerException.Message, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                throw;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    adp.Dispose();
                    conn.Close();
                }
            }

            if (dt.Rows.Count > 0)
            {
                schema = dt.Rows[0][0].ToString();
            }

            return schema;
        }
     
        public static void Log(string sourcesaleid, string method, string methodmessage, string date)
        {
            string logquery = "INSERT INTO Verifications_log VALUES(@SourceSaleID, @Method, @MethodMessage, @Date);";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@SourceSaleID", SqlDbType = SqlDbType.VarChar, Value = sourcesaleid },
                new SqlParameter() { ParameterName = "@Method", SqlDbType = SqlDbType.VarChar, Value = method },
                new SqlParameter() { ParameterName = "@MethodMessage", SqlDbType = SqlDbType.VarChar, Value = methodmessage },
                new SqlParameter() { ParameterName = "@Date", SqlDbType = SqlDbType.VarChar, Value = date }
            };

            DataAccess.ExecuteDataReader(logquery, parameters);
        }

        public static string RemoveSpecialCharacters(string textValue)
        {
            return Regex.Replace(textValue, @"[^\w]", " ");
        }
        public static DataTable GetAllLanguages()
        {
            string languagesQuery = @"SELECT COLUMN_NAME
                                      FROM INFORMATION_SCHEMA.COLUMNS
                                      WHERE TABLE_NAME = 'UserLanguages' AND COLUMN_NAME NOT IN('UserID','English','Name','Username') 
                                      ORDER BY COLUMN_NAME";

            using (DataTable dtData = DataAccess.ExecuteDataReader(languagesQuery))
            {
                return dtData;
            }
        }
    }
}