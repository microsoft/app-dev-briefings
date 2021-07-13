using Microsoft.Azure.WebJobs;
using Company.Sales.Api.Models;
using System;

namespace Company.Sales.Api.Handlers
{
    public static class PaymentHandler
    {
        internal const string CollectionName = "Payments";

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
                Amount = subscription.Price
            };
        }
    }
}
