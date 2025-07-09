using PaymentGateway.Api.PaymentsController.Models;
using PaymentGateway.Api.Shared;

namespace PaymentGateway.Api.BankClients.ABCBank;

public interface IABCBankClient
{
    public Task<Result<ABCBankResponse>> TrySendPayment(PaymentRequest request);
}

public class ABCBankClient(HttpClient client) : IABCBankClient
{
    private readonly HttpClient _client = client;
    public async Task<Result<ABCBankResponse>> TrySendPayment(PaymentRequest request)
    {
        var req = Map(request);
        var response =await _client.PostAsJsonAsync("payments",req);
        if (!response.IsSuccessStatusCode) { return new($"HTTP failure code {response.StatusCode} received from ABCBank."); }
        var body = await response.Content.ReadFromJsonAsync<ABCBankResponse>();
        return body == null ? new("Indecipherable response received from ABCBank.") : new(body);
    }

    private static ABCBankRequest Map(PaymentRequest req)
    {
        return new()
        {
            CardNumber = req.CardNumber.ToString(),
            ExpiryDate = $"{(req.ExpiryMonth<10 ? 0 : "")}{req.ExpiryMonth}/{req.ExpiryYear}",
            Currency = req.Currency,
            Amount = req.Amount,
            CVV = req.CVV.ToString()
        };
    }
}
