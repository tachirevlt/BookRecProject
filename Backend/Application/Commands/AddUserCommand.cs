using MediatR;
using Application.Events;
using Core.Entities;
using Core.Interfaces; 

namespace Application.Commands
{
    public record AddUserCommand(UserEntity User) : IRequest<UserEntity>;


    public class AddUserCommandHandler(IUserRepository userRepository, IPublisher mediator)
        : IRequestHandler<AddUserCommand, UserEntity>
    {
        public async Task<UserEntity> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            
            if (request.User.UserId == Guid.Empty)
            {
                request.User.UserId = Guid.NewGuid();
            }
            var createdUser = await userRepository.AddUserAsync(request.User);

            await mediator.Publish(new BookCreatedEvent(createdUser.UserId), cancellationToken); 

            return createdUser;
        }
    }
}
