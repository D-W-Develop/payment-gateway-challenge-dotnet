using System.Text.Json.Serialization;

namespace PaymentGateway.Api.BankClients.ABCBank;

public class ABCBankRequest
{
    [JsonPropertyName("card_number")]
    public required string CardNumber { get; init; }
    [JsonPropertyName("expiry_date")]
    public required string ExpiryDate { get; init; }
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }
    [JsonPropertyName("amount")]
    public int Amount { get; init; }
    [JsonPropertyName("cvv")]
    public required string CVV { get; init; }
};