using System;
using Utils.Logger;
using System.Web.Http;
using RoadCover.GenerateToken.Models;
using RoadCover.GenerateToken.Token.Service;
using RoadCover.GenerateToken.Subscription.Service;

namespace RoadCover.GenerateToken.Controllers
{
    public class SubscriptionController : ApiController
    {
        //GET api/SubscriptionVerification
        [HttpGet]
        public string SubscriptionVerification(string cellPhone) 
        {
            var token = TokenService.GetToken();
            if (!String.IsNullOrWhiteSpace(token))
            {
                try
                {
                    string isSubsribed = SubscriptionService.SubscriptionVerification(cellPhone, token);
                    return isSubsribed;
                }
                catch (Exception ex)
                {
                    evLogger.LogEvent("Something went while verifying subscription." + ex.Message + "\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                }
            }
            return "Something went wrong verifying subscription.";
        }

        //POST api/CreateSubscription
        [HttpPost]
        public string CreateSubscription(ClientDetailsModel model)
        {
            var token = TokenService.GetToken();
            if (!String.IsNullOrWhiteSpace(token))
            {
                try
                {
                    string responseMessage = SubscriptionService.CreateSubscription(model.SalesId, model.ClientId, token, model.CellPhone, model.CampaignName);
                    return responseMessage;
                }
                catch (Exception ex)
                {
                    evLogger.LogEvent("Something went while creating subscription." + ex.Message + "\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                }
            }
            return "Something went wrong creating subscription.";
        }

        //DELETE api/DeleteSubscription
        [HttpDelete]
        public string DeleteSubscription(string cellphone)
        {
            var token = TokenService.GetToken();
            if (!String.IsNullOrWhiteSpace(token))
            {
                try
                {
                    string subscriptioCancellation = SubscriptionService.CancellingSubscription(cellphone, token);
                    return subscriptioCancellation;
                }
                catch (Exception ex)
                {
                    evLogger.LogEvent("Something went while cancelling subscription." + ex.Message + "\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                }
            }
            return "Something went wrong cancelling subscription.";
        }
    }
}