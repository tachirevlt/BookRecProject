using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Commands;
using Application.Queries;
using Core.Entities;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UsersController(ISender sender) : ControllerBase
    {
        private bool IsUserOwnerOrAdmin(Guid resourceId)
        {
            // 1. Lấy ID từ Token (Claim "sub" hoặc NameIdentifier)
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                               ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(userIdFromToken)) return false;

            // 2. Kiểm tra xem User có phải là Admin không?
            if (User.IsInRole("Admin")) return true;

            // 3. So sánh ID trong token với ID cần thao tác
            return userIdFromToken.Equals(resourceId.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        /// Endpoint để Đăng nhập
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto loginDto)
        {
            try
            {
                var query = new LoginQuery(loginDto);
                var token = await sender.Send(query);

                // Trả về 200 OK với Token
                return Ok(new { Token = token });
            }
            catch (KeyNotFoundException ex)
            {
                // Trả về 401 Unauthorized (Không được phép) nếu sai tên hoặc mật khẩu
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn.", error = ex.Message });
            }
        }

        /// Endpoint để Đăng ký (Tạo User mới)
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegistrationDto userDto)
        {
            try 
            {
                var command = new AddUserCommand(userDto);
                var result = await sender.Send(command); 

                var responseDto = new UserDto
                {
                    UserId = result.UserId,
                    Username = result.Username,
                    Email = result.Email,
                    Role = result.Role,
                    FavoriteBooks = result.FavoriteBooks
                };
                
                return CreatedAtAction("GetUserById", new { UserId = result.UserId }, responseDto);
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server.", error = ex.Message });
            }
        }


        [HttpGet("{UserId}")]
        [Authorize]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid UserId)
        {
            // Lấy ID của người đang gửi Request (từ Token)
            var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                   ?? User.FindFirstValue("sub");
            
            Guid? currentUserId = null;
            if (Guid.TryParse(currentUserIdString, out var parsedId))
            {
                currentUserId = parsedId;
            }

            bool isAdmin = User.IsInRole("Admin");
            var query = new GetUserByIdQuery(UserId, currentUserId, isAdmin);
            var result = await sender.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy user với ID: {UserId}" });
            }

            return Ok(result);
        }


        [HttpPut("{UserId}")]
        [Authorize]
        // Đổi [FromBody] UserEntity -> [FromBody] UserUpdateDto
        public async Task<IActionResult> UpdateUserAsync([FromRoute] Guid UserId, [FromBody] UserUpdateDto updateData)
        {
            // Kiểm tra bảo mật (như bài trước)
            if (!IsUserOwnerOrAdmin(UserId))
            {
                return Forbid();
            }

            try
            {
                var command = new UpdateUserCommand(UserId, updateData);
                var result = await sender.Send(command); // result là UserEntity (chứa pass)

                // 2. FIX LỖI BẢO MẬT: Map sang DTO trước khi trả về
                var responseDto = new UserDto
                {
                    UserId = result.UserId,
                    Username = result.Username,
                    Email = result.Email,
                    Role = result.Role,
                    FavoriteBooks = result.FavoriteBooks
                };

                return Ok(responseDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Bắt lỗi trùng lặp hoặc sai định dạng email
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật.", error = ex.Message });
            }
        }

        [HttpDelete("{UserId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid UserId)
        {
            var success = await sender.Send(new DeleteUserCommand(UserId));
            if (!success)
            {
                return NotFound($"Không tìm thấy người dùng với ID: {UserId} để xóa.");
            }
            return Ok(new { message = "Xóa người dùng thành công." });

        }


        [HttpPost("{userId}/favorites/{bookId}")]
        [Authorize]
        public async Task<IActionResult> AddFavoriteBookAsync([FromRoute] Guid userId, [FromRoute] Guid bookId)
        {
            if (!IsUserOwnerOrAdmin(userId))
            {
                return Forbid(); // Trả về 403 Forbidden
            }
            try
            {
                var command = new AddBookToFavoritesCommand(userId, bookId);
                var success = await sender.Send(command);

                if (success)
                {
                    // Trả về 200 OK nếu thêm thành công
                    return Ok(new { message = "Đã thêm sách vào danh sách yêu thích." });
                }
                else
                {
                    // Trả về 409 Conflict nếu sách đã tồn tại
                    return Conflict(new { message = "Sách này đã có trong danh sách yêu thích của bạn." });
                }
            }
            catch (KeyNotFoundException ex)
            {
                // Trả về 404 Not Found nếu User hoặc Book không tồn tại
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Trả về 500 Internal Server Error cho các lỗi khác
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn.", error = ex.Message });
            }
        }
        
        [HttpDelete("{userId}/favorites/{bookId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFavoriteBookAsync([FromRoute] Guid userId, [FromRoute] Guid bookId)
        {
            if (!IsUserOwnerOrAdmin(userId))
            {
                return Forbid(); // Trả về 403 Forbidden
            }
            try
            {
                var command = new RemoveBookFromFavoritesCommand(userId, bookId);
                var success = await sender.Send(command);

                if (success)
                {
                    return Ok(new { message = "Đã xóa sách khỏi danh sách yêu thích." });
                }
                else
                {
                    // Trả về 404 nếu sách không có trong danh sách của user này
                    return NotFound(new { message = "Sách này không có trong danh sách yêu thích của bạn." });
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn.", error = ex.Message });
            }
        }


    }


}
