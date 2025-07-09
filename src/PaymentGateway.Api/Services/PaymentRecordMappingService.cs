using PaymentGateway.Api.PaymentsController.Models;

namespace PaymentGateway.Api.Services;

public interface IPaymentRecordMappingService
{
    public PaymentRecord MapToPaymentRecord(PaymentRequest req, PaymentStatus status, Guid id);
}

public class PaymentRecordMappingService : IPaymentRecordMappingService
{
    public PaymentRecord MapToPaymentRecord(PaymentRequest req, PaymentStatus status, Guid id)
    {
        return new
        (
            Id:id,
            Status: status,
            CardNumberLastFour:uint.Parse(req.CardNumber.ToString()[^4..]),
            ExpiryMonth: req.ExpiryMonth,
            ExpiryYear: req.ExpiryYear,
            Currency: req.Currency,
            Amount: req.Amount
        );
    }
}
