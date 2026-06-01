using MediatR;
using Neighborhood.Services.Application.Customers.DTOs;

namespace Neighborhood.Services.Application.Customers.Queries
{
    public class GetDeletedCustomersQuery : IRequest<List<CustomerSummaryDTO>>
    {
    }
}
