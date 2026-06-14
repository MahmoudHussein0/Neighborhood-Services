using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.PromoCodes.DTOs;
using Neighborhood.Services.Application.PromoCodes.Interface;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.PromoCodes;

namespace Neighborhood.Services.Application.PromoCodes.Commands.ApplyPromoCode
{
    public class ApplyPromoCodeCommandHandler : IRequestHandler<ApplyPromoCodeCommand, PromoCodeResponseDto>
    {
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly IPromoCodeUsageRepository _promoCodeUsageRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ApplyPromoCodeCommandHandler(
            IPromoCodeRepository promoCodeRepository,
            IPromoCodeUsageRepository promoCodeUsageRepository,
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork)
        {
            _promoCodeRepository = promoCodeRepository;
            _promoCodeUsageRepository = promoCodeUsageRepository;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PromoCodeResponseDto> Handle(ApplyPromoCodeCommand request, CancellationToken cancellationToken)
        {
            var normalizedCode = request.Code.Trim();

            if (string.IsNullOrWhiteSpace(normalizedCode))
                throw new BadRequestException("Promo code is required.");

            var isValid = await _promoCodeRepository.IsValidAsync(normalizedCode);

            if (!isValid)
                throw new BadRequestException($"Promo code '{normalizedCode}' is invalid or expired.");

            var promoCode = await _promoCodeRepository.GetByCodeAsync(normalizedCode)
                ?? throw new KeyNotFoundException($"Promo code '{normalizedCode}' not found.");

            var booking = (await _bookingRepository.GetByConditionAsync(
                b => b.Id == request.BookingId,
                includeProperties: "Customer"))
                .FirstOrDefault()
                ?? throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found.");

            if (booking.Customer.ApplicationUserId != request.UserId)
                throw new BadRequestException("Promo code can only be applied by the booking customer.");

            if (booking.PromoCodeId is not null)
                throw new BadRequestException("A promo code has already been applied to this booking.");

            var alreadyUsed = await _promoCodeUsageRepository.HasUserUsedPromoAsync(request.UserId, promoCode.Id);

            if (alreadyUsed)
                throw new BadRequestException("You have already used this promo code.");

            var baseAmount = booking.FinalPrice > 0 ? booking.FinalPrice : booking.EstimatedPrice;
            var discountAmount = Math.Round(baseAmount * promoCode.DiscountPercentage / 100, 2);

            booking.PromoCodeId = promoCode.Id;
            booking.FinalPrice = Math.Max(0, baseAmount - discountAmount);
            booking.UpdatedAt = DateTime.UtcNow;

            var usage = new PromoCodeUsage
            {
                PromoCodeId = promoCode.Id,
                UserId = request.UserId,
                BookingId = request.BookingId,
                UsedAt = DateTime.UtcNow
            };

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var usageIncremented = await _promoCodeRepository.TryIncrementUsageAsync(promoCode.Id);

                if (!usageIncremented)
                    throw new BadRequestException($"Promo code '{normalizedCode}' is no longer available.");

                await _promoCodeUsageRepository.AddAsync(usage);
                await _bookingRepository.UpdateAsync(booking);
            }, cancellationToken);

            return new PromoCodeResponseDto
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                DiscountPercentage = promoCode.DiscountPercentage,
                MaxUses = promoCode.MaxUses,
                UsedCount = promoCode.UsedCount + 1,
                ExpiresAt = promoCode.ExpiresAt,
                IsActive = promoCode.IsActive,
                CreatedAt = promoCode.CreatedAt
            };
        }
    }
}
