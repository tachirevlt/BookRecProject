using MediatR;
using Core.Interfaces;

namespace Application.Commands
{
    public record DeleteUserCommand(Guid UserID) : IRequest<bool>;

    internal class DeleteUserCommandHandler(IUserRepository userRepository)
        : IRequestHandler<DeleteUserCommand, bool>
    {
        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await userRepository.DeleteUserAsync(request.UserID);
        }
    }
}
