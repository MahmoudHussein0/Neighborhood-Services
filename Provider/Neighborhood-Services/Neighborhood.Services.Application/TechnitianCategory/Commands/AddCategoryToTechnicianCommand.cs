using MediatR;


namespace Neighborhood.Services.Application.TechnitianCategory.Commands
{
    public class AddCategoryToTechnicianCommand  : IRequest<int>
    {
       

        public int CategoryId { get; set; }
        public AddCategoryToTechnicianCommand(int categoryId)
        {
            CategoryId = categoryId;
        }

    }
}
