using Microsoft.Azure.WebJobs;
using Company.Sales.Api.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Company.Sales.Api.Handlers
{
    public static class PaymentHandler
    {
        internal const string CollectionName = "Payments";

        [FunctionName("GetPayments")]
        public static IActionResult GetPayments(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers/{customerId}/payments")] HttpRequest req,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: CollectionName,
                ConnectionStringSetting = "CosmosConnectionStr",
                SqlQuery = "SELECT * FROM p where p.customerId = {customerId}")] IEnumerable<Payment> payments,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: CustomerHandler.CollectionName,
                ConnectionStringSetting = "CosmosConnectionStr",
                Id = "{customerId}",
                PartitionKey = "{customerId}")] Customer customer,
            Guid customerId,
            ILogger log)
        {
            if (customer == null)
            {
                return new BadRequestObjectResult($"No customer with Id {customerId}");
            }
            else
            {
                return new OkObjectResult(payments);
            }
        }

        [FunctionName("ProcessSubscriptionPayment")]
        public static void ProcessSubscriptionPayment(
            [ServiceBusTrigger(
                "subscription-payments",
                Connection = "ServiceBusConnectionStr")] Subscription subscription,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: CustomerHandler.CollectionName,
                ConnectionStringSetting = "CosmosConnectionStr",
                Id = "{customerId}",
                PartitionKey = "{customerId}")] Customer customer,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: CollectionName,
                ConnectionStringSetting = "CosmosConnectionStr")] out Payment payment)
        {
            payment = new Payment
            {
                Id = Guid.NewGuid(),
                CustomerId = subscription.CustomerId,
                SubscriptionId = subscription.Id,
                Timestamp = DateTime.UtcNow,
                Amount = subscription.Price,
                EmailAddress = customer.EmailAddress
            };
        }
    }
}
