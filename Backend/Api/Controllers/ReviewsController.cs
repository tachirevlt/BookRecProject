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

        // Người dùng tạo bình luận (chữ)
        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });
            }

            // Gọi Command AddReviewCommand (đã cập nhật ở lần trước)
            var command = new AddReviewCommand { user_id = userId, book_id = request.BookId, review = request.Review };
            var result = await _mediator.Send(command);

            return Ok(new { message = "Đăng bình luận thành công!" });
        }

        // Lấy tất cả bình luận chữ của 1 sách (Có thể dùng API GetReviewsByBookIdQuery nếu bạn đã tạo, hoặc truy xuất trực tiếp)
        // [HttpGet("book/{bookId}")] -> Nếu cần, bạn có thể triển khai thêm query cho nó.
    }
}