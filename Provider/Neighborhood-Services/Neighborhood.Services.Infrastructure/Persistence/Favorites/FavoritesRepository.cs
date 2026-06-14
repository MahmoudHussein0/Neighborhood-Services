using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Conversations;
using Neighborhood.Services.Application.Favorites;
using Neighborhood.Services.Application.Favorites.DTOs;
using Neighborhood.Services.Domain.favorites;
using Neighborhood.Services.Domain.Technicians;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Favorites
{
    public class FavoritesRepository : GenericRepository<Domain.favorites.Favorite, int>, IFavoritesRepository
    {
        public FavoritesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Domain.favorites.Favorite>> GetByAppUserId(string userId)
        {
            return await _context.Favorites
                .Include(e=>e.Technician).ThenInclude(e=>e.TechnicianPhotos)
                .Where(e => e.UserId == userId && e.IsDeleted==false)
                .ToListAsync();
        }

        public async Task<bool> CheckIfExists(string userId,int TechnicianId)
        {
            var res = await
                   _context.Favorites
                  .Include(e => e.Technician)
                  .Include(e => e.Customer)
                  .Where(e => e.TechnicianId == TechnicianId && e.UserId == userId)
                  .ToListAsync();

            return res.Count>0;
        }

        public override async Task<IReadOnlyList<Favorite>> GetAllAsync()
        {
            return await _context.Favorites.Where(e => e.IsDeleted == false).ToListAsync();
           
        }

     //   public async Task<IReadOnlyList<FavoriteDto>> GetAllDetailsAsync()

     //   {
     //       var result = await _context.Favorites
     //.Include(f => f.Technician).Include(k=>k.User)
     //.Join(
     //    _context.Users,
     //    f => f.UserId,
     //    u => u.Id,
     //    (f, u) => new FavoriteDto
     //    {
     //        favoriteId = f.Id,
     //        userId=u.Id,
     //        technicianId = f.TechnicianId,
     //        technicianName=u.FullName,
     //        addedAt=f.addedAt
             
     //    }).ToListAsync();


     //       return result;        }
    }
}
