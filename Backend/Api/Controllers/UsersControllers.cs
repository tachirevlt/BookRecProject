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
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                               ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(userIdFromToken)) return false;

            if (User.IsInRole("Admin")) return true;

            return userIdFromToken.Equals(resourceId.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto loginDto)
        {
            try
            {
                var query = new LoginQuery(loginDto);
                var token = await sender.Send(query);

                return Ok(new { Token = token });
            }
            catch (KeyNotFoundException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn.", error = ex.Message });
            }
        }

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
        public async Task<IActionResult> UpdateUserAsync([FromRoute] Guid UserId, [FromBody] UserUpdateDto updateData)
        {
            // Kiểm tra quyền sở hữu
            if (!IsUserOwnerOrAdmin(UserId))
            {
                return Forbid();
            }

            try
            {
                // Command này chỉ xử lý Username và Email
                var command = new UpdateUserCommand(UserId, updateData);
                var result = await sender.Send(command);

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
            catch (ArgumentException ex) // Bắt lỗi trùng email/username
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật thông tin.", error = ex.Message });
            }
        }

        [HttpPut("{UserId}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromRoute] Guid UserId, [FromBody] ChangePasswordDto passwordData)
        {
            // Kiểm tra quyền sở hữu
            if (!IsUserOwnerOrAdmin(UserId))
            {
                return Forbid();
            }

            // Kiểm tra Validate DTO (nếu API Controller không tự check)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Tạo Command đổi mật khẩu (Bạn cần tạo class Command này bên Application Layer)
                var command = new ChangePasswordCommand(UserId, passwordData.CurrentPassword, passwordData.NewPassword);
                
                // Gửi sang Handler để xử lý (Logic check pass cũ, hash pass mới nằm ở Handler)
                await sender.Send(command);

                return Ok(new { message = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex) // Pass cũ sai hoặc validation lỗi logic
            {
                return BadRequest(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi đổi mật khẩu.", error = ex.Message });
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
                return Forbid(); 
            }
            try
            {
                var command = new AddBookToFavoritesCommand(userId, bookId);
                var success = await sender.Send(command);

                if (success)
                {
                    return Ok(new { message = "Đã thêm sách vào danh sách yêu thích." });
                }
                else
                {
                    return Conflict(new { message = "Sách này đã có trong danh sách yêu thích của bạn." });
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
        
        [HttpDelete("{userId}/favorites/{bookId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFavoriteBookAsync([FromRoute] Guid userId, [FromRoute] Guid bookId)
        {
            if (!IsUserOwnerOrAdmin(userId))
            {
                return Forbid();
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
