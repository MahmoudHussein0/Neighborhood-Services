using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Payments.Gateways;
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
        private readonly IUnitOfWork _unitOfWork;

        public WalletsController(
            IMediator mediator,
            ICurrentUserService currentUser,
            IPaymentGatewayService paymentGatewayService,
            ITransactionRepository transactionRepository,
            IWalletRepository walletRepository,
            IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _currentUser = currentUser;
            _paymentGatewayService = paymentGatewayService;
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
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
            var transaction = new Transaction
            {
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
                MerchantOrderId = transaction.Id.ToString()
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

        public record TopUpWalletRequest(decimal Amount, int PaymentMethodId, PaymentProvider Provider);
    }
}
