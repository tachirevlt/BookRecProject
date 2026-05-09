using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Queries;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using Core.Models;

namespace Api.Controllers
{
    [Route("api/reviews")]
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

        // Người dùng tạo bình luận (chữ)
        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var user_id))
            {
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });
            }

            var command = new AddReviewCommand { user_id = user_id, book_id = request.book_id, review = request.Review };
            var result = await _mediator.Send(command);

            return Ok(new { message = "Đăng bình luận thành công!" });
        }

        [HttpGet("book/{book_id}")]
        public async Task<IActionResult> GetReviewsByBookId(Guid book_id)
        {
            var query = new GetReviewsByBookIdQuery(book_id);
            var result = await _mediator.Send(query); 
            
            return Ok(result);
        }

        [HttpDelete("{review_id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid review_id)
        {
            try
            {
                // 1. Tìm bình luận để lấy thông tin user_id (Người đã viết nó)
                var getQuery = new GetReviewByIdQuery(review_id);
                var review = await _mediator.Send(getQuery);

                if (review == null)
                {
                    return NotFound(new { message = "Không tìm thấy bình luận." });
                }

                // 2. Đưa user_id của tác giả bình luận vào hàm kiểm tra quyền
                if (!IsUserOwnerOrAdmin(review.user_id))
                {
                    return Forbid();
                }

                // 3. Nếu qua được cửa bảo vệ, tiến hành xóa đích danh
                var command = new DeleteReviewByIdCommand(review_id);
                var success = await _mediator.Send(command);

                if (success)
                {
                    return Ok(new { message = "Đã xóa bình luận thành công." });
                }
                else
                {
                    return BadRequest(new { message = "Xóa bình luận thất bại do lỗi hệ thống." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn.", error = ex.Message });
            }
        }
    }
}