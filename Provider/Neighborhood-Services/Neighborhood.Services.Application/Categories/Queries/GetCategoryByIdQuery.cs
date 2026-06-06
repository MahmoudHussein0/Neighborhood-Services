using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Categories.Queries
{
    public class GetCategoryByIdQuery : IRequest<CategoryDetailsDto>
    {
  
        public int  Id { get; set; }
        public string Lang  { get; set; }

        public GetCategoryByIdQuery(int id, string lang)
        {
            Id = id;
            Lang = lang;
        }
    }

}
