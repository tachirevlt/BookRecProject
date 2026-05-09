using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Queries;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using Models.Dtos;

namespace Api.Controllers
{
    [Route("api/ratings")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RatingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> AddRating([FromBody] AddRatingRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var user_id))
            {
                return Unauthorized();
            }

            // Sử dụng AddRatingCommand (đã được cấp ở bước trước) để lưu và cập nhật tổng rating vào BookEntity
            var command = new AddRatingCommand { user_id = user_id, book_id = request.book_id, rating = request.Rating };
            var result = await _mediator.Send(command);

            if (result) return Ok(new { message = "Đánh giá sách thành công!" });
            return BadRequest(new { message = "Sách không tồn tại." });
        }

        [HttpDelete("book/{book_id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRating([FromRoute] Guid book_id, [FromQuery] Guid user_id)
        {
            var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(currentUserIdString)) return Unauthorized();

            // Chỉ chủ sở hữu hoặc Admin mới được xóa
            if (!User.IsInRole("Admin") && currentUserIdString != user_id.ToString())
            {
                return Forbid();
            }

            // Sử dụng DeleteRatingCommand (đã cấp ở bước trước)
            var command = new DeleteRatingCommand { user_id = user_id, book_id = book_id };
            var result = await _mediator.Send(command);

            if (result) return Ok(new { message = "Xóa đánh giá thành công." });
            return BadRequest(new { message = "Xóa thất bại. Bạn chưa đánh giá sách này hoặc sách không tồn tại." });
        }
    }
}