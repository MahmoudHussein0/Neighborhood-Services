using MediatR;
using Neighborhood.Services.Application.Customers.DTOs;

namespace Neighborhood.Services.Application.Customers.Queries.GetCurrentCustomerQuery
{
    public class GetCurrentCustomerQuery : IRequest<CustomerDetailsDTO>
    {
    }
}
