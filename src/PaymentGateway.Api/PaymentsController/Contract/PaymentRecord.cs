namespace PaymentGateway.Api.PaymentsController.Models;

public record PaymentRecord
(
    Guid Id,
    PaymentStatus Status,
    uint CardNumberLastFour,
    uint ExpiryMonth,
    uint ExpiryYear,
    string Currency,
    int Amount
);
