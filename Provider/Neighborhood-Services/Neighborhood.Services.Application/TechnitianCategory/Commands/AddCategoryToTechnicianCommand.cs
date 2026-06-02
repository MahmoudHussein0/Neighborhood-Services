using MediatR;


namespace Neighborhood.Services.Application.TechnitianCategory.Commands
{
    public class AddCategoryToTechnicianCommand  : IRequest<int>
    {
        public int TechnicianId { get; set; }
        public int CategoryId { get; set; }

    }
}
