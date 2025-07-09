using System;

using PaymentGateway.Api.Contract;
using PaymentGateway.Api.PaymentsController.Models;
using PaymentGateway.Api.Repository;

using Xunit;

namespace PaymentGateway.Api.Tests.Repository;
public class When_Using_The_Payment_Record_Repository
{
    private readonly PaymentRecordRepository _repository = new();

    [Fact]
    public void Can_Add_And_Retrieve_Records()
    {
        var record = new PaymentRecord
        (
            Id: Guid.NewGuid(),
            Status: PaymentStatus.Declined,
            ExpiryYear: 2026,
            ExpiryMonth: 11,
            Amount: 100,
            CardNumberLastFour: 1111,
            Currency: "GBP"
        );

        _repository.AddOrUpdate(record);
        var result = _repository.TryGet(record.Id);
        Assert.True(result.Success);
        Assert.Equal(record, result.Value);
    }

    [Fact]
    public void If_Record_Not_Stored_Returns_Failure()
    {
        var result = _repository.TryGet(Guid.NewGuid());
        Assert.False(result.Success);
    }
}
