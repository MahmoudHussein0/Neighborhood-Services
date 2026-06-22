using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Application.TechnitianCategory.Interface;
using Neighborhood.Services.Application.TechnitianPricing.Interface;
using Neighborhood.Services.Domain.TechnicianCategories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianCategory.Queries
{
    public class GetTechnicianCategoryQueryHandler : IRequestHandler<GetTechnicianCategoryQuery, IReadOnlyList<CategoryDto>>
    {

        private readonly ITechnicianCategoryRepository _technicianCategoryRepo;
        private readonly ITechnicianPricingRepository _technicianPricingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianRepository _technicianRepository;

        public GetTechnicianCategoryQueryHandler(ITechnicianCategoryRepository technicianCategoryRepo, ITechnicianPricingRepository technicianPricingRepository, ICurrentUserService currentUserService, ITechnicianRepository technicianRepository)
        {
            _technicianCategoryRepo = technicianCategoryRepo;
            _technicianPricingRepository = technicianPricingRepository;
            _currentUserService = currentUserService;
            _technicianRepository = technicianRepository;
        }
        public async Task<IReadOnlyList<CategoryDto>> Handle(GetTechnicianCategoryQuery request, CancellationToken cancellationToken)
        {
            var lang = request.Lang.ToLower();

            string? userId = _currentUserService.UserId;

            if (userId is null)
                throw new UnauthorizedException("User is unautherized");

            var technician = await _technicianRepository.GetByUserIdAsync(userId);

            if (technician is null)
                throw new ForbiddenException("User is not a technician");



            var problemTypesIds = (await _technicianPricingRepository
                .GetByConditionAsync(tp => tp.TechnicianId == technician.Id)).Select(tp => tp.ProblemTypeId).ToHashSet();


            return (await _technicianCategoryRepo.GetByConditionAsync(TC => (!TC.IsDeleted)
                        &&
                        TC.TechnicianId == technician.Id, "Category,Category.ProblemTypes"))
                             .Select(TC => new CategoryDto
                             {
                                 TechnicianCategoryId = TC.Id,
                                 Id = TC.CategoryId,
                                 Name = lang == "en" ? TC.Category.NameEn : TC.Category.NameAr,
                                 Icon = TC.Category.Icon,
                                 NameAr = TC.Category.NameAr,
                                 NameEn = TC.Category.NameEn,
                                 ProblemTypes = TC.Category.ProblemTypes.Where( p => !problemTypesIds.Contains(p.Id)  ).Select(p => new ProblemTypeDto()
                                 {
                                     Id = p.Id,
                                     Name = lang == "en" ? p.NameEn : p.NameAr,
                                     NameEn = p.NameEn,
                                     NameAr = p.NameAr,
                                     Description = lang == "en" ? p.DescriptionEn : p.DescriptionAr,
                                     DescriptionAr = p.DescriptionAr,
                                     DescriptionEn = p.DescriptionEn,

                                 }).ToList()
                             }).ToList();
        }
    }
}
