using MediatR;
using Microsoft.IdentityModel.Tokens;
using Neighborhood.Services.Application.Conversations.Commands;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Messages.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.Commands
{
    public class UpdateMessageCommandDTO : IRequest<MessageUpdatedDto>
    {
        public int MessageId { set; get; }
        //الرسالة بتتحدث أنها تتحول من غير مقروءة لمقروءة، فقط لا غير.
    }

    public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommandDTO, MessageUpdatedDto>
    {
        private readonly IMessageRepository _messagerepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMessageCommandHandler(IMessageRepository Messagerepository,IUnitOfWork UnitOfWork)
        {
            this._messagerepository=Messagerepository;
            this._unitOfWork=UnitOfWork;
            
        }
        public async Task<MessageUpdatedDto> Handle(UpdateMessageCommandDTO request, CancellationToken cancellationToken)
        {
            Message mssg = await _messagerepository.GetByIdAsync(request.MessageId);

            if (mssg == null) { return null;}
            mssg.isRead = true;

            await _messagerepository.UpdateAsync(mssg);
            await _unitOfWork.SaveChangesAsync();
            return new MessageUpdatedDto
            {
                id = mssg.Id,
                content = mssg.content,
                Read = mssg.isRead,
                Deleted=mssg.IsDeleted
            };
           
        }
    }
}
