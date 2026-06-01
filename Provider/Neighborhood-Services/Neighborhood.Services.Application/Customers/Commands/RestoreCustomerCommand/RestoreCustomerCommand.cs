using MediatR;

namespace Neighborhood.Services.Application.Customers.Commands
{
    public class RestoreCustomerCommand : IRequest
    {
        public int Id { get; set; }
    }
}
