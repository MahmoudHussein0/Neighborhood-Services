using MediatR;

namespace Neighborhood.Services.Application.CustomerAddresses.Commands
{
    public class RestoreCustomerAddressCommand : IRequest
    {
        public int Id { get; set; }
    }
}
