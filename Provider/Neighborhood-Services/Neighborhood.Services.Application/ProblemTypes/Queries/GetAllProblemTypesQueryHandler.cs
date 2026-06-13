using Mapster;
using MediatR;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Queries
{
    public class GetAllProblemTypesQueryHandler : IRequestHandler<GetAllProblemTypesQuery, IReadOnlyList<ProblemTypeDto>>
    {
        private readonly IProblemTypeRepository _problemTypeRepo;

        public GetAllProblemTypesQueryHandler(IProblemTypeRepository problemTypeRepo)
        {
            _problemTypeRepo = problemTypeRepo;
        }
        public async Task<IReadOnlyList<ProblemTypeDto>> Handle(GetAllProblemTypesQuery request, CancellationToken cancellationToken)
        {

            var lang = request.Lang.ToLower();

            var problemTypes = (await _problemTypeRepo.GetByConditionAsync(
             p =>
            !p.IsDeleted
            &&
            (lang == "en" ? (string.IsNullOrEmpty(request.SearchTerm))  ||(p.DescriptionEn != null && p.DescriptionEn.Contains(request.SearchTerm))  : (string.IsNullOrEmpty(request.SearchTerm)) || (p.DescriptionAr != null && p.DescriptionAr.Contains(request.SearchTerm)))
            &&
            (!request.MinPrice.HasValue || p.MinPrice >= request.MinPrice.Value)
            &&
            (!request.MaxPrice.HasValue || p.MaxPrice <= request.MaxPrice.Value))).OrderByDescending(p => p.CreatedAt);


            var problemTypeDto = problemTypes.Select(P => new ProblemTypeDto
            {
                Id = P.Id ,
                Name = lang == "en" ? P.NameEn : P.NameAr,
                Description = lang == "en" ? P.DescriptionEn : P.DescriptionAr,
                MinPrice = P.MinPrice ,
                MaxPrice = P.MaxPrice 
                
            }).ToList();

            return problemTypeDto;
        }
    }
}
