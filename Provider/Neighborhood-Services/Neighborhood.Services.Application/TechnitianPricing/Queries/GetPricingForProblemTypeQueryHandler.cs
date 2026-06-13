using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;
using Neighborhood.Services.Application.TechnitianPricing.Interface;

namespace Neighborhood.Services.Application.TechnitianPricing.Queries
{
    public class GetPricingForProblemTypeQueryHandler : IRequestHandler<GetPricingForProblemTypeQuery, IReadOnlyList<TechnicianPricingDto>>
    {
        private readonly ITechnicianPricingRepository _technicianPricingRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianRepository _technicianRepo;

        public GetPricingForProblemTypeQueryHandler(ITechnicianPricingRepository technicianPricingRepo , ICurrentUserService currentUserService, ITechnicianRepository technicianRepo)
        {
            _technicianPricingRepo = technicianPricingRepo;
            _currentUserService = currentUserService;
            _technicianRepo = technicianRepo;
        }

        public async Task<IReadOnlyList<TechnicianPricingDto>> Handle(GetPricingForProblemTypeQuery request, CancellationToken cancellationToken)
        {

            string? userId = _currentUserService.UserId;
            var lang = request.Lang.ToLower();


            if (userId is null)
                throw new UnauthorizedException("User not autherized");


            var technician = await _technicianRepo.GetByUserIdAsync(userId);

            if (technician is null)
                throw new ForbiddenException("User is not a technician");

            var pricing = await _technicianPricingRepo.GetByConditionAsync(TP => (!TP.IsDeleted)  &&  TP.TechnicianId == technician.Id, "ProblemType,Technician");

            if (pricing == null || !pricing.Any())
                return [];

            return pricing
                .Select(TP => new TechnicianPricingDto
                {
                    Id = TP.Id,
                    NationalId = TP.Technician.NationalId,
                    Experience  = TP.Technician.Experience,
                    Rating = TP.Technician.Rating,
                    MaxTravelDistance =TP.Technician.MaxTravelDistance,
                    VerificationStatus = TP.Technician.VerificationStatus,
                    ProblemTypeId = TP.ProblemTypeId,
                    ProblemTypeDescription = lang == "en" ? TP.ProblemType.DescriptionEn : TP.ProblemType.DescriptionAr,
                    ProblemTypeName = lang == "en" ?  TP.ProblemType.NameEn : TP.ProblemType.NameAr,
                    TechMaxPrice = TP.MaxPrice,
                    TechMinPrice = TP.MinPrice ,
                })
                .ToList();
        }
    }
}
