using System.Text.Json.Serialization;
using PaymentGateway.Api.BankClients.ABCBank;
using PaymentGateway.Api.Repository;
using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var abcBankUrl = builder.Configuration.GetValue<string>("ABCBankUrl");
//Services
builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSingleton<IValidationService, ValidationService>();
builder.Services.AddHttpClient<IABCBankClient, ABCBankClient>(c => c.BaseAddress = new(abcBankUrl!));
builder.Services.AddSingleton<IPaymentRecordMappingService, PaymentRecordMappingService>();
builder.Services.AddSingleton<PaymentRecordRepository>();
var app = builder.Build();

//Config
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
