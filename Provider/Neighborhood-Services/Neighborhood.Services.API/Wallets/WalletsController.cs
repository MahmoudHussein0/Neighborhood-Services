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
                MerchantOrderId = transaction.Id.ToString(),
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
                Status = TransactionStatus.Pending
            };

            await _transactionRepository.AddAsync(transaction);
            await _walletRepository.DebitAsync(wallet.Id, request.Amount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Ok(new { message = "Withdrawal initiated successfully.", transactionId = transaction.Id });
        }

        public record WithdrawRequest(decimal Amount);

        [HttpGet("me/transactions/finalize")]
        public async Task<IActionResult> FinalizeTransaction(
            [FromQuery] int merchant_order_id, 
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

            var transaction = await _transactionRepository.GetByIdAsync(merchant_order_id);
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
