using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Payments.Gateways;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Transactions.Interfaces;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Domain.Payments;
using Neighborhood.Services.Domain.Transactions;

namespace Neighborhood.Services.API.Payments
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public PaymentsController(
            IPaymentGatewayService paymentGatewayService,
            ITransactionRepository transactionRepository,
            IWalletRepository walletRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _paymentGatewayService = paymentGatewayService;
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate([FromBody] PaymentInitiationRequest request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var wallet = await _walletRepository.GetByIdAsync(request.WalletId)
                ?? throw new KeyNotFoundException($"Wallet with ID {request.WalletId} not found.");

            if (wallet.UserId != userId)
                return Forbid();

            var transaction = new Transaction
            {
                ToWalletId = wallet.Id,
                Amount = request.Amount,
                Currency = request.Currency,
                Type = request.TransactionType,
                Status = TransactionStatus.Pending,
                PaymentMethodId = request.PaymentMethodId
            };

            await _transactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _paymentGatewayService.InitiateAsync(new PaymentGatewayRequest
            {
                Amount = request.Amount,
                Currency = request.Currency,
                Description = request.Description,
                Provider = request.Provider,
                PaymentMethodId = request.PaymentMethodId,
                WalletId = request.WalletId,
                MerchantOrderId = $"{transaction.Id}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"
            }, cancellationToken);

            if (!result.Success)
            {
                transaction.Status = TransactionStatus.Failed;
                await _transactionRepository.UpdateAsync(transaction);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("paymob/callback")]
        public async Task<IActionResult> PaymobCallback([FromQuery] string hmac, [FromBody] PaymobCallbackPayload payload, [FromServices] Microsoft.Extensions.Options.IOptions<Neighborhood.Services.Infrastructure.Services.Payments.PaymentGatewayOptions> options, CancellationToken cancellationToken)
        {
            var obj = payload.obj;
            if (obj == null || obj.order == null)
                return BadRequest("Invalid payload.");

            var secret = options.Value.PaymobHmacSecret;
            if (!string.IsNullOrWhiteSpace(secret) && !string.IsNullOrWhiteSpace(hmac))
            {
                if (!VerifyPaymobHmac(obj, hmac, secret))
                    return Unauthorized("Invalid HMAC signature.");
            }

            // MerchantOrderId format: "{transactionId}-{timestamp}" — extract just the ID part
            var rawOrderId = obj.order.merchant_order_id;
            var idPart = rawOrderId.Contains('-') ? rawOrderId.Split('-')[0] : rawOrderId;
            if (!int.TryParse(idPart, out var localTransactionId))
                return BadRequest(new { message = "Invalid merchant order id." });

            var transaction = await _transactionRepository.GetByIdAsync(localTransactionId);
            if (transaction == null)
                return NotFound(new { message = "Transaction not found." });

            if (transaction.Status == TransactionStatus.Completed)
                return Ok();

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (obj.success)
                {
                    transaction.Status = TransactionStatus.Completed;
                    if (transaction.ToWalletId.HasValue)
                        await _walletRepository.CreditAsync(transaction.ToWalletId.Value, transaction.Amount);
                }
                else
                {
                    transaction.Status = TransactionStatus.Failed;
                }

                await _transactionRepository.UpdateAsync(transaction);
            }, cancellationToken);

            return Ok();
        }

        private bool VerifyPaymobHmac(PaymobObj obj, string hmac, string secret)
        {
            var concatenatedString = 
                $"{obj.amount_cents}" +
                $"{obj.created_at}" +
                $"{obj.currency}" +
                $"{obj.error_occured.ToString().ToLower()}" +
                $"{obj.has_parent_transaction.ToString().ToLower()}" +
                $"{obj.id}" +
                $"{obj.integration_id}" +
                $"{obj.is_3d_secure.ToString().ToLower()}" +
                $"{obj.is_auth.ToString().ToLower()}" +
                $"{obj.is_capture.ToString().ToLower()}" +
                $"{obj.is_refunded.ToString().ToLower()}" +
                $"{obj.is_standalone_payment.ToString().ToLower()}" +
                $"{obj.is_voided.ToString().ToLower()}" +
                $"{obj.order?.id}" +
                $"{obj.owner}" +
                $"{obj.pending.ToString().ToLower()}" +
                $"{obj.source_data?.pan}" +
                $"{obj.source_data?.sub_type}" +
                $"{obj.source_data?.type}" +
                $"{obj.success.ToString().ToLower()}";

            using var hmacSha512 = new System.Security.Cryptography.HMACSHA512(System.Text.Encoding.UTF8.GetBytes(secret));
            var hash = hmacSha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(concatenatedString));
            var hex = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return hex == hmac.ToLower();
        }

        public record PaymentInitiationRequest(
            decimal Amount,
            string Currency,
            string? Description,
            PaymentProvider Provider,
            int? PaymentMethodId,
            int WalletId,
            TransactionType TransactionType);

        public class PaymobCallbackPayload { public PaymobObj? obj { get; set; } }
        public class PaymobObj
        {
            public int id { get; set; }
            public int amount_cents { get; set; }
            public string created_at { get; set; }
            public string currency { get; set; }
            public bool error_occured { get; set; }
            public bool has_parent_transaction { get; set; }
            public int integration_id { get; set; }
            public bool is_3d_secure { get; set; }
            public bool is_auth { get; set; }
            public bool is_capture { get; set; }
            public bool is_refunded { get; set; }
            public bool is_standalone_payment { get; set; }
            public bool is_voided { get; set; }
            public PaymobOrder order { get; set; }
            public int owner { get; set; }
            public bool pending { get; set; }
            public PaymobSourceData source_data { get; set; }
            public bool success { get; set; }
        }
        public class PaymobOrder { public int id { get; set; } public string merchant_order_id { get; set; } }
        public class PaymobSourceData { public string pan { get; set; } public string sub_type { get; set; } public string type { get; set; } }
    }
}
