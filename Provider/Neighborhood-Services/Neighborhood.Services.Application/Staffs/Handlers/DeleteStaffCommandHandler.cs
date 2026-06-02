using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Staffs.Commands;
using Neighborhood.Services.Application.Staffs.Interfaces;

namespace Neighborhood.Services.Application.Staffs.Handlers
{
    public class DeleteStaffCommandHandler : IRequestHandler<DeleteStaffCommand, bool>
    {
        private readonly IStaffRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteStaffCommandHandler(IStaffRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteStaffCommand request, CancellationToken cancellationToken)
        {
            var staff = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (staff is null)
                throw new Exception($"Staff with id {request.Id} not found.");

            staff.IsDeleted = true;
            await _repository.UpdateAsync(staff);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
