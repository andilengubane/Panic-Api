using System;
using System.Net;
using Utils.Logger;

namespace RoadCover.SMSGataway
{
    public class HTMLPosters
    {
        const string HTTPSMSADDRESS = "https://relay1.za.oxygen8.com:9000/RoadCover ";
        const string SMSGATEWAYPASS = "RoadC";
        const string SMSGATEWAYUSER = "R0@dC0";
        const string HTTPCALL = "https://relay1.za.oxygen8.com:9000/RoadCover?username=RoadC&password=R0@dC0&MSISDN={0}&content={1}&channel=SOUTHAFRICA.BULK";

        public static string SendSMSMessage( string cellPhone, string message)
        {
            string bReturn = string.Empty;
            try
            {
                string msidsdn = cellPhone;

                if (msidsdn.Substring(0, 1) == "0")
                    msidsdn = "27" + msidsdn.Substring(1);

                string httpGetter = string.Format(HTTPCALL, msidsdn, message);

                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                WebClient Client = new WebClient();

                string httpResponse = Client.DownloadString(httpGetter);

                if (httpResponse.Contains("Message accepted"))
                    bReturn = httpResponse.Split('\n')[2].ToString();
            }
            catch (Exception ex)
            {
                evLogger.LogEvent("Client (WebClient) sending a sms" + ex.Message + "\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
            }
            return bReturn;
        }
    }
}
