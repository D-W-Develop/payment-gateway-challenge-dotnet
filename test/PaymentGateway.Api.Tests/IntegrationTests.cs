using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.PaymentsController.Models;

using Xunit;

namespace PaymentGateway.Api.Tests;

public abstract class IntegrationTests
{
    protected readonly Random _random = new();

    protected readonly HttpClient _appClient;
    protected readonly JsonSerializerOptions _options;
    public IntegrationTests()
    {
        var webApplicationFactory = new WebApplicationFactory<Controllers.PaymentsController>();
        _appClient = webApplicationFactory.CreateClient();
        _options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        _options.Converters.Add(new JsonStringEnumConverter());
    }
}

public class When_Making_Payments : IntegrationTests
{
    [Fact]
    public async Task If_Request_Invalid_Returns_400_And_Rejected()
    {
        var req = BuildPaymentRequest((ulong)_random.Next(1111, 9999) /*Card too short*/);
        await SendPostRequestAndAssertOnStatuses(req, HttpStatusCode.BadRequest, PaymentStatus.Rejected);
    }

    [Fact]
    public async Task If_Error_Calling_Bank_Returns_502_And_Rejected()
    {
        var req = BuildPaymentRequest(12345678912340 /*Card ending in 0 triggers failure in test bank API*/);
        await SendPostRequestAndAssertOnStatuses(req, HttpStatusCode.BadGateway, PaymentStatus.Rejected);
    }

    [Fact]
    public async Task If_Bank_Declines_Returns_200_And_Declined()
    {
        var req = BuildPaymentRequest(12345678912346 /*Card ending in even number triggers decline in test bank API*/);
        await SendPostRequestAndAssertOnStatuses(req, HttpStatusCode.OK, PaymentStatus.Declined);
    }

    [Fact]
    public async Task If_Bank_Authorizes_Returns_200_And_Authorized_And_Can_Get_PaymentRecord()
    {
        var req = BuildPaymentRequest(12345678912347 /*Card ending in odd number triggers authorize in test bank API*/);
        var postResponse = await SendPostRequestAndAssertOnStatuses(req, HttpStatusCode.OK, PaymentStatus.Authorized);
        var getResponse = await _appClient.GetAsync($"/api/Payments/{postResponse.Payment!.Id}");
        var getResponseBody = await getResponse.Content.ReadFromJsonAsync<PaymentRecord>(_options);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal(postResponse.Payment.Id, getResponseBody!.Id);
    }


    private PaymentRequest BuildPaymentRequest(ulong cardNumber)
    {
        return new
        (

            ExpiryYear:(uint)_random.Next(DateTime.Now.Year + 1, DateTime.Now.Year + 10),
            ExpiryMonth:(uint)_random.Next(1, 12),
            Amount: _random.Next(1, 10000),
            CardNumber: cardNumber,
            Currency: "GBP",
            CVV: 123
        );
    }

    private async Task<PostPaymentResponse> SendPostRequestAndAssertOnStatuses(PaymentRequest req, HttpStatusCode expectedHttpStatus, PaymentStatus expectedPaymentStatus)
    {
        var postResponse = await _appClient.PostAsJsonAsync("/api/Payments", req);
        var postResponseBody = await postResponse.Content.ReadFromJsonAsync<PostPaymentResponse>(_options);
        Assert.Equal(expectedHttpStatus, postResponse.StatusCode);
        Assert.Equal(expectedPaymentStatus, postResponseBody!.Status);
        return postResponseBody;
    }
}

public class When_Getting_Payments : IntegrationTests
{
    [Fact]
    public async Task If_Payment_Not_Found_Returns_404()
    {
        var response = await _appClient.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}