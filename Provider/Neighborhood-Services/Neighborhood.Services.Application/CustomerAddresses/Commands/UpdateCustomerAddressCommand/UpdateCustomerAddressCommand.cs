using MediatR;
using Neighborhood.Services.Domain.CustomerAddresses;

namespace Neighborhood.Services.Application.CustomerAddresses.Commands
{
    public class UpdateCustomerAddressCommand : IRequest
    {
        public int Id { get; set; }
        public CustomerAddressLabel Label { get; set; }
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
