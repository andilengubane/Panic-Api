using System;
using System.Text;
using Utils.Logger;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace RoadCover.GenerateToken.Token.Service
{
    public class TokenService
    {
        private const string baseUrl = @"https://staging-panic.aura.services/panic-api/oauth/token";
        public static string GetToken()
        {
            string token = string.Empty;
            string clientSecret = "939D2BPuIrhKsddER5nNEIYXXB5t0S-pQE-YNgY0PyQkccn1eKZIdEwhQo7FOCQX";

            var data = new Dictionary<string, string>
            {
                {"clientId", "O3ujMTEiEotWApm7W5SHiXaz3ktbdu6c"},
                {"clientSecret", clientSecret},
                {"client_credentials", "client_credentials"}
            };

            using (var http = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var request = http.PostAsync(baseUrl, content);
                var response = request.Result.Content.ReadAsStringAsync().Result;

                JObject tekenObject = JObject.Parse(response);
                token  = Convert.ToString(tekenObject.SelectToken("accessToken"));
                string expireDate =Convert.ToString(tekenObject.SelectToken("expiresIn"));
                string tokenType = Convert.ToString(tekenObject.SelectToken("tokenType"));

                if (request.Result.IsSuccessStatusCode == true)
                {
                    try
                    {
                        BusinessLogic.TokenRepositary.InsertToken(token, expireDate,tokenType, baseUrl);
                    }
                    catch (Exception ex)
                    {
                        evLogger.LogEvent("Problem getting token" + ex.Message + "\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                        throw ex;
                    }
                    return token;
                }
            }
            return token;
        }
    }
}