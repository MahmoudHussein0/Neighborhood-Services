using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.TechnicianPhotos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnicianPhotos.Interfaces
{
    public interface ITechnicianPhotoRepository : IGenericRepository<TechnicianPhoto, int>
    {
    }
}
