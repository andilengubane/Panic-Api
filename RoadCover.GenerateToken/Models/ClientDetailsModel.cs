using System;

namespace RoadCover.GenerateToken.Models
{
    public class ClientDetailsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string CellPhone { get; set; }
        public string EmailAddress { get; set; }
        public string SalesId { get; set; }
        public string ClientId { get; set; }
        public string CampaignName { get; set; }
        public int CampaignId { get; set; }
    }
}