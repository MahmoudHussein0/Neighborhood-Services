using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Payments.Gateways;
using Neighborhood.Services.Application.Payments.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Transactions.Interfaces;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Application.Wallets.Queries.GetWalletByUserId;
using Neighborhood.Services.Domain.Payments;
using Neighborhood.Services.Domain.Transactions;

namespace Neighborhood.Services.API.Wallets
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WalletsController(
            IMediator mediator,
            ICurrentUserService currentUser,
            IPaymentGatewayService paymentGatewayService,
            ITransactionRepository transactionRepository,
            IWalletRepository walletRepository,
            IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _currentUser = currentUser;
            _paymentGatewayService = paymentGatewayService;
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyWallet()
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _mediator.Send(new GetWalletByUserIdQuery { UserId = userId });
            return Ok(result);
        }

        /// <summary>
        /// Initiates a wallet top-up via a payment gateway.
        /// Returns a redirect URL for the user to complete payment.
        /// The wallet balance is credited only after the gateway callback succeeds.
        /// </summary>
        [HttpPost("me/topup")]
        public async Task<IActionResult> TopUp([FromBody] TopUpWalletRequest request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var wallet = await _walletRepository.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Wallet not found for current user.");

            // Create a single pending transaction — balance is NOT credited yet.
            // The Paymob callback at POST /api/payments/paymob/callback will credit the wallet
            // once Paymob confirms the payment.
            string? token = null;
            if (request.PaymentMethodId.HasValue)
            {
                var method = await _paymentRepository.GetByIdAsync(request.PaymentMethodId.Value);
                if (method != null && method.UserId == userId)
                {
                    token = method.ProviderToken;
                }
            }

            var transaction = new Transaction
            {
                FromWalletId = null,
                ToWalletId = wallet.Id,
                Amount = request.Amount,
                Currency = "EGP",
                Type = TransactionType.TopUp,
                Status = TransactionStatus.Pending,
                PaymentMethodId = request.PaymentMethodId
            };

            await _transactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var gatewayResult = await _paymentGatewayService.InitiateAsync(new PaymentGatewayRequest
            {
                Amount = request.Amount,
                Currency = "EGP",
                Description = "Wallet Top-Up",
                Provider = request.Provider,
                PaymentMethodId = request.PaymentMethodId,
                WalletId = wallet.Id,
                MerchantOrderId = $"{transaction.Id}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                Token = token
            }, cancellationToken);

            if (!gatewayResult.Success)
            {
                transaction.Status = TransactionStatus.Failed;
                await _transactionRepository.UpdateAsync(transaction);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return BadRequest(new { gatewayResult.ErrorMessage });
            }

            return Ok(new
            {
                TransactionId = transaction.Id,
                gatewayResult.RedirectUrl,
                gatewayResult.ProviderReference
            });
        }

        public record TopUpWalletRequest(decimal Amount, int? PaymentMethodId, PaymentProvider Provider);

        /// <summary>
        /// Called by Angular after user returns from Paymob (MPGS 3DS redirect).
        /// Queries Paymob's order API to confirm payment status and updates our transaction.
        /// </summary>
        [HttpPost("me/transactions/verify-payment")]
        public async Task<IActionResult> VerifyPayment(
            [FromBody] VerifyPaymentRequest request,
            [FromServices] Microsoft.Extensions.Options.IOptions<Neighborhood.Services.Infrastructure.Services.Payments.PaymentGatewayOptions> options,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var wallet = await _walletRepository.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Wallet not found for current user.");

            var transaction = await _transactionRepository.GetByIdAsync(request.LocalTransactionId);
            if (transaction == null)
                return NotFound(new { message = "Transaction not found." });

            if (transaction.ToWalletId != wallet.Id && transaction.FromWalletId != wallet.Id)
                return Forbid();

            if (transaction.Status != TransactionStatus.Pending)
                return Ok(new { message = "Transaction already processed.", status = transaction.Status.ToString() });

            // Query Paymob's order API to get the real payment status
            var paymobOptions = options.Value;
            using var http = new System.Net.Http.HttpClient();

            // Step 1: Get Paymob auth token
            var authResp = await http.PostAsJsonAsync(
                $"{paymobOptions.PaymobBaseUrl}/auth/tokens",
                new { api_key = paymobOptions.PaymobApiKey },
                cancellationToken);

            if (!authResp.IsSuccessStatusCode)
                return StatusCode(502, new { message = "Could not authenticate with Paymob." });

            var authBody = await authResp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>(cancellationToken: cancellationToken);
            var authToken = authBody.GetProperty("token").GetString();

            // Step 2: Get order from Paymob by paymobOrderId
            var orderResp = await http.GetAsync(
                $"{paymobOptions.PaymobBaseUrl}/ecommerce/orders/{request.PaymobOrderId}?token={authToken}",
                cancellationToken);

            if (!orderResp.IsSuccessStatusCode)
                return StatusCode(502, new { message = $"Could not retrieve order from Paymob ({(int)orderResp.StatusCode})." });

            var orderBody = await orderResp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>(cancellationToken: cancellationToken);

            // Strategy 1: Check paid_amount_cents — if Paymob reports full amount paid, payment succeeded
            bool paymentSucceeded = false;
            if (orderBody.TryGetProperty("paid_amount_cents", out var paidProp))
            {
                var paid = paidProp.ValueKind == System.Text.Json.JsonValueKind.Number ? paidProp.GetInt64() : 0;
                var amountCents = (long)(transaction.Amount * 100);
                if (paid >= amountCents && paid > 0)
                    paymentSucceeded = true;
            }

            // Strategy 2: If paid_amount_cents wasn't conclusive, query the transactions search endpoint
            if (!paymentSucceeded)
            {
                var txnSearchResp = await http.GetAsync(
                    $"{paymobOptions.PaymobBaseUrl}/acceptance/transactions?order_id={request.PaymobOrderId}&token={authToken}",
                    cancellationToken);

                if (txnSearchResp.IsSuccessStatusCode)
                {
                    var txnSearchBody = await txnSearchResp.Content.ReadAsStringAsync(cancellationToken);
                    // Log raw response for debugging
                    Console.WriteLine($"[VerifyPayment] Paymob txn search: {txnSearchBody}");

                    var txnJson = System.Text.Json.JsonDocument.Parse(txnSearchBody).RootElement;

                    // Response may be a paged object with "results" array, or a direct array
                    System.Text.Json.JsonElement txnArray;
                    if (txnJson.ValueKind == System.Text.Json.JsonValueKind.Array)
                        txnArray = txnJson;
                    else if (txnJson.TryGetProperty("results", out var results))
                        txnArray = results;
                    else
                        txnArray = default;

                    if (txnArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var txn in txnArray.EnumerateArray())
                        {
                            if (txn.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
                            {
                                paymentSucceeded = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Strategy 3: Check transactions array in order (may contain full objects or just IDs)
            if (!paymentSucceeded &&
                orderBody.TryGetProperty("transactions", out var txns) &&
                txns.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                foreach (var txn in txns.EnumerateArray())
                {
                    if (txn.ValueKind == System.Text.Json.JsonValueKind.Object &&
                        txn.TryGetProperty("success", out var successProp) &&
                        successProp.GetBoolean())
                    {
                        paymentSucceeded = true;
                        break;
                    }
                }
            }

            Console.WriteLine($"[VerifyPayment] TransactionId={request.LocalTransactionId}, PaymobOrderId={request.PaymobOrderId}, Succeeded={paymentSucceeded}");

            if (paymentSucceeded)
            {
                transaction.Status = TransactionStatus.Completed;
                if (transaction.Type == TransactionType.TopUp && transaction.ToWalletId.HasValue)
                    await _walletRepository.CreditAsync(transaction.ToWalletId.Value, transaction.Amount);
            }
            else
            {
                transaction.Status = TransactionStatus.Failed;
            }

            await _transactionRepository.UpdateAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Ok(new { message = "Payment verified.", status = transaction.Status.ToString(), success = paymentSucceeded });
        }

        public record VerifyPaymentRequest(int LocalTransactionId, string PaymobOrderId);

        [HttpPost("me/withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var wallet = await _walletRepository.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Wallet not found for current user.");

            if (request.Amount <= 0)
                return BadRequest(new { message = "Withdrawal amount must be greater than zero." });

            if (wallet.Balance < request.Amount)
                return BadRequest(new { message = "Insufficient wallet balance." });

            var transaction = new Transaction
            {
                FromWalletId = wallet.Id,
                Amount = request.Amount,
                Currency = "EGP",
                Type = TransactionType.Withdrawal,
                Status = TransactionStatus.Completed
            };

            await _transactionRepository.AddAsync(transaction);
            await _walletRepository.DebitAsync(wallet.Id, request.Amount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Ok(new { message = "Withdrawal completed successfully.", transactionId = transaction.Id });
        }

        public record WithdrawRequest(decimal Amount);

        [HttpGet("me/transactions/finalize")]
        public async Task<IActionResult> FinalizeTransaction(
            [FromQuery] string merchant_order_id, 
            [FromQuery] bool success, 
            [FromQuery] string? token, 
            [FromQuery] string? masked_pan, 
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var wallet = await _walletRepository.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Wallet not found for current user.");

            // MerchantOrderId format: "{transactionId}-{timestamp}" — extract just the local transaction ID
            var idPart = merchant_order_id?.Contains('-') == true
                ? merchant_order_id.Split('-')[0]
                : merchant_order_id;
            if (!int.TryParse(idPart, out var localTransactionId))
                return BadRequest(new { message = "Invalid merchant order id." });

            var transaction = await _transactionRepository.GetByIdAsync(localTransactionId);
            if (transaction == null)
                return NotFound(new { message = "Transaction not found." });


            // Ensure transaction belongs to user's wallet
            if (transaction.ToWalletId != wallet.Id && transaction.FromWalletId != wallet.Id)
                return Forbid();

            // Save Token if successful
            if (success && !string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(masked_pan))
            {
                var existingMethods = await _paymentRepository.GetByConditionAsync(p => p.UserId == userId && p.ProviderToken == token && !p.IsDeleted);
                if (existingMethods == null || !existingMethods.Any())
                {
                    var newMethod = new PaymentMethod
                    {
                        UserId = userId,
                        PaymentProvider = PaymentProvider.Paymob,
                        PaymentType = PaymentType.CreditCard,
                        ProviderToken = token,
                        LastFourDigits = masked_pan.Length >= 4 ? masked_pan.Substring(masked_pan.Length - 4) : masked_pan,
                        ExpiryMonth = 1, // We don't get this from Paymob redirect
                        ExpiryYear = 2039
                    };
                    await _paymentRepository.AddAsync(newMethod);
                    transaction.PaymentMethodId = newMethod.Id;
                }
            }

            // Only process if it is Pending
            if (transaction.Status != TransactionStatus.Pending)
                return Ok(new { message = "Transaction already processed.", status = transaction.Status.ToString() });

            if (success)
            {
                transaction.Status = TransactionStatus.Completed;
                if (transaction.Type == TransactionType.TopUp)
                {
                    await _walletRepository.CreditAsync(wallet.Id, transaction.Amount);
                }
            }
            else
            {
                transaction.Status = TransactionStatus.Failed;
            }

            await _transactionRepository.UpdateAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Ok(new { message = "Transaction finalized.", status = transaction.Status.ToString() });
        }
    }
}
