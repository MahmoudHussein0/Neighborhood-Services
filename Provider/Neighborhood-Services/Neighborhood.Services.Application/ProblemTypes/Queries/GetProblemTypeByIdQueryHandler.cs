using Mapster;
using MediatR;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Queries
{
    public class GetProblemTypeByIdQueryHandler : IRequestHandler<GetProblemTypeByIdQuery, ProblemTypeDetailsDto>
    {
        private readonly IProblemTypeRepository _problemTypeRepo;

        public GetProblemTypeByIdQueryHandler(IProblemTypeRepository problemTypeRepo)
        {
            _problemTypeRepo = problemTypeRepo;
        }
        public async Task<ProblemTypeDetailsDto> Handle(GetProblemTypeByIdQuery request, CancellationToken cancellationToken)
        {
           var problemTypes = await  _problemTypeRepo.GetByConditionAsync(
                P => P.Id == request.Id, "Category,TechnicionPricing");

          var problemType =  problemTypes.FirstOrDefault();

         if (problemType is null) throw new Exception("ProblemType not found");
            
         return    problemType.Adapt<ProblemTypeDetailsDto>();

        }
    }
}
