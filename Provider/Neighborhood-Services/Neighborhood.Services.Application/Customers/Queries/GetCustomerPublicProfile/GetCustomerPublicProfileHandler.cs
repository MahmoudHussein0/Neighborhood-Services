using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.PublicProfiles.DTOs;

namespace Neighborhood.Services.Application.Customers.Queries
{
    public class GetCustomerPublicProfileHandler(ICustomerRepository customerRepository)
        : IRequestHandler<GetCustomerPublicProfileQuery, PublicProfileDto>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task<PublicProfileDto> Handle(GetCustomerPublicProfileQuery request, CancellationToken cancellationToken)
        {
            return await _customerRepository.GetPublicProfileAsync(request.CustomerId)
                ?? throw new NotFoundException("Customer", request.CustomerId);
        }
    }
}
