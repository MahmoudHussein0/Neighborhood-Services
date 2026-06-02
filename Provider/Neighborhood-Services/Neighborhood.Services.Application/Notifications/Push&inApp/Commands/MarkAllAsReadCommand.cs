using MediatR;
using Neighborhood.Services.Application.Messages.DTOs;
using Neighborhood.Services.Application.Notifications.Push_inApp.DTOs;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Notifications.Push_inApp.Commands
{

    public class MarkAllAsReadCommandDto: IRequest
    {
        public string userId { set; get; }
    }
    public class MarkAllAsReadCommand : IRequestHandler<MarkAllAsReadCommandDto>
    {
        private readonly INotificationsRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public MarkAllAsReadCommand(INotificationsRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task Handle(MarkAllAsReadCommandDto request, CancellationToken cancellationToken)
        {

            await _repository.MarkAllAsReadAsync(_currentUser.UserId);

        }
    }
}
