using MediatR;
using Application.Events;
using Core.Entities;
using Core.Interfaces; 

namespace Application.Commands
{
    public record AddBookCommand(BookEntity Book) : IRequest<BookEntity>;


    public class AddBookCommandHandler(IBookRepository bookRepository, IPublisher mediator)
        : IRequestHandler<AddBookCommand, BookEntity>
    {
        public async Task<BookEntity> Handle(AddBookCommand request, CancellationToken cancellationToken)
        {
            
            if (request.Book.BookId == Guid.Empty)
            {
                request.Book.BookId = Guid.NewGuid();
            }
            var createdBook = await bookRepository.AddBookAsync(request.Book);

            await mediator.Publish(new BookCreatedEvent(createdBook.BookId), cancellationToken); 

            return createdBook;
        }
    }
}
