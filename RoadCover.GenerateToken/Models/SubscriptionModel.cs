using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoadCover.GenerateToken.Models
{
    public class SubscriptionModel
    {
        public string ClientId { get; set; }
        public string SalesId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string CellPhone { get; set; }
        public string Subscription_Id { get; set; }
        public string SubscriptionTypeId { get; set; }
        public string CustomerId { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string CancelledDate { get; set; }
        public string CancelledReason { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}