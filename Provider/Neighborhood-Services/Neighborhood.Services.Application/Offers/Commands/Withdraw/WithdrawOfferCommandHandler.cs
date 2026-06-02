using MediatR;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Offers.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Offers.Commands.Withdraw
{
    public class WithdrawOfferCommandHandler : IRequestHandler<WithdrawOfferCommand,bool>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        public WithdrawOfferCommandHandler(IUnitOfWork unitOfWork, IOfferRepository offerRepository, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _offerRepository = offerRepository;
        }
        public async Task<bool> Handle(WithdrawOfferCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? throw new UnauthorizedException("User is not Authorized");

            var offer = await _offerRepository.GetOfferWithDetailsAsync(request.OfferId);

            if (offer == null)
                throw new NotFoundException(nameof(offer), request.OfferId);
            if (offer.Technician.ApplicationUserId != userId)
                throw new ForbiddenException("You don't have access to this offer.");
            if (offer.Status != OfferStatus.Pending)
                throw new BadRequestException($"Only a pending offer can be withdrawn. Current status: {offer.Status}.");
            offer.Status = OfferStatus.Withdrawn;
            await _offerRepository.UpdateAsync(offer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
