using Neighborhood.Services.Domain.Message;
using System;
using System.Collections.Generic;
using System.Text;
using static Neighborhood.Services.Application.Messages.Services.MessageService;
using Neighborhood.Services.Application.Messages;
using Neighborhood.Services.Application.Conversations;

namespace Neighborhood.Services.Application.Messages.Services
{
    public class MessageService
    {
        
            private readonly IConversationRepository _convRepo;

            private readonly IMessageRepository _messageRepo;


            public MessageService(
                 IConversationRepository convRepo,
                IMessageRepository messageRepo
                )
            {
            _convRepo = convRepo;
                _messageRepo = messageRepo;
            }

            
            }
        }
    

