using MediatR;

namespace Neighborhood.Services.Application.Customers.Commands
{
    public class DeleteCustomerCommand : IRequest
    {
        public int Id { get; set; }
    }
}
