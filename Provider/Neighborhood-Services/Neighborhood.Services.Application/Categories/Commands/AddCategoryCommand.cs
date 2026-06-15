using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Domain.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Neighborhood.Services.Application.Categories.Commands
{
    public  class AddCategoryCommand :IRequest<int>
    {
        public string NameEn  { get; set; }
        public string NameAr  { get; set; }
        public string Icon  { get; set; }
        public string? Image  { get; set; }
    }
}
