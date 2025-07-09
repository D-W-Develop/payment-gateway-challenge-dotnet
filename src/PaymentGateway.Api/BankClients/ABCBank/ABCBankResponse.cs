using System.Text.Json.Serialization;

namespace PaymentGateway.Api.BankClients.ABCBank;

public record ABCBankResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; init; }
    [JsonPropertyName("authorization_code")]
    public required string AuthorizationCode { get; init; }
}
