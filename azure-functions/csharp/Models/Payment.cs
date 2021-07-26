using System;
using Newtonsoft.Json;

namespace Company.Sales.Api.Models
{
    public class Payment
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("customerId")]
        public Guid CustomerId { get; set; }

        [JsonProperty("subscriptionId")]
        public Guid SubscriptionId { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }
    }
}