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
            var problemtTypes = (await _problemTypeRepo.GetByConditionAsync(
                 P =>
                !P.IsDeleted &&
                 ( string.IsNullOrWhiteSpace(request.SearchTerm) || P.Description.ToLower().Contains(request.SearchTerm.ToLower())) 
                 &&
                 (!request.MinPrice.HasValue ||P.MinPrice == request.MinPrice.Value) 
                 && 
                 (!request.MaxPrice.HasValue ||P.MaxPrice == request.MaxPrice.Value)
                 )).OrderByDescending(P => P.CreatedAt);
           return  problemtTypes.Adapt<IReadOnlyList<ProblemTypeDto>>();
        }
    }
}
