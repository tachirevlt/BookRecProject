using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Commands;
using Application.Queries;
using Core.Entities;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/books")]
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

        [HttpPut("{book_id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBookAsync([FromRoute] Guid book_id, [FromBody] BookEntity Book)
        {
            var result = await sender.Send(new UpdateBookCommand(book_id, Book));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{book_id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBookAsync([FromRoute] Guid book_id)
        {
            var success = await sender.Send(new DeleteBookCommand(book_id));
            if (!success) return NotFound($"Không tìm thấy sách với ID: {book_id} để xóa.");
            
            return Ok(new { message = "Xóa sách thành công." });
        }
        
        [HttpGet("{book_id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookByIdAsync([FromRoute] Guid book_id)
        {
            var result = await sender.Send(new GetBookByIdQuery(book_id));
            if (result == null) return NotFound($"Không tìm thấy sách với ID: {book_id}"); 
            
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
        
        [HttpGet("recommendations")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecommendedBooks([FromQuery] int count = 8)
        {
            // Gửi Query chỉ với tham số Count
            var result = await sender.Send(new GetRecommendedBooksQuery(count));
            return Ok(result);
        }
    }
}