using System.Linq;
using System.Net.Http;

namespace RoadCover.GenerateToken.SMS.Service
{
    public class SMSService
    {
        public static void SendSMS(string mobileNumber)
        {
            using (var http = new HttpClient())
            {
                var url = "https://relay1.za.oxygen8.com:9000/Project_Help_Unitrans?&username=unitrans&password=R3DascOR&MSISDN="+mobileNumber+"&content=newbulksmstest&channel=SOUTHAFRICA.BULK&premium=0&campaignid=45240";
                var response = http.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;
                
                if (response.Result.IsSuccessStatusCode == true)
                {
                    result.ToArray();
                }
            }
        }
    }
}