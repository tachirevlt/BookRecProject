using MediatR;
using Application.Events;
using Core.Entities;
using Core.Interfaces; 
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record AddBookCommand(BookEntity Book) : IRequest<BookEntity>;

    public class AddBookCommandHandler(IBookRepository bookRepository, IPublisher mediator)
        : IRequestHandler<AddBookCommand, BookEntity>
    {
        public async Task<BookEntity> Handle(AddBookCommand request, CancellationToken cancellationToken)
        {
            if (request.Book.book_id == Guid.Empty)
            {
                request.Book.book_id = Guid.NewGuid();
            }
            var createdBook = await bookRepository.AddBookAsync(request.Book, cancellationToken);

            await mediator.Publish(new BookCreatedEvent(createdBook.book_id), cancellationToken); 

            return createdBook;
        }
    }
}