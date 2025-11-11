using MediatR;
using Core.Entities;
using Core.Interfaces;
using Core.Models; // Cần cho UserLoginDto
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt; // Cần để tạo Token
using System.Security.Claims; // Cần cho Claims
using System.Text; // Cần cho Encoding
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // Cần để đọc appsettings.json
using Microsoft.IdentityModel.Tokens; // Cần cho SymmetricSecurityKey

namespace Application.Queries
{
    public record LoginQuery(UserLoginDto LoginData) : IRequest<string>; // Trả về string (token)

    public class LoginQueryHandler : IRequestHandler<LoginQuery, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public LoginQueryHandler(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            // 1. Tìm User bằng Username (Cần thêm phương thức này vào IUserRepository)
            var user = await _userRepository.GetUserByUsernameAsync(request.LoginData.Username, cancellationToken);

            if (user == null)
            {
                // Không tìm thấy User
                throw new KeyNotFoundException("Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            // 2. Xác minh mật khẩu
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.LoginData.Password, user.HashedPassword);

            if (!isPasswordValid)
            {
                // Sai mật khẩu
                throw new KeyNotFoundException("Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            // 3. Nếu ĐÚNG: Tạo Token
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Lấy Secret Key từ appsettings.json
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Key"]);

            // Tạo "Claims" (Thông tin bên trong Token)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // ID của User
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // <-- Đây là phần Phân quyền (Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Token hết hạn sau 1 giờ
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}