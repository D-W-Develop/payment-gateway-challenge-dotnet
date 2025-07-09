using System;

using PaymentGateway.Api.PaymentsController.Models;
using PaymentGateway.Api.Services;

using Xunit;

namespace PaymentGateway.Api.Tests.Services;
public class When_Validating_A_Payment_Request
{
    private readonly Random _random = new();
    private readonly PaymentRequest _validPaymentRequest;

    private readonly ValidationService _validationService = new();
    
    public When_Validating_A_Payment_Request()
    {
        _validPaymentRequest = new
        (

            ExpiryYear: (uint)_random.Next(DateTime.Now.Year + 1, DateTime.Now.Year + 10),
            ExpiryMonth: (uint)_random.Next(1, 12),
            Amount: _random.Next(1, 10000),
            CardNumber: 12345678912345,
            Currency: "GBP",
            CVV: 123
        );
    }

    [Fact]
    public void Should_Return_True_If_Request_Valid()
    =>Assert.True(_validationService.IsValid(_validPaymentRequest, out _));

    [Fact]
    public void Should_Return_False_If_CVV_Under_3_Characters()
    => Assert.False(_validationService.IsValid(_validPaymentRequest with {CVV=2 }, out _));

    [Fact]
    public void Should_Return_False_If_CVV_Over_5_Characters()
    => Assert.False(_validationService.IsValid(_validPaymentRequest with { CVV = 12334 }, out _));

    [Fact]
    public void Should_Return_False_If_Card_Number_Under_14_Characters()
    => Assert.False(_validationService.IsValid(_validPaymentRequest with { CardNumber = 1234567890123 }, out _));

    [Fact]
    public void Should_Return_True_If_Card_Number_Over_19_Characters()
    => Assert.False(_validationService.IsValid(_validPaymentRequest with { CardNumber = 12345678901234567890 }, out _));

    [Fact]
    public void Should_Return_False_If_Currency_Less_Than_3_Characters()
    => Assert.False(_validationService.IsValid(_validPaymentRequest with { Currency="AB"}, out _));

    [Fact]
    public void Should_Return_False_If_Currency_More_Than_3_Characters()
    => Assert.False(_validationService.IsValid(_validPaymentRequest with { Currency = "ABCD" }, out _));

    [Fact]
    public void Should_Return_False_If_Currency_Unrecognised()
    => Assert.False(_validationService.IsValid(_validPaymentRequest with { Currency = "ABC" }, out _));

    [Fact]
    public void Should_Return_False_If_Month_Less_Than_1()
    => Assert.False(_validationService.IsValid(_validPaymentRequest with { ExpiryMonth = 0 }, out _));

    [Fact]
    public void Should_Return_False_If_Month_More_Than_12()
    => Assert.False(_validationService.IsValid(_validPaymentRequest with { ExpiryMonth = 13 }, out _));

    [Fact]
    public void Should_Return_True_If_Expiry_Date_Current_Month()
    => Assert.True(_validationService.IsValid(_validPaymentRequest with
    {
        ExpiryMonth = (uint) DateTime.UtcNow.Month,
        ExpiryYear = (uint)DateTime.UtcNow.Year,
    }, out _));

    [Fact]
    public void Should_Return_False_If_Expiry_Date_Last_Month()
    {
        var lastMonth = DateTime.UtcNow.AddMonths(-1);
        Assert.False(_validationService.IsValid(_validPaymentRequest with
        {
            ExpiryMonth = (uint)lastMonth.Month,
            ExpiryYear = (uint)lastMonth.Year,
        }, out _));
    }
}
