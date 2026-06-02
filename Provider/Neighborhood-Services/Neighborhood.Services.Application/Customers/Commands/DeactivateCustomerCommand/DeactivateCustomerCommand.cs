using MediatR;

namespace Neighborhood.Services.Application.Customers.Commands
{
    public class DeactivateCustomerCommand : IRequest
    {
        public int Id { get; set; }
    }
}
