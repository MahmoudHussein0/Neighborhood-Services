using MediatR;

namespace Neighborhood.Services.Application.Customers.Commands
{
    public class ActivateCustomerCommand : IRequest
    {
        public int Id { get; set; }
    }
}
