using Neighborhood.Services.Application.Offers.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Offers.Interfaces
{
    public interface IOfferRepository : IGenericRepository<Offer, int>
    {
        Task<IEnumerable<Offer>> GetOffersByServiceRequestAsync(int serviceRequestId);
        Task<IEnumerable<Offer>> GetTechnicianOffersAsync(int technicianId);
        Task<PagedResult<OfferDto>> GetTechnicianOffersPagedAsync(int technicianId, OfferStatus? status, int page, int pageSize);
        Task<Offer?> GetAcceptedOfferAsync(int serviceRequestId);
        Task<Offer?> GetOfferWithDetailsAsync(int offerId);
    }
}
