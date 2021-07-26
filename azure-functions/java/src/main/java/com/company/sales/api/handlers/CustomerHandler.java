package com.company.sales.api.handlers;

import com.microsoft.azure.functions.*;
import com.microsoft.azure.functions.annotation.*;

import java.util.UUID;

import com.company.sales.api.models.*;

public class CustomerHandler {
    final static String CollectionName = "Customers";

    @FunctionName("GetCustomers")
    public HttpResponseMessage getCustomers(
            @HttpTrigger(
                name = "req",
                methods = {HttpMethod.GET},
                authLevel = AuthorizationLevel.ANONYMOUS,
                route = "customers") HttpRequestMessage<String> request,
            @CosmosDBInput(name = "database",
                databaseName = "%DatabaseName%",
                collectionName = CollectionName,
                sqlQuery = "select * from c",
                connectionStringSetting = "CosmosConnectionStr") Customer[] customers,
            final ExecutionContext context) {
        context.getLogger().info(customers.length + " records retrieved.");

        // Parse query parameter
        // final String query = request.getQueryParameters().get("name");
        // final String name = request.getBody().orElse(query);

        return request.createResponseBuilder(HttpStatus.OK).body(customers).build();
    }

    @FunctionName("GetCustomer")
    public HttpResponseMessage getCustomer(
            @HttpTrigger(
                name = "req",
                methods = {HttpMethod.GET},
                authLevel = AuthorizationLevel.ANONYMOUS,
                route = "customers/{id}") HttpRequestMessage<String> request,
            @CosmosDBInput(name = "database",
                databaseName = "%DatabaseName%",
                collectionName = CollectionName,
                id = "{id}",
                partitionKey = "{id}",
                connectionStringSetting = "CosmosConnectionStr") Customer customer,
            @BindingName("id") final UUID id,
            final ExecutionContext context) {
        if (customer != null) {
            return request.createResponseBuilder(HttpStatus.OK).body(customer).build();
        } else {
            return request.createResponseBuilder(HttpStatus.NOT_FOUND).body("No customer with Id " + id).build();
        }
    }

    @FunctionName("CreateCustomer")
    public HttpResponseMessage createCustomer(
            @HttpTrigger(
                name = "req",
                methods = {HttpMethod.POST},
                authLevel = AuthorizationLevel.ANONYMOUS,
                route = "customers") HttpRequestMessage<Customer> request,
            @CosmosDBOutput(name = "database",
                databaseName = "%DatabaseName%",
                collectionName = CollectionName,
                connectionStringSetting = "CosmosConnectionStr") OutputBinding<Customer> customerOutput,
            final ExecutionContext context) {
        Customer customer = request.getBody();
        customer.id = UUID.randomUUID();
        customerOutput.setValue(customer);

        return request.createResponseBuilder(HttpStatus.CREATED).body(customer).build();
    }
}
