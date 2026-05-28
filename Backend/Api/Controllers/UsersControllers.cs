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
    [Route("api/users")]
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

        private Guid? GetCurrentUserId()
        {
            var s = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            return Guid.TryParse(s, out var id) ? id : null;
        }

        // ── AUTH ──────────────────────────────────────────────────────────────

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto loginDto)
        {
            try
            {
                var token = await sender.Send(new LoginQuery(loginDto));
                return Ok(new { Token = token });
            }
            catch (KeyNotFoundException ex) { return Unauthorized(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn.", error = ex.Message }); }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegistrationDto userDto)
        {
            try
            {
                var result = await sender.Send(new AddUserCommand(userDto));
                var responseDto = new UserDto
                {
                    user_id = result.user_id,
                    user_name = result.user_name,
                    full_name = result.full_name,
                    email = result.email,
                    role = result.role,
                    sex = result.sex
                };
                // return CreatedAtAction(nameof(GetUserByIdAsync), new { user_id = result.user_id }, responseDto);
                return Created($"/api/users/{result.user_id}", responseDto);
            }
            catch (ArgumentException ex) { return Conflict(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = "Lỗi server.", error = ex.Message }); }
        }

        // ── USER PROFILE ──────────────────────────────────────────────────────

        [HttpGet("{user_id}")]
        [Authorize]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid user_id)
        {
            var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                   ?? User.FindFirstValue("sub");

            Guid? currentuser_id = null;
            if (Guid.TryParse(currentUserIdString, out var parsedId))
                currentuser_id = parsedId;

            bool isAdmin = User.IsInRole("Admin");
            var result = await sender.Send(new GetUserByIdQuery(user_id, currentuser_id, isAdmin));

            if (result == null)
                return NotFound(new { message = $"Không tìm thấy user với ID: {user_id}" });

            return Ok(result);
        }

        [HttpPut("{user_id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] Guid user_id, [FromBody] UserUpdateDto updateData)
        {
            if (!IsUserOwnerOrAdmin(user_id)) return Forbid();
            try
            {
                var result = await sender.Send(new UpdateUserCommand(user_id, updateData));
                var responseDto = new UserDto
                {
                    user_id = result.user_id,
                    user_name = result.user_name,
                    full_name = result.full_name,
                    email = result.email,
                    sex = result.sex,
                    role = result.role
                };
                return Ok(responseDto);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return Conflict(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = "Lỗi khi cập nhật thông tin.", error = ex.Message }); }
        }

        [HttpPut("{user_id}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromRoute] Guid user_id, [FromBody] ChangePasswordDto passwordData)
        {
            if (!IsUserOwnerOrAdmin(user_id)) return Forbid();
            try
            {
                await sender.Send(new ChangePasswordCommand(user_id, passwordData.CurrentPassword, passwordData.NewPassword));
                return Ok(new { message = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = "Lỗi khi đổi mật khẩu.", error = ex.Message }); }
        }

        [HttpDelete("{user_id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid user_id)
        {
            var success = await sender.Send(new DeleteUserCommand(user_id));
            if (!success) return NotFound($"Không tìm thấy người dùng với ID: {user_id} để xóa.");
            return Ok(new { message = "Xóa người dùng thành công." });
        }


        // ── PURCHASE ──────────────────────────────────────────────────────────

        [HttpPost("me/purchase/{book_id}")]
        [Authorize]
        public async Task<IActionResult> PurchaseBookAsync([FromRoute] Guid book_id, CancellationToken ct)
        {
            // Lấy user_id trực tiếp từ Token (Xác nhận tại Backend)
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) 
                return Unauthorized(new { message = "Không xác định được danh tính người dùng hoặc token không hợp lệ." });

            try
            {
                // Truyền user_id đã xác thực vào Command
                var command = new PurchaseBookCommand { user_id = currentUserId.Value, book_id = book_id };
                var result = await sender.Send(command, ct);
                
                return Ok(new { message = "Mua sách thành công. Sách đã được thêm vào thư viện của bạn." });
            } 
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = "Lỗi hệ thống.", error = ex.Message }); }
        }
        /// <summary>GET /api/users/{user_id}/books — Lấy danh sách sách đã mua</summary>
        [HttpGet("{user_id}/books")]
        [Authorize]
        public async Task<IActionResult> GetMyBooksAsync([FromRoute] Guid user_id, CancellationToken ct)
        {
            if (!IsUserOwnerOrAdmin(user_id)) return Forbid();
            var result = await sender.Send(new GetUserBooksQuery(user_id), ct);
            return Ok(result);
        }

        // ── ADMIN ─────────────────────────────────────────────────────────────

        [HttpPost("admin/top-up")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TopUpBalanceAsync([FromBody] TopUpRequest request)
        {
            if (request.Amount <= 0) return BadRequest("Số tiền phải lớn hơn 0.");
            var user = await userRepository.GetUserByUsernameAsync(request.user_name);
            if (user == null) return NotFound("Không tìm thấy người dùng.");
            user.current_balance += request.Amount;
            await userRepository.SaveChangesAsync();
            return Ok(new { message = $"Nạp thành công {request.Amount} cho {request.user_name}. Số dư mới: {user.current_balance}" });
        }

        public class TopUpRequest
        {
            public string user_name { get; set; } = null!;
            public decimal Amount { get; set; }
        }
    }
}
