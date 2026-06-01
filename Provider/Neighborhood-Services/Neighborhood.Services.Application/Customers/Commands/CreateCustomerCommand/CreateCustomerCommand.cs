using MediatR;

namespace Neighborhood.Services.Application.Customers.Commands
{
    public class CreateCustomerCommand : IRequest<int>
    {
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
