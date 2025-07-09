using PaymentGateway.Api.PaymentsController.Models;

namespace PaymentGateway.Api.Services;

public interface IValidationService
{
    public bool IsValid(PaymentRequest request, out string validationError);
}
public class ValidationService : IValidationService
{
    private readonly int _minCardNumberLength = 14;
    private readonly int _maxCardNumberLength = 19;
    private readonly HashSet<string> _validCurrencies = ["GBP", "USD", "JPY"];
    private readonly int _minCVVLength = 3;
    private readonly int _maxCVVLength = 4;

    public bool IsValid(PaymentRequest request, out string validationError) 
    {
        validationError = string.Empty;
        if (!HasValidCardNumber(request, out var cardNumberError)){ validationError += cardNumberError; }
        if (!HasFutureExpiryDate(request, out var expiryDateError)) { validationError += expiryDateError; }
        if (!HasValidCurrency(request, out var currencyError)){ validationError += currencyError; }
        if (!HasValidCVV(request, out var cvvError)) { validationError += cvvError; }
        return validationError == string.Empty; 
    }

    private bool HasValidCardNumber(PaymentRequest request, out string validationError)
    {
        validationError = string.Empty;
        var length = request.CardNumber.ToString().Length;
        if (length < _minCardNumberLength) { validationError += $"Card number must be at least {_minCardNumberLength} digits. "; }
        if (length > _maxCardNumberLength) { validationError += $"Card number must be max {_maxCardNumberLength} digits. "; }
        return validationError == string.Empty;
    }

    private static bool HasFutureExpiryDate(PaymentRequest request, out string validationError)
    {
        validationError = string.Empty;
        if (request.ExpiryMonth < 1 || request.ExpiryMonth > 12)
        {
            validationError += "Expiry month must be 1-12";
            return false;
        }
        var expiry = new DateOnly((int)request.ExpiryYear, (int)request.ExpiryMonth,1).AddMonths(1);
        if (DateOnly.FromDateTime(DateTime.UtcNow) >= expiry) { validationError += "Expiry date must be in the future. "; }
        return validationError == string.Empty;
    }

    private bool HasValidCurrency(PaymentRequest request, out string validationError)
    {
        validationError = string.Empty;
        if (request.Currency.Length != 3) { validationError += "Currency code must be three characters. "; }
        if (!_validCurrencies.Contains(request.Currency)) { validationError += "Currency code not recognised. "; }
        return validationError == string.Empty;
    }

    private bool HasValidCVV(PaymentRequest request, out string validationError)
    {
        validationError = string.Empty;
        var length = request.CVV.ToString().Length;
        if (length < _minCVVLength || length > _maxCVVLength) { validationError += "CVV must be 3-4 digits."; }
        return validationError == string.Empty;
    }
}
