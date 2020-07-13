using System;
using Helper;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace RoadCover.BusinessLogic
{
    public class TokenRepositary
    {
        public static void InsertToken(string token, string expire_date, string tokenType,string baseUrl)
        {
            List<SqlParameter> parms = new List<SqlParameter>
            {
            new SqlParameter("@Token", token),
            new SqlParameter("@ExpireDate", expire_date),
            new SqlParameter("@TokenType", tokenType),
            new SqlParameter("@DateLogged",DateTime.Now),
            new SqlParameter("@baseUrl", baseUrl)
            };
            DataAccess.ExecuteSpNoReader("sprocedureInsertToken", parms);
        }

        public static void InsertClientDetails(string clientId, string salesId,  string cellPhone, string campaignName, string responseMessage, string baseUrl)
        {
            List<SqlParameter> parms = new List<SqlParameter>
            {
            new SqlParameter("@SalesId", salesId),
            new SqlParameter("@ClientId", clientId),
            new SqlParameter("@CellPhone", cellPhone),
            new SqlParameter("@CampaignName", campaignName),
            new SqlParameter("@ResponseMessage", responseMessage),
            new SqlParameter("@DateLogged",DateTime.Now),
            new SqlParameter("@baseUrl", baseUrl)
            };
            DataAccess.ExecuteSpNoReader("sprocedureInsertClientDetails", parms);
        }

        public static void InsertSubscriptionDetails(string cellPhone,string subscription_Id,string subscriptionTypeId,string customerId,string validFrom,string validTo,string cancelledDate,string cancelledReason,string createdAt, string updatedAt,string baseUrl)
        {
            List<SqlParameter> parms = new List<SqlParameter>
            {
            new SqlParameter("@CellPhone", cellPhone),
            new SqlParameter("@Subscription_Id",subscription_Id),
            new SqlParameter("@SubscriptionTypeId", subscriptionTypeId),
            new SqlParameter("@CustomerId",customerId),
            new SqlParameter("@ValidFrom",validFrom),
            new SqlParameter("@ValidTo",validTo),
            new SqlParameter("@CancelledDate",cancelledDate),
            new SqlParameter("@CancelledReason",cancelledReason),
            new SqlParameter("@CreatedAt",createdAt),
            new SqlParameter("@UpdatedAt",updatedAt),
            new SqlParameter("@DateLogged",DateTime.Now),
            new SqlParameter("@baseUrl",baseUrl)
            };
            DataAccess.ExecuteSpNoReader("InsertSubscriptionDetails", parms);
        }

        public static void CancelSusbscription(int subscriptionId,string cellPhone,string responseMessage, string baseUrl)
        {
            List<SqlParameter> parms = new List<SqlParameter>
            {
            new SqlParameter("@SubscriptionId", subscriptionId),
            new SqlParameter("@CellphoneNUmber", cellPhone),
            new SqlParameter("@ResponseMessage", responseMessage),
            new SqlParameter("@DateLogged",DateTime.Now),
            new SqlParameter("@baseUrl", baseUrl)
            };
            DataAccess.ExecuteSpNoReader("spCancelSusbscription", parms);
        }

        public static int GetSubscriptionId(string cellPhone)
        {
            DataTable resultTable= DataAccess.ExecuteDataReader("SELECT DISTINCT Subscription_Id FROM SubscriptionDetails WHERE CellPhone ='" + cellPhone + "' AND ValidFrom <='" + DateTime.Now + "' AND ValidTo > '" + DateTime.Now + "'");
            int subscriptionId = 0;

            if (resultTable.Rows.Count>0)
            {
                subscriptionId = (int)resultTable.Rows[0]["Subscription_Id"];
            }
            resultTable.Dispose();
            return subscriptionId;
        }
    }
}
