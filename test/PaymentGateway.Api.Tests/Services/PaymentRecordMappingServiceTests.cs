using System;

using PaymentGateway.Api.Contract;
using PaymentGateway.Api.PaymentsController.Models;
using PaymentGateway.Api.Services;

using Xunit;

namespace PaymentGateway.Api.Tests.Services;
public class When_Mapping_A_Payment_Request_To_A_Payment_Record
{
    private readonly PaymentRecordMappingService _mappingService = new();

    [Fact]
    public void Should_Map_Values_Correctly_Including_Masking_Credit_Card_Number()
    {
        var req = new PaymentRequest
        (

            ExpiryYear: 2026,
            ExpiryMonth: 11,
            Amount: 100,
            CardNumber: 12345678911111,
            Currency: "GBP",
            CVV: 123
        );
        var id = Guid.NewGuid();
        var status = PaymentStatus.Authorized;

        var expectedResult = new PaymentRecord
        (
            Id:id,
            Status:status,
            ExpiryYear: 2026,
            ExpiryMonth: 11,
            Amount: 100,
            CardNumberLastFour: 1111,
            Currency: "GBP"
        );
        var result = _mappingService.MapToPaymentRecord(req, status, id);
        Assert.Equal(expectedResult, result);
    }
}
