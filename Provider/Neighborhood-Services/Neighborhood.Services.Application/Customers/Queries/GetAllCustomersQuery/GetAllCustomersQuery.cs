using MediatR;
using Neighborhood.Services.Application.Customers.DTOs;

namespace Neighborhood.Services.Application.Customers.Queries
{
    public class GetAllCustomersQuery : IRequest<List<CustomerSummaryDTO>>
    {
    }
}
