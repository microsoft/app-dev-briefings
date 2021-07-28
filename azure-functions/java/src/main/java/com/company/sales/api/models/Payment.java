package com.company.sales.api.models;

import java.math.BigDecimal;
import java.util.UUID;

public class Payment {
    public UUID id;
    public UUID customerId;
    public UUID subscriptionId;
    public String timestamp;
    public BigDecimal amount;
    public String emailAddress;
}
