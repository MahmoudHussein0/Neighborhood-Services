using MediatR;
using Neighborhood.Services.Application.Customers.DTOs;

namespace Neighborhood.Services.Application.Customers.Queries
{
    public class GetCustomerByIdQuery : IRequest<CustomerDetailsDTO>
    {
        public int Id { get; set; }
    }
}
