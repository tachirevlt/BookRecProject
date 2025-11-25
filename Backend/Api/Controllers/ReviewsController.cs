using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Queries;
using Core.Models;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private bool IsUserOwnerOrAdmin(Guid resourceId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(userIdString)) return false;

            if (User.IsInRole("Admin")) return true;

            return userIdString.Equals(resourceId.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> AddReview([FromBody] Models.Dtos.AddReviewRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });
            }

            var command = new AddReviewCommand(userId, request.BookId, request.Rating);
            var result = await _mediator.Send(command);

            if (result)
            {
                return Ok(new { message = "Đánh giá sách thành công!" });
            }
            return BadRequest(new { message = "Đánh giá thất bại. Có thể bạn đã đánh giá sách này rồi." });
        }

        [HttpGet("book/{bookId}")] 
        public async Task<IActionResult> GetRatings(Guid bookId)
        {
            var query = new GetRatingsByBookIdQuery(bookId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("book/{bookId}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid bookId, [FromQuery] Guid userId)
        {
            if (!IsUserOwnerOrAdmin(userId))
            {
                return Forbid();
            }

            try
            {
                var command = new DeleteRatingCommand(bookId, userId);
                var result = await _mediator.Send(command);

                if (result)
                {
                    return Ok(new { message = "Xóa đánh giá thành công." });
                }
                return BadRequest(new { message = "Xóa thất bại. Bạn chưa đánh giá sách này hoặc sách không tồn tại." });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn.", error = ex.Message });
            }
        }
    }
}