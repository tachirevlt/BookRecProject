using MediatR;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt; 
using System.Security.Claims; 
using System.Text; 
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens; 

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

            var user = await _userRepository.GetUserByUsernameAsync(request.LoginData.Username, cancellationToken);

            if (user == null)
            {
                // Không tìm thấy User
                throw new KeyNotFoundException("Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.LoginData.Password, user.HashedPassword);

            if (!isPasswordValid)
            {
                throw new KeyNotFoundException("Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtKeyString = _configuration["JwtSettings:Key"]
                ?? throw new InvalidOperationException("Không tìm thấy cấu hình 'JwtSettings:Key' trong appsettings.json.");
            var key = Encoding.UTF8.GetBytes(jwtKeyString);

            if (key.Length < 32)
            {
                throw new InvalidOperationException("Key bí mật (JwtSettings:Key) phải có ít nhất 32 ký tự.");
            }
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
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