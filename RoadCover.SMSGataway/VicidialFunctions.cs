using System;
using System.Net;

namespace RoadCover.SMSGataway
{
    public class VicidialFunctions
    {
        const string VICIDIALHTTPSTATUS = "http://bps-cam-web001.bytespeoplesolutions.co.za/vicidial/non_agent_api.php?source=SMSGateway&user=9998&pass=20142029&function=update_lead&lead_id={0}&status={1}";
        public bool UpdateVivcidialStatus(string LeadID, string VicidialStatus, string CampaignID)
        {
            bool bReturn = false;
            try
            {
                string httpGetter = string.Format(VICIDIALHTTPSTATUS, LeadID, VicidialStatus);
                if (VicidialStatus == "CALLBK")
                {
                    httpGetter += "&callback=Y&callback_datetime=NOW&campaign_id=" + CampaignID;
                }

                WebClient Client = new WebClient();
                string httpResponse = Client.DownloadString(httpGetter);

                if (httpResponse.Contains("SUCCESS"))
                    bReturn = true;
            }
            catch (Exception)
            {
                bReturn = false;
            }
            return bReturn;
        }
    }
}
