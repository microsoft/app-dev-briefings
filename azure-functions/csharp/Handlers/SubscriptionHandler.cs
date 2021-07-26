using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Company.Sales.Api.Models;
using System;

namespace Company.Sales.Api.Handlers
{
    public static class SubscriptionHandler
    {
        internal const string CollectionName = "Subscriptions";

        [FunctionName("GetSubscriptions")]
        public static IActionResult GetSubscriptions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers/{customerId}/subscriptions")] HttpRequest req,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: CollectionName,
                ConnectionStringSetting = "CosmosConnectionStr",
                SqlQuery = "SELECT * FROM s where s.customerId = {customerId}")] IEnumerable<Subscription> subscriptions,
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
                return new OkObjectResult(subscriptions);
            }
        }

        [FunctionName("CreateSubscription")]
        public static async Task<IActionResult> CreateSubscription(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "customers/{customerId:guid}/subscriptions")] Subscription subscription,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: CustomerHandler.CollectionName,
                ConnectionStringSetting = "CosmosConnectionStr",
                Id = "{customerId}",
                PartitionKey = "{customerId}")] Customer customer,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: CollectionName,
                ConnectionStringSetting = "CosmosConnectionStr")] IAsyncCollector<Subscription> subscriptionCollector,
            Guid customerId,
            ILogger log)
        {
            if (customer == null)
            {
                return new BadRequestObjectResult($"No customer with Id {customerId}");
            }
            else if (subscription.Price <= 0M)
            {
                return new BadRequestObjectResult($"Price invalid.");
            }
            else
            {
                subscription.Id = Guid.NewGuid();
                subscription.CustomerId = customerId;
                subscription.DayOfMonth = DateTime.UtcNow.Day;

                await subscriptionCollector.AddAsync(subscription);

                return new CreatedResult($"/customers/{subscription.CustomerId}/subscription", subscription);
            }
        }

        [FunctionName("RetrieveDailySubscriptions")]
        public static void RetrieveDailySubscriptions(
            [TimerTrigger("0 0 0 * * *")]TimerInfo subscriptionTimer,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: CollectionName,
                ConnectionStringSetting = "CosmosConnectionStr",
                SqlQuery = "SELECT * FROM s where s.dayOfMonth = DateTimePart(\"d\", GetCurrentDateTime())")] IEnumerable<Subscription> subscriptions,
            [ServiceBus(
                "subscription-payments",
                Connection = "ServiceBusConnectionStr")] IAsyncCollector<Subscription> subscriptionCollector,
            ILogger log)
        {
            var tasks = new List<Task>();

            foreach (var subscription in subscriptions)
            {
                tasks.Add(subscriptionCollector.AddAsync(subscription));
            }

            Task.WaitAll(tasks.ToArray());
            log.LogInformation($"Processed {subscriptions.Count()} subscriptions.");
        }
    }
}
