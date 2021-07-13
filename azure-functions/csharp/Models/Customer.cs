using System;
using Newtonsoft.Json;

namespace Company.Sales.Api.Models
{
    public class Customer
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string lastName { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set;}
    }
}