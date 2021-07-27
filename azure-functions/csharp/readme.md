# Azure Functions C# Examples
This is a collection of Azure functions examples.

## Local Development
Ensure you have:
* .NET 3.1 SDK installed
* Azure Functions Core Tools installed
* Include a local.settings.json file with the following information
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "<storage account connection string>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "DatabaseName": "Sales",
    "CosmosConnectionStr": "<cosmos connection string>",
    "ServiceBusConnectionStr": "<service bus connection string>",
    "PaymentQueue": "subscription-payments"
  }
}
```