# Azure Functions Java Examples
This is a collection of Azure functions examples.

## Azure Setup
The following services must be set up:
* Function App
  * App settings:
    * DatabaseName: Sales
    * CosmosConnectionStr: cosmos_connection_string
    * ServiceBusConnectionStr: service_bus_connection_string
* Storage Account
* Cosmos Core API Account
  * Database: Sales
    * Container: Customers
    * Container: Subscriptions
    * Container: Payments
* Application Insights
* Service Bus
  * Queue: subscription-payments


## Local Development
### Setup
Ensure you have:
* Java 8 SDK installed
* Maven installed
* Azure Functions Core Tools installed
* Include a local.settings.json file with the following information
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "<storage account connection string>",
    "FUNCTIONS_WORKER_RUNTIME": "java",
    "DatabaseName": "Sales",
    "CosmosConnectionStr": "<cosmos connection string>",
    "ServiceBusConnectionStr": "<service bus connection string>"
  }
}
```

### Running
* ```mvn clean package```
* ```mvn azure-functions:run```