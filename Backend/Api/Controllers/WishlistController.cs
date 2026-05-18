using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Commands;
using Application.Queries;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    [Authorize]
    public class WishlistController(ISender sender) : ControllerBase
    {
        private Guid? GetCurrentUserId()
        {
            var s = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            return Guid.TryParse(s, out var id) ? id : null;
        }

        /// <summary>GET /api/wishlist — Lấy wishlist của người dùng hiện tại</summary>
        [HttpGet]
        public async Task<IActionResult> GetMyWishlist(CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await sender.Send(new GetWishlistQuery(userId.Value), ct);
            return Ok(result);
        }

        /// <summary>POST /api/wishlist/{book_id} — Thêm sách vào wishlist</summary>
        [HttpPost("{book_id}")]
        public async Task<IActionResult> AddToWishlist(
            [FromRoute] Guid book_id,
            [FromQuery] string? collection_name,
            CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var command = new AddToWishlistCommand
                {
                    user_id = userId.Value,
                    book_id = book_id,
                    collection_name = collection_name
                };
                var result = await sender.Send(command, ct);
                return Ok(new { message = "Đã thêm sách vào danh sách yêu thích.", item = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server.", error = ex.Message });
            }
        }

        /// <summary>DELETE /api/wishlist/{book_id} — Xóa sách khỏi wishlist</summary>
        [HttpDelete("{book_id}")]
        public async Task<IActionResult> RemoveFromWishlist([FromRoute] Guid book_id, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await sender.Send(new RemoveFromWishlistCommand { user_id = userId.Value, book_id = book_id }, ct);
            if (!success) return NotFound(new { message = "Sách này không có trong wishlist của bạn." });

            return Ok(new { message = "Đã xóa sách khỏi danh sách yêu thích." });
        }
    }
}
