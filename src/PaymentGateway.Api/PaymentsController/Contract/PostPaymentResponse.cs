namespace PaymentGateway.Api.PaymentsController.Models;

public record PostPaymentResponse(PaymentStatus Status, PaymentRecord? Payment, string? Error);