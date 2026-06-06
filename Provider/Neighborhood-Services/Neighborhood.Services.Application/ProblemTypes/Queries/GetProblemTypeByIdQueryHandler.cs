using Mapster;
using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;
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

            var lang = request.Lang.ToLower();

           var problemType = (await  _problemTypeRepo.GetByConditionAsync(P =>  (!P.IsDeleted) &&  P.Id == request.Id, "Category,TechnicionPricing")).FirstOrDefault();
           if (problemType is null) throw new NotFoundException("ProblemType" , request.Id);

            var problemTypeDetailsDto = new ProblemTypeDetailsDto()
            {
                Name = lang == "en" ?  problemType.NameEn : problemType.NameAr,
                Description = lang == "en" ?  problemType.DescriptionEn : problemType.DescriptionAr,
                MaxPrice = problemType.MaxPrice,
                MinPrice = problemType.MinPrice,
                CategoryName = lang == "en"  ? problemType.Category.NameEn : problemType.Category.NameAr,
                CategoryIcon = problemType.Category.Icon,
            };

             var techPriceing =   problemType.TechnicionPricing.Select(TP => new TechnicianPricingDto()
            {
                ProblemTypeName = lang == "en"  ? TP.ProblemType.NameEn : TP.ProblemType.NameAr,
                TechPriceMaxPrice = TP.MaxPrice,
                TechPriceMinPrice = TP.MinPrice
            }).ToList();

            problemTypeDetailsDto.TechnicionPricing.AddRange(techPriceing);

           return problemTypeDetailsDto;
        }
    }
}
