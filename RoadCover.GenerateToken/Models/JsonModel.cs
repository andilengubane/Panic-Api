using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoadCover.GenerateToken.Models
{
    public class JsonModel
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("subscriptionTypeId")]
        public int subscriptionTypeId { get; set; }
        [JsonProperty("customerId")]
        public int customerId { get; set; }
        [JsonProperty("validFrom")]
        public DateTime validFrom { get; set; }
        [JsonProperty("validTo")]
        public DateTime validTo { get; set; }
        [JsonProperty("CancelledDate")]
        public DateTime CancelledDate { get; set; }
        [JsonProperty("CancelledReason")]
        public DateTime CancelledReason { get; set; }

        [JsonProperty("createdAt")]
        public DateTime createdAt { get; set; }
        [JsonProperty("updatedAt")]
        public DateTime updatedAt { get; set; }
    }
}