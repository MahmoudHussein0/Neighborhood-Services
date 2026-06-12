using MediatR;
using Neighborhood.Services.Application.Staffs.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Staffs.Queries
{
    public record GetActiveStaffsQuery()
        : IRequest<IEnumerable<StaffDto>>;
}
