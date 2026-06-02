using Mapster;
using MediatR;
using Neighborhood.Services.Application.Exceptions;
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
           var problemType = (await  _problemTypeRepo.GetByConditionAsync(P => P.Id == request.Id, "Category,TechnicionPricing")).FirstOrDefault();
           if (problemType is null) throw new NotFoundException("ProblemType" , request.Id);
           return  problemType.Adapt<ProblemTypeDetailsDto>();

        }
    }
}
