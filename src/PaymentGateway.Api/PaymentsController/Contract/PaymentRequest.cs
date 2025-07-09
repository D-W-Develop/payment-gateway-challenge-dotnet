namespace PaymentGateway.Api.PaymentsController.Models;

public record PaymentRequest
(
    ulong CardNumber,
    uint ExpiryMonth,
    uint ExpiryYear,
    string Currency,
    int Amount,
    uint CVV
);