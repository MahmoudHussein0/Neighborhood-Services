using MediatR;
using Neighborhood.Services.Application.Messages.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.Commands
{
    public class DeleteMessagCommandDTO: IRequest<MessageUpdatedDto>
    {
        public int MessageId { set; get; }
    }
   

    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessagCommandDTO, MessageUpdatedDto>
    {
        private readonly IMessageRepository _messagerepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMessageCommandHandler(IMessageRepository Messagerepository, IUnitOfWork UnitOfWork)
        {
            this._messagerepository = Messagerepository;
            this._unitOfWork = UnitOfWork;

        }
        public async Task<MessageUpdatedDto> Handle(DeleteMessagCommandDTO request, CancellationToken cancellationToken)
        {
            Message mssg = await _messagerepository.GetByIdAsync(request.MessageId);

            if (mssg == null) { return null!; }
           

            await _messagerepository.DeleteAsync(mssg.Id);
            await _unitOfWork.SaveChangesAsync();
            return new MessageUpdatedDto
            {
                id = mssg.Id,
                content = mssg.content,
                Read = mssg.isRead,
                Deleted = mssg.IsDeleted
            };

        }
    }
}
