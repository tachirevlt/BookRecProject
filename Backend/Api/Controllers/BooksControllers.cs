using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Commands;
using Application.Queries;
using Core.Entities;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class BooksController(ISender sender) : ControllerBase
    {

        [HttpPost("")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBookAsync([FromBody] BookEntity book)
        { 
            var result = await sender.Send(new AddBookCommand(book));
            return Ok(result);
        }

        [HttpPut("{BookId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBookAsync([FromRoute] Guid BookId, [FromBody] BookEntity Book)
        {
            var result = await sender.Send(new UpdateBookCommand(BookId, Book));
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete("{BookId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBookAsync([FromRoute] Guid BookId)
        {
            var success = await sender.Send(new DeleteBookCommand(BookId));
            if (!success)
            {
                return NotFound($"Không tìm thấy sách với ID: {BookId} để xóa.");
            }
            return Ok(new { message = "Xóa sách thành công." });

        }
        
        [HttpGet("{BookId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookByIdAsync([FromRoute] Guid BookId)
        {
            var result = await sender.Send(new GetBookByIdQuery(BookId));
            if (result == null)
            {
                return NotFound($"Không tìm thấy sách với ID: {BookId}"); 
            }
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedList<BookEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBooks(
            [FromQuery] PaginationParams pagination,
            [FromQuery] BookFilterParams filters,
            CancellationToken cancellationToken)
        {
            var query = new GetAllBooksQuery(pagination, filters);

            var result = await sender.Send(query, cancellationToken);

            return Ok(result);
        }

    }


}
