using MediatR;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Domain.Staffs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Staffs.Queries
{
    public record GetStaffsByRoleQuery(StaffRole Role)
        : IRequest<IEnumerable<StaffDto>>;
}
