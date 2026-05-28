using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Application.Queries
{
    // Đổi thành UserBookDTO
    public record GetUserBooksQuery(Guid UserId) : IRequest<IReadOnlyList<UserBookDTO>>;

    public class GetUserBooksQueryHandler : IRequestHandler<GetUserBooksQuery, IReadOnlyList<UserBookDTO>>
    {
        private readonly IUserBookRepository _userBookRepository;

        public GetUserBooksQueryHandler(IUserBookRepository userBookRepository)
        {
            _userBookRepository = userBookRepository;
        }

        public async Task<IReadOnlyList<UserBookDTO>> Handle(GetUserBooksQuery request, CancellationToken cancellationToken)
        {
            var userBooks = await _userBookRepository.GetUserBooksAsync(request.UserId, cancellationToken);
            return userBooks.Select(ub => UserBookDTO.FromEntity(ub))
                            .Where(dto => dto != null)
                            .ToList()!;
        }
    }
}