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
    public static class CustomerHandler
    {
        internal const string CollectionName = "Customers";

        [FunctionName("GetCustomers")]
        public static IActionResult GetCustomers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers")] HttpRequest req,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: CollectionName,
                ConnectionStringSetting = "CosmosConnectionStr",
                SqlQuery = "SELECT * FROM c")] IEnumerable<Customer> customers,
            ILogger log)
        {
            log.LogInformation($"{customers.Count()} records retrieved.");
            return new OkObjectResult(customers);
        }

        [FunctionName("GetCustomer")]
        public static IActionResult GetCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers/{id:guid}")] HttpRequest req,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: CollectionName,
                ConnectionStringSetting = "CosmosConnectionStr",
                Id = "{id}",
                PartitionKey = "{id}")] Customer customer,
            Guid id,
            ILogger log)
        {
            if (customer != null)
            {
                return new OkObjectResult(customer);
            }
            else
            {
                return new BadRequestObjectResult($"No customer with Id {id}");
            }
        }

        [FunctionName("CreateCustomer")]
        public static async Task<IActionResult> CreateCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] Customer customer,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: CollectionName,
                ConnectionStringSetting = "CosmosConnectionStr")] IAsyncCollector<Customer> customerCollector,
            ILogger log)
        {
            customer.Id = Guid.NewGuid();
            await customerCollector.AddAsync(customer);
            return new CreatedResult($"/customers/{customer.Id}", customer);
        }
    }
}
