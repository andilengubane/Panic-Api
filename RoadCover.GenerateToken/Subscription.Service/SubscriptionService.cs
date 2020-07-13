using System;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using RoadCover.SMSGataway;
using System.Net.Http.Headers;
using System.Collections.Generic;
using RoadCover.GenerateToken.Models;
using RoadCover.GenerateToken.SMS.Service;

namespace RoadCover.GenerateToken.Subscription.Service
{
    public class SubscriptionService
    {
        const string baseUrl = "https://staging-rappid.aura.services/panic-api"; 
        public static string SubscriptionVerification(string cellPhone,string token)
        {
            string isSubscribed = string.Empty;
            string msidsdn = cellPhone;

            var model = new SubscriptionModel();

            if (msidsdn.Substring(0, 1) == "0")
                msidsdn = "+27" + msidsdn.Substring(1);

            if (!String.IsNullOrWhiteSpace(msidsdn))
            {
                using(var http = new HttpClient())
                {
                    var url = $"baseUrl/customers/{msidsdn}/subscriptions";
                    http.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", token));
                    var response = http.GetAsync(url);
                    var request = response.Result.Content.ReadAsStringAsync().Result;
                    var jsonResult = request.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                    JObject jObject = JObject.Parse(jsonResult);
                    model.Subscription_Id = Convert.ToString(jObject.SelectToken("id"));
                    model.SubscriptionTypeId = Convert.ToString(jObject.SelectToken("subscriptionTypeId"));
                    model.CustomerId = Convert.ToString(jObject.SelectToken("customerId"));
                    model.ValidFrom = Convert.ToString(jObject.SelectToken("validFrom"));
                    model.ValidTo = Convert.ToString(jObject.SelectToken("validTo"));
                    model.CancelledDate = Convert.ToString(jObject.SelectToken("validFrom"));
                    model.CancelledReason = Convert.ToString(jObject.SelectToken("validTo"));
                    model.CreatedAt = Convert.ToString(jObject.SelectToken("validFrom"));
                    model.UpdatedAt = Convert.ToString(jObject.SelectToken("validTo"));

                    if (!String.IsNullOrWhiteSpace(model.Subscription_Id) && !String.IsNullOrWhiteSpace(model.SubscriptionTypeId)) 
                    {
                        BusinessLogic.TokenRepositary.InsertSubscriptionDetails(cellPhone, model.Subscription_Id, model.SubscriptionTypeId,
                                                                                model.CustomerId, model.ValidFrom, model.ValidTo, model.CancelledDate,
                                                                                model.CancelledReason, model.CreatedAt, model.UpdatedAt, baseUrl);

                        return isSubscribed = $"Subscription already exist for cellphone number: {msidsdn}";
                    }
                    else
                    {
                        return isSubscribed = $"No subcription found for cellphone number: {msidsdn}" ;
                    }
                }
            }
            return isSubscribed;
        }

        public static string CreateSubscription(string salesId, string clientId, string token, string cellPhone,  string campaignName)
        {
            string isCreated = string.Empty;
            string msidsdn = cellPhone;

            if (msidsdn.Substring(0, 1) == "0")
                msidsdn = "+27" + msidsdn.Substring(1);

            var data = new Dictionary<string, string>
            {
                {"mobileNumber", msidsdn}
            };

            if (!String.IsNullOrWhiteSpace(msidsdn))
            {
                using (var http = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(data));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    http.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", token));
                    var request = http.PostAsync($"baseUrl/oauth/user", content);
                    var response = request.Result.Content.ReadAsStringAsync().Result;

                    JObject jObject = JObject.Parse(response);
                    string responeMessage = Convert.ToString(jObject.SelectToken("message"));
                    string responeCode = Convert.ToString(jObject.SelectToken("code"));
                    string mobile = Convert.ToString(jObject.SelectToken("mobile"));

                    if (responeMessage == "user_creation_success") 
                    {
                        SMSService.SendSMS(mobile);
                        //BusinessLogic.TokenRepositary.InsertClientDetails(clientId, salesId, name, surname, cellPhone, emailAddress, campaignName, campaignId, responeMessage, responeCode);
                        BusinessLogic.TokenRepositary.InsertClientDetails(clientId, salesId, cellPhone, campaignName, responeMessage, baseUrl);
                        return isCreated = responeMessage;
                    }
                    else
                    {
                        var errorMessage = "Unable to create subscription";
                        BusinessLogic.TokenRepositary.InsertClientDetails(clientId, salesId, cellPhone, campaignName, responeMessage, baseUrl);
                        // public static void InsertClientDetails(string clientId, string salesId,  string cellPhone, string campaignName, string responseMessage, string baseUrl)
                        return isCreated = errorMessage + msidsdn;
                    }
                }
            }
            return isCreated;
        }

        public static string CancellingSubscription(string cellPhone,string token)
        {
            string isCancelled = string.Empty;
            string msidsdn = cellPhone;
            var model = new ResponeModel();

            if (msidsdn.Substring(0, 1) == "0")
                msidsdn = "+27" + msidsdn.Substring(1);

            var subscriptionId = BusinessLogic.TokenRepositary.GetSubscriptionId(msidsdn);

            if (!String.IsNullOrWhiteSpace(msidsdn))
            {
                using (var http = new HttpClient())
                {
                    var url = "baseUrl/subscription/{msidsdn}";
                    http.DefaultRequestHeaders.Add("Authorization",String.Format("Bearer {0}", token));
                    var response = http.GetAsync(url);
                    var request = response.Result.Content.ReadAsStringAsync().Result;

                    JObject jObject = JObject.Parse(request);
                    model.Success = Convert.ToString(jObject.SelectToken("success"));
                    model.Error = Convert.ToString(jObject.SelectToken("error"));

                    if (model.Success == "Successully cancelled subscription with cancellation reason")
                    {
                        string message = $"Please note that your subcription was canceled. Customer: {subscriptionId} , Phone Number: {msidsdn}";
                        var sendMessageResponse = HTMLPosters.SendSMSMessage( cellPhone, message);

                        if (sendMessageResponse == "Message accepted")
                        {
                            var successMessage = model.Success + ", subscription canceled";
                            BusinessLogic.TokenRepositary.CancelSusbscription(subscriptionId, cellPhone, model.Success, baseUrl);
                        }
                        return isCancelled = model.Success;
                    }
                    else
                    {
                        var errorMessage = model.Error+ " unable to cancel subscription";
                        BusinessLogic.TokenRepositary.CancelSusbscription(subscriptionId, cellPhone, errorMessage, baseUrl);
                        return isCancelled = model.Error;
                    }
                }
            }
            return isCancelled;
        }
    }
}