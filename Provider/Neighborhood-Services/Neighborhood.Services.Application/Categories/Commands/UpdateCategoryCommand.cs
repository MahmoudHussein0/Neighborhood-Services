using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Categories.Commands
{
    public class UpdateCategoryCommand :  IRequest<CategoryDto>
    {
        public int  Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string  Icon { get; set; }
        public string  Image { get; set; }
    }
}
