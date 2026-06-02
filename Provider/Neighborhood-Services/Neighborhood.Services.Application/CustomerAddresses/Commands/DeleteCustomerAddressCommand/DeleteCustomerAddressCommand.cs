using MediatR;

namespace Neighborhood.Services.Application.CustomerAddresses.Commands
{
    public class DeleteCustomerAddressCommand : IRequest
    {
        public int Id { get; set; }
    }
}
