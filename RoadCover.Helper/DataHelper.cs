using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Helper
{
    public class DataHelper
    {
        public static bool Sale(SaleAction action, string sourcesaleid, string loggedOnUser, string comments, string campaignid, string campaignname, string status)
        {
            bool saleStatus = false;
            switch (action)
            {
                case SaleAction.FAIL:
                    FailSale(sourcesaleid, loggedOnUser, campaignid, comments, campaignname);
                    break;
                case SaleAction.RETRY:
                    saleStatus = SaleRetry(sourcesaleid, loggedOnUser, comments, campaignid, campaignname, status);
                    break;
                case SaleAction.PASS:
                    saleStatus = PassSale(sourcesaleid, loggedOnUser, comments, campaignid, campaignname, status);
                    break;
            }
            return saleStatus;
        }
        public enum SaleAction
        {
            FAIL,
            RETRY,
            PASS
        }
        public static string GetCampaignWorkFlow(string campaigncode, string status, string campaignname)
        {
            using (DataTable dtData = DataAccess.ExecuteDjangoQueryReturnReader("SELECT id FROM clientdetails_workflowactionstatus_l WHERE campaign_id = " + campaigncode + " AND name = '" + status + "'", campaignname))
            {
                return dtData.Rows[0]["id"].ToString();
            }
        }
        public static string GetCampaignWorkFlowName(string workflowid, string campaignname)
        {
            try
            {
                using (DataTable dtData = DataAccess.ExecuteDjangoQueryReturnReader("SELECT name FROM clientdetails_workflowactionstatus_l WHERE id = " + workflowid, campaignname))
                {
                    return dtData.Rows[0]["name"].ToString();
                }
            }
            catch
            {
                return "--ERROR--Action Not Found";
            }
        }
        private static void FailSale(string sourcesaleid, string loggedOnUser, string campaignid, string comments, string campaignname)
        {
            string failsale = GetCampaignWorkFlow(campaignid, "VET-Fail", campaignname);
            string saleQuery = @"INSERT INTO clientdetails_workflowaction (clientid, status_id, actioner,comment, created)
                                       VALUES (@SourceSaleID, @StatusID, @Actioner, @Comment, @Created)";

            List<MySqlParameter> mysqlparameters = new List<MySqlParameter>()
            {
                new MySqlParameter() {ParameterName = "@SourceSaleID", MySqlDbType= MySqlDbType.VarChar, Value = sourcesaleid },
                new MySqlParameter() {ParameterName = "@StatusID", MySqlDbType = MySqlDbType.Int32, Value = failsale },
                new MySqlParameter() {ParameterName = "@Actioner", MySqlDbType = MySqlDbType.VarChar, Value = loggedOnUser},
                new MySqlParameter() {ParameterName = "@Comment", MySqlDbType = MySqlDbType.VarChar, Value = comments },
                new MySqlParameter() {ParameterName = "@Created", MySqlDbType = MySqlDbType.Date, Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
            };

            if (DataAccess.ExecuteDjangoBool(saleQuery, mysqlparameters, campaignname) == true)
            {
                List<SqlParameter> updateParameters = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@SourceSaleID", SqlDbType = SqlDbType.VarChar, Value = sourcesaleid },
                new SqlParameter() {ParameterName = "@WorkFlowStepID", SqlDbType = SqlDbType.Int, Value = failsale },
                new SqlParameter() {ParameterName = "@SaleStatusID", SqlDbType = SqlDbType.Int, Value = failsale },
                new SqlParameter() {ParameterName = "@SaleDescription", SqlDbType = SqlDbType.VarChar, Value = "VET-Fail" },
                 new SqlParameter() {ParameterName = "@SaleMessage", SqlDbType = SqlDbType.VarChar, Value = comments },
                new SqlParameter() {ParameterName = "@FailState", SqlDbType = SqlDbType.Bit, Value = 1 },
                new SqlParameter() {ParameterName = "@Campaign", SqlDbType = SqlDbType.VarChar, Value = campaignname}
            };
                DataAccess.ExecuteNoReader("UPDATE Saless SET WorkFlowStepID = @WorkFlowStepID, SaleStatus = @SaleStatusID, SaleDescription = @SaleDescription,SaleMessage=@SaleMessage, FailState=@FailState WHERE SourceSaleID = @SourceSaleID AND CampaignName = @Campaign;", updateParameters);
                DataAccess.ExecuteSpNoReader("spCleanTempTable", sourcesaleid);
            }
        }
        private static bool SaleRetry(string sourcesaleid, string loggedOnUser, string comments, string campaignid, string campaignname, string status)
        {
            bool isReTried = false;
            using (DataTable dtReTrySale = CheckSaleStatus(sourcesaleid, campaignname))
            {
                if (dtReTrySale.Rows.Count > 0 && dtReTrySale.Rows[0]["name"].ToString().ToLower().Contains("retry") && !dtReTrySale.Rows[0]["name"].ToString().ToLower().Contains("retry1"))
                {
                    isReTried = true;
                    string sqlQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SET ANSI_WARNINGS  OFF;UPDATE Saless Set  SaleUserAssigned=1,WorkFlowStepID=" + dtReTrySale.Rows[0]["status_id"].ToString() + ",UserAssigned='" + dtReTrySale.Rows[0]["actioner"].ToString() + "' WHERE SourceSaleID= " + sourcesaleid + " AND campaignName = '" + campaignname + "' SET ANSI_WARNINGS ON;";
                    DataAccess.ExecuteNoReader(sqlQuery);
                    DataAccess.ExecuteNoReader("DELETE FROM SalesTemp WHERE SourceSaleID= " + sourcesaleid + " AND Campaign = '" + campaignname + "'");
                }
                else
                {
                    string retry = GetCampaignWorkFlow(campaignid, status, campaignname);
                    string saleQueryRetry = @"INSERT INTO clientdetails_workflowaction (clientid, status_id, actioner,comment, created)
                                       VALUES (" + sourcesaleid + "," + retry + ",'" + loggedOnUser + "'," + "'" + comments + " -VS'" + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                    //BALUKA TABLES, Who's Child are these tables
                    string retrymessage = @"INSERT INTO clientdetails_workflowactionmessages (message, author,workflowaction_id) 
                                  SELECT comment, actioner, id from clientdetails_workflowaction
                                  WHERE status_id = " + retry + " and clientid = " + sourcesaleid + " ORDER BY created DESC LIMIT 1";


                    int firstretrycount = 0;
                    using (DataTable dtData = DataAccess.ExecuteDataReader("select FirstReTrySale FROM Saless WHERE SourceSaleID= " + sourcesaleid  + " AND CampaignName = '" + campaignname + "'"))
                    {
                        firstretrycount = int.Parse(dtData.Rows[0]["FirstReTrySale"].ToString()) + 1;
                    }
                    DataAccess.ExecuteDjangoQuery(saleQueryRetry, campaignname);
                    DataAccess.ExecuteDjangoQuery(retrymessage, campaignname);
                    DataAccess.ExecuteNoReader("UPDATE Saless SET WorkFlowStepID=" + retry + ",SaleStatus=" + retry + ",SaleDescription='VET-RETRY',SaleMessage='" + comments + "',FirstReTrySale=" + firstretrycount + " WHERE SourceSaleID= " + sourcesaleid + " AND CampaignName = '" + campaignname + "'");
                    DataAccess.ExecuteSpNoReader("spCleanTempTable", sourcesaleid);
                }
            }
            return isReTried;
        }
        private static bool PassSale(string sourcesaleid, string loggedOnUser, string comments, string campaignid, string campaignname, string status)
        {
            bool isPassed = false;
            using (DataTable dtPassedSale = CheckSaleStatus(sourcesaleid, campaignname))
            {
                if (dtPassedSale.Rows.Count > 0 && dtPassedSale.Rows[0]["name"].ToString().ToLower().Contains("pass"))
                {
                    isPassed = true;
                    string sqlQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SET ANSI_WARNINGS  OFF;UPDATE Saless Set  SaleUserAssigned=1,WorkFlowStepID=" + dtPassedSale.Rows[0]["status_id"].ToString() + ",UserAssigned='" + dtPassedSale.Rows[0]["actioner"].ToString() + "' WHERE SourceSaleID=" + sourcesaleid + " AND campaignName = '" + campaignname + "' SET ANSI_WARNINGS ON;";
                    DataAccess.ExecuteNoReader(sqlQuery);
                    DataAccess.ExecuteNoReader("DELETE SalesTemp WHERE SourceSaleID=" + sourcesaleid + " AND Campaign = '" + campaignname + "'");
                }
                else
                {
                    string pass = GetCampaignWorkFlow(campaignid, status, campaignname);
                    string saleQuery = @"INSERT INTO clientdetails_workflowaction (clientid, status_id, actioner,comment, created)
                                       VALUES (@SourceSaleID, @StatusID, @Actioner, @Comment, @Created)";

                    List<MySqlParameter> mysqlparameters = new List<MySqlParameter>()
                            {
                                new MySqlParameter() {ParameterName = "@SourceSaleID", MySqlDbType= MySqlDbType.VarChar, Value = sourcesaleid },
                                new MySqlParameter() {ParameterName = "@StatusID", MySqlDbType = MySqlDbType.Int32, Value = pass },
                                new MySqlParameter() {ParameterName = "@Actioner", MySqlDbType = MySqlDbType.VarChar, Value = loggedOnUser},
                                new MySqlParameter() {ParameterName = "@Comment", MySqlDbType = MySqlDbType.VarChar, Value = comments +" -VS : Set By" + loggedOnUser  },
                                new MySqlParameter() {ParameterName = "@Created", MySqlDbType = MySqlDbType.DateTime, Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                            };
                    if (DataAccess.ExecuteDjangoBool(saleQuery, mysqlparameters, campaignname) == true)
                    {
                        int firstTry = IsFirstPass(sourcesaleid, campaignname);
                        List<SqlParameter> updateParameters = new List<SqlParameter>()
                            {
                                new SqlParameter() {ParameterName = "@SourceSaleID", SqlDbType = SqlDbType.VarChar, Value = sourcesaleid },
                                new SqlParameter() {ParameterName = "@WorkFlowStepID", SqlDbType = SqlDbType.Int, Value = pass },
                                new SqlParameter() {ParameterName = "@SaleStatusID", SqlDbType = SqlDbType.Int, Value = pass },
                                new SqlParameter() {ParameterName = "@SaleDescription", SqlDbType = SqlDbType.VarChar, Value = "VET-Pass" },
                                new SqlParameter() {ParameterName = "@SaleMessage", SqlDbType = SqlDbType.VarChar, Value = "Sale passed -VS" },
                                new SqlParameter() {ParameterName = "@FirstPassSale", SqlDbType = SqlDbType.VarChar, Value = firstTry },
                                new SqlParameter() {ParameterName = "@Campaign", SqlDbType = SqlDbType.VarChar, Value = campaignname}
                            };

                        DataAccess.ExecuteNoReader("UPDATE Saless SET WorkFlowStepID = @WorkFlowStepID, SaleStatus = @SaleStatusID, SaleDescription = @SaleDescription,SaleMessage=@SaleMessage,FirstPassSale=@FirstPassSale, SaleLastModified=GetDate() WHERE SourceSaleID = @SourceSaleID AND CampaignName =@Campaign;", updateParameters);
                        DataAccess.ExecuteSpNoReader("spCleanTempTable", sourcesaleid);
                    }
                }
                return isPassed;
            }
        }
        public static int IsFirstPass(string sourceSaleID, string campaingName)
        {
            int retVal = 0;
            List<SqlParameter> selParameters = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@SourceSaleID", SqlDbType = SqlDbType.VarChar, Value = sourceSaleID },
                new SqlParameter() {ParameterName = "@Campaign", SqlDbType = SqlDbType.VarChar, Value = campaingName}
            };

            DataTable tmpTable = DataAccess.ExecuteDataReader("select s.ID from saless s where s.CampaignName = @Campaign and s.SourceSaleID=@SourceSaleID and s.FirstReTrySale =0 and s.FailState=0", selParameters);
            if(tmpTable.Rows.Count==0)
            {
                retVal = 1; 
            }
            return retVal;   
        }
        public static DataTable CheckSaleStatus(string sourcesaleid, string campaignname)
        {
            DataTable dtData = DataAccess.ExecuteDjangoQueryReturnReader(@"SELECT cw.status_id,cw.clientid,cw.actioner,cwl.name
                                                                                  FROM clientdetails_workflowaction cw
                                                                                  INNER JOIN clientdetails_workflowactionstatus_l cwl ON cw.status_id = cwl.id
                                                                                  WHERE cw.clientid = " + sourcesaleid + " ORDER BY cw.created DESC LIMIT 1; ", campaignname);
            return dtData;
        }
        public enum SaleTask
        {
            NextTask = 1
        }
    }
}