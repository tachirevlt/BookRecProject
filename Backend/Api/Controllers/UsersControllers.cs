using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Commands;
using Application.Queries;
using Core.Entities;
using Core.Models;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UsersController(ISender sender, IUserRepository userRepository) : ControllerBase
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
                    Sex = result.Sex,
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
            if (!IsUserOwnerOrAdmin(UserId)) return Forbid();

            try
            {
                // Command này gọi Handler để cập nhật Username/Email trong DB
                var command = new UpdateUserCommand(UserId, updateData);
                var result = await sender.Send(command);

                // Trả về thông tin mới nhất
                var responseDto = new UserDto
                {
                    UserId = result.UserId,
                    Username = result.Username,
                    Email = result.Email,
                    Sex = result.Sex,
                    Role = result.Role,
                    FavoriteBooks = result.FavoriteBooks
                };

                return Ok(responseDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex) // Lỗi trùng Email/Username
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
            if (!IsUserOwnerOrAdmin(UserId)) return Forbid();

            // Lưu ý: [ApiController] sẽ tự động check ModelState (Validation DTO) và trả về 400 nếu sai.
            
            try
            {
                // Command này gọi Handler để:
                // 1. Lấy User từ DB.
                // 2. Hash password cũ gửi lên -> so sánh với Hash trong DB.
                // 3. Nếu khớp -> Hash password mới -> Lưu vào DB.
                var command = new ChangePasswordCommand(UserId, passwordData.CurrentPassword, passwordData.NewPassword);
                
                await sender.Send(command);

                return Ok(new { message = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex) // Sai mật khẩu cũ hoặc logic nghiệp vụ
            {
                // Trả về BadRequest (400) vì lỗi input từ phía client (sai pass)
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
        [HttpPost("admin/top-up")]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền
        public async Task<IActionResult> TopUpBalanceAsync([FromBody] TopUpRequest request)
        {
            if (request.Amount <= 0) return BadRequest("Số tiền phải lớn hơn 0.");

            var user = await userRepository.GetUserByUsernameAsync(request.Username);
            if (user == null) return NotFound("Không tìm thấy người dùng.");

            user.CurrentBalance += request.Amount;
            await userRepository.SaveChangesAsync();

            return Ok(new { message = $"Nạp thành công {request.Amount} cho {request.Username}. Số dư mới: {user.CurrentBalance}" });
        }

        [HttpPost("{userId}/purchase/{bookId}")]
        [Authorize]
        public async Task<IActionResult> PurchaseBookAsync([FromRoute] Guid userId, [FromRoute] Guid bookId)
        {
            if (!IsUserOwnerOrAdmin(userId)) return Forbid();

            try
            {
                var success = await userRepository.PurchaseBookAsync(userId, bookId);
                if (!success) return BadRequest("Số dư không đủ để mua cuốn sách này.");

                return Ok(new { message = "Mua sách thành công. Sách đã được thêm vào bộ sưu tập của bạn." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = "Lỗi hệ thống.", error = ex.Message }); }
        }

        // Model phụ cho request nạp tiền
        public class TopUpRequest
        {
            public string Username { get; set; } = null!;
            public decimal Amount { get; set; }
        }


    }


}
