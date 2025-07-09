using System.Net;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.BankClients.ABCBank;
using PaymentGateway.Api.PaymentsController.Models;
using PaymentGateway.Api.Repository;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IValidationService validationService, IABCBankClient bankClient, IPaymentRecordMappingService mapper,  PaymentRecordRepository paymentsRepository) : Controller
{
    private readonly IValidationService _validationService = validationService;
    private readonly IABCBankClient _bankClient = bankClient;
    private readonly IPaymentRecordMappingService _mapper = mapper;
    private readonly PaymentRecordRepository _paymentsRepository = paymentsRepository;

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> AddPayment([FromBody]PaymentRequest paymentRequest)
    {
        if (!_validationService.IsValid(paymentRequest, out var validationError)) 
        {
            return new BadRequestObjectResult(new PostPaymentResponse(PaymentStatus.Rejected, null, validationError));
        }

        var paymentSendResult = await _bankClient.TrySendPayment(paymentRequest);
        if (!paymentSendResult.Success)
        {
            return new ObjectResult(new PostPaymentResponse(PaymentStatus.Rejected, null, "Error sending payment to acquiring bank.")) {StatusCode =  (int)HttpStatusCode.BadGateway};
        }

        if (!paymentSendResult.Value!.Authorized) { return new PostPaymentResponse(PaymentStatus.Declined, null, "Payment declined by acquiring bank."); }

        var paymentRecord = _mapper.MapToPaymentRecord(paymentRequest, PaymentStatus.Authorized, Guid.NewGuid());
        _paymentsRepository.AddOrUpdate(paymentRecord);
        return new PostPaymentResponse(PaymentStatus.Authorized, paymentRecord,string.Empty); 
    }

    [HttpGet("{id:guid}")]
    public ActionResult<PaymentRecord> GetPayment(Guid id)
    {
        var paymentResult = _paymentsRepository.TryGet(id);
        return paymentResult.Success 
            ? paymentResult.Value!
            : new NotFoundResult();
    }
}