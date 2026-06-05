using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianPricing.Interface;

namespace Neighborhood.Services.Application.TechnitianPricing.Commands
{
    internal class DeleteTechnicianPricingForProblemTypeCommandHandler : IRequestHandler<DeleteTechnicianPricingForProblemTypeCommand , bool>
    {
        private readonly ITechnicianPricingRepository _pricingRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTechnicianPricingForProblemTypeCommandHandler(ITechnicianPricingRepository pricingRepo, IUnitOfWork unitOfWork)
        {
            _pricingRepo = pricingRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteTechnicianPricingForProblemTypeCommand request, CancellationToken cancellationToken)
        {
            var pricing = await _pricingRepo.GetByIdAsync(request.Id);

            if (pricing is null)
                throw new NotFoundException("Pricing", request.Id);

            await _pricingRepo.DeleteAsync(pricing.Id);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
