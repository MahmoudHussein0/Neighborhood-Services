using MediatR;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Commands
{
    public class UpdateProblemTypeCommand : IRequest<UpdateProblemTypeDto>
    {
        public int  Id { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}
