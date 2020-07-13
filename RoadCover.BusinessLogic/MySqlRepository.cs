using System.Data;

namespace RoadCover.BusinessLogic
{
    public class MySqlRepository
    {
        public static void insertCancelSubscription(int subscriptionId, string cellPhone, string responseMessage)
        {
            DataTable tmpTable = new RoadCover.Helper.SubscriptionData().insertCancelSubscription(subscriptionId, cellPhone, responseMessage);
            tmpTable.Dispose();
            tmpTable = null;
        }
    }
}
