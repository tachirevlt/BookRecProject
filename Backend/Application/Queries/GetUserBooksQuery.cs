using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Application.Queries
{
    public record GetUserBooksQuery(Guid UserId) : IRequest<IReadOnlyList<UserBookEntity>>;

    public class GetUserBooksQueryHandler : IRequestHandler<GetUserBooksQuery, IReadOnlyList<UserBookEntity>>
    {
        private readonly IUserBookRepository _userBookRepository;

        public GetUserBooksQueryHandler(IUserBookRepository userBookRepository)
        {
            _userBookRepository = userBookRepository;
        }

        public async Task<IReadOnlyList<UserBookEntity>> Handle(GetUserBooksQuery request, CancellationToken cancellationToken)
        {
            return await _userBookRepository.GetUserBooksAsync(request.UserId, cancellationToken);
        }
    }
}
