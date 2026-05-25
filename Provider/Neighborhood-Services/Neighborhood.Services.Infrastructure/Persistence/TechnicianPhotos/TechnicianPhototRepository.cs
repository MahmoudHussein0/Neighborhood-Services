using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;
using Neighborhood.Services.Domain.TechnicianPhotos;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.TechnicianPhotos
{
    public class TechnicianPhototRepository : GenericRepository<TechnicianPhoto, int>, ITechnicianPhotoRepository
    {
        public TechnicianPhototRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
