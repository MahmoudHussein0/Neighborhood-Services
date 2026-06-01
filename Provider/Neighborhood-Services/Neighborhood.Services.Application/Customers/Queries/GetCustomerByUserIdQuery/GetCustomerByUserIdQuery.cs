using MediatR;
using Neighborhood.Services.Application.Customers.DTOs;

namespace Neighborhood.Services.Application.Customers.Queries
{
    public class GetCustomerByUserIdQuery : IRequest<CustomerDetailsDTO>
    {
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
