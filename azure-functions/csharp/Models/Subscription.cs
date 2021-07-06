using System;
using Newtonsoft.Json;

namespace Company.Sales.Api.Models
{
    public class Subscription
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("customerId")]
        public Guid CustomerId { get; set; }
        
        [JsonProperty("dayOfMonth")]
        public int DayOfMonth { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}