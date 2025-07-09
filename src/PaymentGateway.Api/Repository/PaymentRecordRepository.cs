using PaymentGateway.Api.PaymentsController.Models;
using PaymentGateway.Api.Shared;

namespace PaymentGateway.Api.Repository;

public class PaymentRecordRepository
{
    private readonly Dictionary<Guid, PaymentRecord> _payments = [];
    
    public void AddOrUpdate(PaymentRecord payment)
    {
        _payments[payment.Id] = payment;
    }

    public Result<PaymentRecord> TryGet(Guid id)
    {
        return _payments.TryGetValue(id, out var payment)
                ? new(payment)
                : new("Not found.");
    }
}