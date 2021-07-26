package com.company.sales.api.models;

import java.math.BigDecimal;
import java.util.UUID;

public class Subscription {
    public UUID id;
    public UUID customerId;
    public int dayOfMonth;
    public BigDecimal price;
}
