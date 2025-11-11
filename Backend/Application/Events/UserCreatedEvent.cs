using MediatR;
using System;

namespace Application.Events
{
    public record UserCreatedEvent(Guid UserID) : INotification;
}