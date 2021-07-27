# Azure Functions Java Examples
This is a collection of Azure functions examples.

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
    "ServiceBusConnectionStr": "<service bus connection string>",
    "PaymentQueue": "subscription-payments"
  }
}
```

### Running
* ```mvn clean package```
* ```mvn azure-functions:run```