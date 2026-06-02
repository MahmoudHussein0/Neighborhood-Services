using MediatR;

namespace Neighborhood.Services.Application.Staffs.Commands
{
    public class DeleteStaffCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
