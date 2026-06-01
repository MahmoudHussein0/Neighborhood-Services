using MediatR;

namespace Neighborhood.Services.Application.CustomerAddresses.Commands
{
    public class SetDefaultCustomerAddressCommand : IRequest
    {
        public int Id { get; set; }
    }
}
