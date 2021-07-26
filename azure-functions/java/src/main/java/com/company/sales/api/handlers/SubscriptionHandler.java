package com.company.sales.api.handlers;

import com.microsoft.azure.functions.*;
import com.microsoft.azure.functions.annotation.*;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.UUID;

import com.company.sales.api.models.*;

public class SubscriptionHandler {
    final static String CollectionName = "Subscriptions";

    @FunctionName("GetSubscriptions")
    public HttpResponseMessage getSubscriptions(
            @HttpTrigger(
                name = "req",
                methods = {HttpMethod.GET},
                authLevel = AuthorizationLevel.ANONYMOUS,
                route = "customers/{customerId}/subscriptions") HttpRequestMessage<String> request,
            @CosmosDBInput(name = "subscriptionDatabase",
                databaseName = "%DatabaseName%",
                collectionName = CollectionName,
                sqlQuery = "select * from s where s.customerId = {customerId}",
                connectionStringSetting = "CosmosConnectionStr") Subscription[] subscriptions,
            @CosmosDBInput(name = "customerDatabase",
                databaseName = "%DatabaseName%",
                collectionName = CustomerHandler.CollectionName,
                id = "{customerId}",
                partitionKey = "{customerId}",
                connectionStringSetting = "CosmosConnectionStr") Customer customer,
            @BindingName("customerId") final UUID customerId,
            final ExecutionContext context) {
        if (customer == null) {
            return request.createResponseBuilder(HttpStatus.BAD_REQUEST).body("No customer with Id " + customerId).build();
        } else {
            return request.createResponseBuilder(HttpStatus.OK).body(subscriptions).build();
        }
    }

    @FunctionName("CreateSubscription")
    public HttpResponseMessage createSubscription(
            @HttpTrigger(
                name = "req",
                methods = {HttpMethod.POST},
                authLevel = AuthorizationLevel.ANONYMOUS,
                route = "customers/{customerId}/subscriptions") HttpRequestMessage<Subscription> request,
            @CosmosDBInput(name = "customerDatabase",
                databaseName = "%DatabaseName%",
                collectionName = CustomerHandler.CollectionName,
                id = "{customerId}",
                partitionKey = "{customerId}",
                connectionStringSetting = "CosmosConnectionStr") Customer customer,
            @CosmosDBOutput(name = "subscriptionDatabase",
                databaseName = "%DatabaseName%",
                collectionName = CollectionName,
                connectionStringSetting = "CosmosConnectionStr") OutputBinding<Subscription> subscriptionOutput,
            @BindingName("customerId") final UUID customerId,
            final ExecutionContext context) {
        Subscription subscription = request.getBody();

        if (customer == null) {
            return request.createResponseBuilder(HttpStatus.BAD_REQUEST).body("No customer with Id " + customerId).build();
        } else if (subscription.price.compareTo(BigDecimal.ZERO) <= 0) {
            return request.createResponseBuilder(HttpStatus.BAD_REQUEST).body("Price invalid.").build();
        } else {
            subscription.customerId = customerId;
            subscription.id = UUID.randomUUID();
            subscription.dayOfMonth = LocalDate.now().getDayOfMonth();
            subscriptionOutput.setValue(subscription);

            return request.createResponseBuilder(HttpStatus.CREATED).body(subscription).build();
        }
    }

    @FunctionName("RetrieveDailySubscriptions")
    public void retrieveDailySubscriptions(
        @TimerTrigger(
            name = "timer",
            schedule = "0 0 0 * * *") String timerInfo,
        @CosmosDBInput(name = "subscriptionDatabase",
            databaseName = "%DatabaseName%",
            collectionName = CollectionName,
            sqlQuery = "SELECT * FROM s where s.dayOfMonth = DateTimePart(\"d\", GetCurrentDateTime())",
            connectionStringSetting = "CosmosConnectionStr") Subscription[] subscriptions,
        @ServiceBusQueueOutput(name = "sb",
            queueName = "%PaymentQueue%",
            connection = "ServiceBusConnectionStr") OutputBinding<Subscription[]> subscriptionOutput,
        final ExecutionContext context) {
        subscriptionOutput.setValue(subscriptions);
    }
}
