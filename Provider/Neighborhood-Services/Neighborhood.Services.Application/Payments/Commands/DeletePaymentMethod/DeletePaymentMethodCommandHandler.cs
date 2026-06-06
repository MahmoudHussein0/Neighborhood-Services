using MediatR;
using Neighborhood.Services.Application.Payments.Interfaces;
using Neighborhood.Services.Application.Shared;
namespace Neighborhood.Services.Application.Payments.Commands.DeletePaymentMethod
{
    public class DeletePaymentMethodCommandHandler : IRequestHandler<DeletePaymentMethodCommand, bool>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePaymentMethodCommandHandler(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var paymentMethod = await _paymentRepository.GetByIdAsync(request.PaymentMethodId)
                ?? throw new KeyNotFoundException($"Payment method with ID {request.PaymentMethodId} not found.");

            //only the owner can delete their own payment method
            if (paymentMethod.UserId != request.UserId)
                throw new UnauthorizedAccessException("You are not allowed to delete this payment method.");

            await _paymentRepository.DeleteAsync(request.PaymentMethodId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
