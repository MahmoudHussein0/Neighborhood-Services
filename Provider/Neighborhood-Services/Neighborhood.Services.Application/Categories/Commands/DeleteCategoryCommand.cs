using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;

namespace Neighborhood.Services.Application.Categories.Commands
{
    public class DeleteCategoryCommand :IRequest<bool>
    {
        public int  Id { get; set; }

        public DeleteCategoryCommand(int id)
        {
            Id = id;
        }
    }
}
