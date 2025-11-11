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
    
    public class UsersController(ISender sender) : ControllerBase
    {
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
        [HttpPost("{userId}/favorites/{bookId}")]
        public async Task<IActionResult> AddFavoriteBookAsync([FromRoute] Guid userId, [FromRoute] Guid bookId)
        {
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
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid UserId)
        {
            var result = await sender.Send(new GetUserByIdQuery(UserId));
            if (result == null)
            {
                return NotFound($"Không tìm thấy sách với ID: {UserId}"); 
            }
            return Ok(result);
        }


        [HttpPut("{UserId}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] Guid UserId, [FromBody] UserEntity User)
        {
            var result = await sender.Send(new UpdateUserCommand(UserId, User));
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete("{UserId}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid UserId)
        {
            var success = await sender.Send(new DeleteUserCommand(UserId));
            if (!success)
            {
                return NotFound($"Không tìm thấy sách với ID: {UserId} để xóa.");
            }
            return Ok(new { message = "Xóa sách thành công." });

        }

        /// Endpoint để Đăng ký (Tạo User mới)
        [HttpPost("register")] 
        [AllowAnonymous] 
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegistrationDto userDto)
        {
            var command = new AddUserCommand(userDto);
            var result = await sender.Send(command);
            return Ok(result);
        }
    }


}
