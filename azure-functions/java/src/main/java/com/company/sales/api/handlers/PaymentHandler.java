package com.company.sales.api.handlers;

import java.util.Date;
import java.util.UUID;

import com.company.sales.api.models.*;
import com.microsoft.azure.functions.*;
import com.microsoft.azure.functions.annotation.*;

public class PaymentHandler {
    final static String CollectionName = "Payments";

    @FunctionName("GetPayments")
    public HttpResponseMessage getPayments(
        @HttpTrigger(
            name = "req",
            methods = {HttpMethod.GET},
            authLevel = AuthorizationLevel.ANONYMOUS,
            route = "customers/{customerId}/payments") HttpRequestMessage<String> request,
        @CosmosDBInput(name = "paymentsDatabase",
            databaseName = "%DatabaseName%",
            collectionName = CollectionName,
            sqlQuery = "select * from p where p.customerId = {customerId}",
            connectionStringSetting = "CosmosConnectionStr") Payment[] payments,
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
            return request.createResponseBuilder(HttpStatus.OK).body(payments).build();
        }
    }

    @FunctionName("ProcessSubscriptionPayment")
    public void processSubscriptionPayment(
        @ServiceBusQueueTrigger(
            name = "sb",
            queueName = "%PaymentQueue%",
            connection = "ServiceBusConnectionStr") Subscription subscription,
        @CosmosDBInput(name = "customerDatabase",
            databaseName = "%DatabaseName%",
            collectionName = CustomerHandler.CollectionName,
            id = "{customerId}",
            partitionKey = "{customerId}",
            connectionStringSetting = "CosmosConnectionStr") Customer customer,
        @CosmosDBOutput(name = "paymentDatabase",
            databaseName = "%DatabaseName%",
            collectionName = CollectionName,
            connectionStringSetting = "CosmosConnectionStr") OutputBinding<Payment> paymentOutput,
        final ExecutionContext context) {
        Payment payment = new Payment();
        payment.id = UUID.randomUUID();
        payment.customerId = subscription.customerId;
        payment.subscriptionId = subscription.id;
        payment.timestamp = new Date();
        payment.amount = subscription.price;
        payment.emailAddress = customer.emailAddress;

        paymentOutput.setValue(payment);
    }
}
