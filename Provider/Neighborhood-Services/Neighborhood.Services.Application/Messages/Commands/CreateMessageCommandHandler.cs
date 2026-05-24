using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Application.Messages;
using Neighborhood.Services.Application.Conversations;
using Neighborhood.Services.Application.Messages.DTOs;

namespace Neighborhood.Services.Application.Messages.Commands
{
    public class CreateMessageCommandHandler
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;

        public CreateMessageCommandHandler(IMessageRepository messageRepository, IConversationRepository conversationRepository)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            //user id to verify authorization
        }

        public async Task<MessageCreatedDto> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var msg = new Message
            {
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                content = request.content,
                isRead = request.isRead
            };

            //adding message to conversation
            Conversation conv = await _conversationRepository.GetByIdAsync(msg.ConversationId);

            //if it exists:
            if (conv != null)
            {
                conv.Messages.Add(msg);
                await _conversationRepository.UpdateAsync(conv);

            }

            //if it doesn't exist
            if (conv == null)
            {
                conv = new Conversation();
                conv.Messages = new List<Message>();
                conv.Messages.Add(msg);
                await _conversationRepository.AddAsync(conv);

            }

            await _messageRepository.AddAsync(msg);

            return new MessageCreatedDto() { senderId = request.SenderId ,content=request.content};
            //return msg.Id;
        }
    }
}
